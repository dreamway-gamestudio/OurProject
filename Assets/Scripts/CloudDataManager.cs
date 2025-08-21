using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

public class CloudDataManager : MonoBehaviour
{
    public static CloudDataManager Instance;

    [Header("Save Strategy Settings")]
    [Tooltip("Интервал автосохранения (сек)")]
    public float autoSaveInterval = 30f;

    [Tooltip("Дебаунс перед обычным сохранением (сек)")]
    public float saveDelay = 0.75f;

    [Tooltip("Ключи, которые считаем критическими")]
    public List<string> criticalKeys = new() { "Diamonds", "Level", "UnlockedItems" };

    [Header("Debug")]
    public bool enableDebugLogs = true;

    // Локальный кеш текущего состояния
    private readonly Dictionary<string, object> cachedData = new();

    // Очередь изменений (сливаем сюда все Set/SetCritical)
    private readonly Dictionary<string, object> pendingChanges = new();

    private Coroutine autoSaveCo;
    private Coroutine delayedSaveCo;

    private bool isInitialized;
    private bool isAuthenticated;
    private bool _quit;

    private readonly object _cacheLock = new object();
    private const int MaxSaveRetries = 5;

    // Один лок — сериализуем все сетевые сохранения (исключает гонки)
    private readonly SemaphoreSlim _saveLock = new(1, 1);

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Application.quitting += () => _quit = true; // во время выхода — не сохраняем в сеть
        _ = InitializeServices();
    }

    private async Task InitializeServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            isAuthenticated = true;
            await LoadAllData();

            isInitialized = true;
            autoSaveCo = StartCoroutine(AutoSaveRoutine());

            DebugLog("CloudDataManager initialized");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Init error: {e.Message}");
        }
    }

    // ---------- ПУБЛИЧНЫЕ API: УСТАНОВКА ДАННЫХ ----------

    // Обычное изменение: кладём в кеш и в очередь. Сохранение — дебаунс/авто.
    public void Set(string key, object value)
    {
        
        lock (_cacheLock)
        {
            cachedData[key] = value;
            pendingChanges[key] = value;
        }

        if (criticalKeys.Contains(key))
        {
            // критич. ключ — сразу просим флуш (но всё равно через очередь)
            _ = FlushAsync();
        }
        else
        {
            if (delayedSaveCo != null) StopCoroutine(delayedSaveCo);
            delayedSaveCo = StartCoroutine(DelayedSaveRoutine());
        }

        DebugLog($"Queued: {key}={value}");
    }

    // Критическое изменение: кладём в очередь и сразу флушим
    public async Task SetCritical(string key, object value)
    {
        lock (_cacheLock)
        {
            cachedData[key] = value;
            pendingChanges[key] = value;
        }
        await FlushAsync();
        DebugLog($"Critical queued & flushed: {key}={value}");
    }

    // Пакет изменений (всё в очередь, затем флуш)
    public async Task SetBatch(Dictionary<string, object> data)
    {
        lock (_cacheLock)
        {
            foreach (var kv in data)
            {
                cachedData[kv.Key] = kv.Value;
                pendingChanges[kv.Key] = kv.Value;
            }
        }
        await FlushAsync();
        DebugLog($"Batch queued: {data.Count} items");
    }

    // Типизированные удобные методы
    public void SetInt(string key, int v) => Set(key, v);
    public void SetFloat(string key, float v) => Set(key, v);
    public void SetString(string key, string v) => Set(key, v);
    public void SetBool(string key, bool v) => Set(key, v);

    public Task SetIntCritical(string key, int v) => SetCritical(key, v);
    public Task SetFloatCritical(string key, float v) => SetCritical(key, v);
    public Task SetStringCritical(string key, string v) => SetCritical(key, v);
    public Task SetBoolCritical(string key, bool v) => SetCritical(key, v);

    // ---------- ПУБЛИЧНЫЕ API: ПОЛУЧЕНИЕ ДАННЫХ ----------

    public int GetInt(string key, int def = 0)
    {
        lock (_cacheLock)
        {
            if (!cachedData.TryGetValue(key, out var v) || v == null) return def;
            switch (v)
            {
                case int i: return i;
                case long l: return (int)l;
                case float f: return (int)f;
                case double d: return (int)d;
                case string s when int.TryParse(s, out var p): return p;
                case bool b: return b ? 1 : 0;
                default: return def;
            }
        }
    }

    public float GetFloat(string key, float def = 0f)
    {
        lock (_cacheLock)
        {
            if (!cachedData.TryGetValue(key, out var v) || v == null) return def;
            switch (v)
            {
                case float f: return f;
                case double d: return (float)d;
                case int i: return i;
                case long l: return (float)l;
                case string s when float.TryParse(s, out var p): return p;
                default: return def;
            }
        }
    }

    public string GetString(string key, string def = "")
    {
        lock (_cacheLock)
        {
            if (!cachedData.TryGetValue(key, out var v) || v == null) return def;
            return v.ToString();
        }
    }

    public bool GetBool(string key, bool def = false)
    {
        lock (_cacheLock)
        {
            if (!cachedData.TryGetValue(key, out var v) || v == null) return def;
            switch (v)
            {
                case bool b: return b;
                case int i: return i != 0;
                case long l: return l != 0;
                case string s when bool.TryParse(s, out var bp): return bp;
                case string s when int.TryParse(s, out var ip): return ip != 0;
                default: return def;
            }
        }
    }

    public bool HasKey(string key) { lock (_cacheLock) return cachedData.ContainsKey(key); }


    // ---------- ФЛУШ ОЧЕРЕДИ (ЕДИНСТВЕННАЯ ТОЧКА СЕТИ) ----------

    // Сериализованно сохраняем все накопленные изменения; новые изменения во время флуша не теряются
    private async Task FlushAsync()
    {
        if (_quit || !isAuthenticated) return;
        await _saveLock.WaitAsync();
        try
        {
            int attempt = 0;
            while (true)
            {
                Dictionary<string, object> toSave;
                lock (_cacheLock)
                {
                    if (pendingChanges.Count == 0) break;
                    toSave = new Dictionary<string, object>(pendingChanges);
                    pendingChanges.Clear();
                }

                try
                {
                    await CloudSaveService.Instance.Data.Player.SaveAsync(toSave);
                    attempt = 0;
                    DebugLog($"Saved {toSave.Count} items");
                }
                catch (Exception e)
                {
                    // вернуть изменения в очередь
                    lock (_cacheLock)
                        foreach (var kv in toSave) pendingChanges[kv.Key] = kv.Value;

                    attempt++;
                    if (attempt > MaxSaveRetries)
                    {
                        Debug.LogError($"Save failed (max retries). {e.Message}");
                        break;
                    }

                    // экспоненциальный бэкофф с джиттером
                    var delay = Mathf.Min(30f, Mathf.Pow(2, attempt)) + UnityEngine.Random.Range(0f, 0.25f);
                    Debug.LogWarning($"Save retry {attempt} in {delay:F2}s: {e.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(delay));
                }
            }
        }
        finally { _saveLock.Release(); }
    }

    // Принудительная синхронизация: флуш + перезагрузка
    public async Task ForceSync()
    {
        await FlushAsync();
        await LoadAllData();
        DebugLog("Force sync completed");
    }

    // ---------- ЗАГРУЗКА СЕРВЕРНЫХ ДАННЫХ ----------

    public async Task LoadAllData()
    {
        if (!isAuthenticated) return;
        try
        {
            var saved = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
            int count;
            lock (_cacheLock)
            {
                cachedData.Clear();
                foreach (var it in saved)
                    cachedData[it.Key] = it.Value.Value.GetAs<object>();
                count = cachedData.Count;
            }
            DebugLog($"Loaded {count} items");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Load error: {e.Message}");
        }
    }


    // ---------- ЖИЗНЕННЫЙ ЦИКЛ/СЕРВИСНЫЕ ----------

    IEnumerator DelayedSaveRoutine()
    {
        yield return new WaitForSeconds(saveDelay);
        _ = FlushAsync(); // мягкий флуш
        delayedSaveCo = null;
    }

    IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            _ = FlushAsync();
        }
    }

    void OnApplicationPause(bool pause) { if (pause) _ = FlushAsync(); }
    void OnApplicationFocus(bool focus) { if (!focus) _ = FlushAsync(); }

    void OnDestroy()
    {
        if (autoSaveCo != null) StopCoroutine(autoSaveCo);
        if (delayedSaveCo != null) StopCoroutine(delayedSaveCo);
        // ВАЖНО: здесь НЕ трогаем сеть — редактор/приложение уже в выгрузке.
    }

    // ---------- Свойства/утилиты ----------
    public bool IsInitialized => isInitialized;
    public bool IsAuthenticated => isAuthenticated;
    public string PlayerId => AuthenticationService.Instance.IsSignedIn ? AuthenticationService.Instance.PlayerId : "Not signed in";
    public int PendingChangesCount { get { lock (_cacheLock) return pendingChanges.Count; } }

    private void DebugLog(string msg) { if (enableDebugLogs) Debug.Log($"[CloudDataManager] {msg}"); }
}
