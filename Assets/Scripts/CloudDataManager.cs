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
    private bool _destroyed; // ДОБАВЛЕНО: флаг уничтожения

    private readonly object _cacheLock = new object();
    private const int MaxSaveRetries = 5;

    // Один лок — сериализуем все сетевые сохранения (исключает гонки)
    private readonly SemaphoreSlim _saveLock = new(1, 1);

    void Awake()
    {
        // УЛУЧШЕНО: более безопасная проверка синглтона
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Application.quitting += () => _quit = true;
        
        // ДОБАВЛЕНО: запускаем инициализацию через корутину для лучшего контроля
        StartCoroutine(InitializeAsync());
    }

    // ДОБАВЛЕНО: обертка-корутина для безопасной инициализации
    private IEnumerator InitializeAsync()
    {
        var task = InitializeServices();
        
        // Ждем завершения с проверкой на уничтожение
        while (!task.IsCompleted)
        {
            if (_destroyed) yield break;
            yield return null;
        }
        
        if (task.IsFaulted && !_destroyed)
        {
            Debug.LogError($"Init error: {task.Exception?.GetBaseException()?.Message}");
        }
    }

    private async Task InitializeServices()
    {
        try
        {
            // ДОБАВЛЕНО: проверка перед каждым async вызовом
            if (_destroyed) return;
            
            await UnityServices.InitializeAsync();
            
            if (_destroyed) return; // проверка после async операции
            
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            if (_destroyed) return;

            isAuthenticated = true;
            await LoadAllData();

            if (_destroyed) return;

            isInitialized = true;
            
            // ДОБАВЛЕНО: проверка перед запуском корутины
            if (!_destroyed && gameObject != null)
            {
                autoSaveCo = StartCoroutine(AutoSaveRoutine());
            }

            DebugLog("CloudDataManager initialized");
        }
        catch (System.Exception e)
        {
            if (!_destroyed) // логируем только если объект еще жив
            {
                Debug.LogError($"Init error: {e.Message}");
            }
        }
    }

    // ---------- ПУБЛИЧНЫЕ API: УСТАНОВКА ДАННЫХ ----------

    public void Set(string key, object value)
    {
        if (_destroyed) return; // ДОБАВЛЕНО: проверка на уничтожение
        
        lock (_cacheLock)
        {
            cachedData[key] = value;
            pendingChanges[key] = value;
        }

        if (criticalKeys.Contains(key))
        {
            _ = FlushAsync();
        }
        else
        {
            if (delayedSaveCo != null) StopCoroutine(delayedSaveCo);
            if (!_destroyed && gameObject != null) // ДОБАВЛЕНО: проверка перед StartCoroutine
            {
                delayedSaveCo = StartCoroutine(DelayedSaveRoutine());
            }
        }

        DebugLog($"Queued: {key}={value}");
    }

    public async Task SetCritical(string key, object value)
    {
        if (_destroyed) return;
        
        lock (_cacheLock)
        {
            cachedData[key] = value;
            pendingChanges[key] = value;
        }
        await FlushAsync();
        DebugLog($"Critical queued & flushed: {key}={value}");
    }

    public async Task SetBatch(Dictionary<string, object> data)
    {
        if (_destroyed) return;
        
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

    // Обычное удаление: помечаем в кеше и очереди как удаленный. Сохранение — дебаунс/авто.
    public void Delete(string key)
    {
        if (_destroyed) return;
        
        lock (_cacheLock)
        {
            cachedData.Remove(key);
            // Добавляем в очередь специальный маркер удаления
            pendingChanges[key] = null;
        }

        if (criticalKeys.Contains(key))
        {
            // критич. ключ — сразу просим флуш
            _ = FlushAsync();
        }
        else
        {
            if (delayedSaveCo != null) StopCoroutine(delayedSaveCo);
            if (!_destroyed && gameObject != null)
            {
                delayedSaveCo = StartCoroutine(DelayedSaveRoutine());
            }
        }

        DebugLog($"Queued for deletion: {key}");
    }

    // Критическое удаление: помечаем в очереди и сразу флушим
    public async Task DeleteCritical(string key)
    {
        if (_destroyed) return;
        
        lock (_cacheLock)
        {
            cachedData.Remove(key);
            pendingChanges[key] = null;
        }
        
        await FlushAsync();
        DebugLog($"Critical deletion flushed: {key}");
    }

    // ---------- ПУБЛИЧНЫЕ API: ПОЛУЧЕНИЕ ДАННЫХ ----------

    public int GetInt(string key, int def = 0)
    {
        if (_destroyed) return def;
        
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
        if (_destroyed) return def;
        
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
        if (_destroyed) return def;
        
        lock (_cacheLock)
        {
            if (!cachedData.TryGetValue(key, out var v) || v == null) return def;
            return v.ToString();
        }
    }

    public bool GetBool(string key, bool def = false)
    {
        if (_destroyed) return def;
        
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

    public bool HasKey(string key) 
    { 
        if (_destroyed) return false;
        lock (_cacheLock) return cachedData.ContainsKey(key); 
    }

    // ---------- ФЛУШ ОЧЕРЕДИ (ЕДИНСТВЕННАЯ ТОЧКА СЕТИ) ----------

    private async Task FlushAsync()
    {
        if (_quit || !isAuthenticated || _destroyed) return; // ДОБАВЛЕНО: проверка _destroyed
        
        await _saveLock.WaitAsync();
        try
        {
            int attempt = 0;
            while (!_destroyed) // ДОБАВЛЕНО: проверка в цикле
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
                    if (_destroyed) break; // проверка перед сетевым вызовом
                    
                    await CloudSaveService.Instance.Data.Player.SaveAsync(toSave);
                    attempt = 0;
                    DebugLog($"Saved {toSave.Count} items");
                }
                catch (Exception e)
                {
                    if (_destroyed) break;
                    
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

    public async Task ForceSync()
    {
        if (_destroyed) return;
        
        await FlushAsync();
        await LoadAllData();
        DebugLog("Force sync completed");
    }

    // ---------- ЗАГРУЗКА СЕРВЕРНЫХ ДАННЫХ ----------

    public async Task LoadAllData()
    {
        if (!isAuthenticated || _destroyed) return;
        
        try
        {
            var saved = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
            
            if (_destroyed) return; // проверка после async операции
            
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
            if (!_destroyed)
            {
                Debug.LogError($"Load error: {e.Message}");
            }
        }
    }

    // ---------- ЖИЗНЕННЫЙ ЦИКЛ/СЕРВИСНЫЕ ----------

    IEnumerator DelayedSaveRoutine()
    {
        yield return new WaitForSeconds(saveDelay);
        if (!_destroyed) // ДОБАВЛЕНО: проверка перед флушем
        {
            _ = FlushAsync();
        }
        delayedSaveCo = null;
    }

    IEnumerator AutoSaveRoutine()
    {
        while (!_destroyed) // ДОБАВЛЕНО: проверка в условии цикла
        {
            yield return new WaitForSeconds(autoSaveInterval);
            if (!_destroyed)
            {
                _ = FlushAsync();
            }
        }
    }

    async System.Threading.Tasks.Task TryFlushOnSuspend()
    {
        if (_destroyed) return;
        
        try
        {
            var t = CloudDataManager.Instance?.ForceSync() ?? System.Threading.Tasks.Task.CompletedTask;
            await System.Threading.Tasks.Task.WhenAny(t, System.Threading.Tasks.Task.Delay(1000));
        }
        catch { /* молча */ }
    }

    void OnApplicationPause(bool pause) 
    { 
        if (pause && !_destroyed) _ = TryFlushOnSuspend(); 
    }
    
    void OnApplicationFocus(bool focus) 
    { 
        if (!focus && !_destroyed) _ = TryFlushOnSuspend(); 
    }

    void OnDestroy()
    {
        _destroyed = true; // ДОБАВЛЕНО: устанавливаем флаг уничтожения
        
        if (autoSaveCo != null) StopCoroutine(autoSaveCo);
        if (delayedSaveCo != null) StopCoroutine(delayedSaveCo);
        
        // Очищаем Instance только если это действительно мы
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // ---------- Свойства/утилиты ----------
    public bool IsInitialized => isInitialized && !_destroyed;
    public bool IsAuthenticated => isAuthenticated && !_destroyed;
    public string PlayerId => (!_destroyed && AuthenticationService.Instance.IsSignedIn) ? AuthenticationService.Instance.PlayerId : "Not signed in";
    public int PendingChangesCount 
    { 
        get 
        { 
            if (_destroyed) return 0;
            lock (_cacheLock) return pendingChanges.Count; 
        } 
    }

    private void DebugLog(string msg) 
    { 
        if (enableDebugLogs && !_destroyed) 
            Debug.Log($"[CloudDataManager] {msg}"); 
    }
}