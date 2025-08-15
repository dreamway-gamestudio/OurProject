using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using System.Collections;

public class CloudDataManager : MonoBehaviour
{
    public static CloudDataManager Instance;
    
    [Header("Save Strategy Settings")]
    [Tooltip("Интервал для автосохранения (в секундах)")]
    public float autoSaveInterval = 30f;
    
    [Tooltip("Задержка после изменения перед сохранением (в секундах)")]
    public float saveDelay = 3f;
    
    [Tooltip("Максимальное количество изменений перед принудительным сохранением")]
    public int maxPendingChanges = 10;
    
    [Tooltip("Критически важные ключи для немедленного сохранения")]
    public List<string> criticalKeys = new List<string> { "Level", "UnlockedItems", "PurchasedItems" };
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    private Dictionary<string, object> cachedData = new Dictionary<string, object>();
    private Dictionary<string, object> pendingChanges = new Dictionary<string, object>();
    private HashSet<string> changedKeys = new HashSet<string>();
    
    private bool isInitialized = false;
    private bool isAuthenticated = false;
    private bool isSaving = false;
    
    private Coroutine autoSaveCoroutine;
    private Coroutine delayedSaveCoroutine;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeServices();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    async void InitializeServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            DebugLog("Unity Services initialized");
            
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                DebugLog($"Signed in anonymously. Player ID: {AuthenticationService.Instance.PlayerId}");
            }
            
            isAuthenticated = true;
            await LoadAllData();
            isInitialized = true;
            
            // Запускаем автосохранение
            autoSaveCoroutine = StartCoroutine(AutoSaveRoutine());
            
            DebugLog("Optimized Cloud Data Manager initialized successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize services: {e.Message}");
        }
    }
    
    // СТРАТЕГИИ СОХРАНЕНИЯ:
    
    // 1. Немедленное сохранение для критически важных данных
    public async Task SetCritical(string key, object value)
    {
        cachedData[key] = value;
        await SaveImmediately(new Dictionary<string, object> { { key, value } });
        DebugLog($"Critical data saved immediately: {key} = {value}");
    }
    
    // 2. Отложенное сохранение для обычных данных (рекомендуется)
    public void Set(string key, object value)
    {
        cachedData[key] = value;
        pendingChanges[key] = value;
        changedKeys.Add(key);
        
        // Если это критически важный ключ - сохраняем немедленно
        if (criticalKeys.Contains(key))
        {
            _ = SaveImmediately(new Dictionary<string, object> { { key, value } });
            pendingChanges.Remove(key);
            changedKeys.Remove(key);
            return;
        }
        
        // Если слишком много изменений - принудительно сохраняем
        if (pendingChanges.Count >= maxPendingChanges)
        {
            DebugLog("Too many pending changes, forcing save");
            SavePendingChanges();
            return;
        }
        
        // Запускаем отложенное сохранение
        if (delayedSaveCoroutine != null)
        {
            StopCoroutine(delayedSaveCoroutine);
        }
        delayedSaveCoroutine = StartCoroutine(DelayedSaveRoutine());
        
        DebugLog($"Data queued for delayed save: {key} = {value}");
    }
    
    // 3. Массовое сохранение (для множественных изменений)
    public async Task SetBatch(Dictionary<string, object> data)
    {
        // Всегда обновляем кеш и ставим изменения в очередь
        foreach (var kvp in data)
        {
            cachedData[kvp.Key] = kvp.Value;
            pendingChanges[kvp.Key] = kvp.Value;
            changedKeys.Add(kvp.Key);
        }

        if (isAuthenticated && !isSaving)
        {
            // Забираем все накопленные изменения одним запросом
            var dataToSave = new Dictionary<string, object>(pendingChanges);
            pendingChanges.Clear();
            changedKeys.Clear();
            await SaveImmediately(dataToSave);
            DebugLog($"Batch data saved: {dataToSave.Count} items");
        }
        else
        {
            // Отложим сохранение (ожидание инициализации или завершения текущего сейва)
            if (delayedSaveCoroutine != null)
            {
                StopCoroutine(delayedSaveCoroutine);
            }
            delayedSaveCoroutine = StartCoroutine(DelayedSaveRoutine());
            DebugLog($"Batch queued (auth/saving pending): {data.Count} items");
        }
    }
    //5
    // Типизированные методы
    public void SetInt(string key, int value) => Set(key, value);
    public void SetFloat(string key, float value) => Set(key, value);
    public void SetString(string key, string value) => Set(key, value);
    public void SetBool(string key, bool value) => Set(key, value);
    
    // Критически важные типизированные методы
    public async Task SetIntCritical(string key, int value) => await SetCritical(key, value);
    public async Task SetFloatCritical(string key, float value) => await SetCritical(key, value);
    public async Task SetStringCritical(string key, string value) => await SetCritical(key, value);
    public async Task SetBoolCritical(string key, bool value) => await SetCritical(key, value);
    
    // Методы получения данных (без изменений)
    public int GetInt(string key, int defaultValue = 0)
    {
        if (cachedData.ContainsKey(key))
        {
            if (cachedData[key] is int intValue)
                return intValue;
            
            if (int.TryParse(cachedData[key].ToString(), out int parsedValue))
                return parsedValue;
        }
        
        return defaultValue;
    }
    
    public float GetFloat(string key, float defaultValue = 0f)
    {
        if (cachedData.ContainsKey(key))
        {
            if (cachedData[key] is float floatValue)
                return floatValue;
                
            if (float.TryParse(cachedData[key].ToString(), out float parsedValue))
                return parsedValue;
        }
        
        return defaultValue;
    }
    
    public string GetString(string key, string defaultValue = "")
    {
        if (cachedData.ContainsKey(key))
        {
            return cachedData[key]?.ToString() ?? defaultValue;
        }
        
        return defaultValue;
    }
    
    public bool GetBool(string key, bool defaultValue = false)
    {
        if (cachedData.ContainsKey(key))
        {
            if (cachedData[key] is bool boolValue)
                return boolValue;
                
            if (bool.TryParse(cachedData[key].ToString(), out bool parsedValue))
                return parsedValue;
        }
        
        return defaultValue;
    }
    
    // ВНУТРЕННИЕ МЕТОДЫ СОХРАНЕНИЯ:
    
    private async Task SaveImmediately(Dictionary<string, object> data)
    {
        if (!isAuthenticated || isSaving)
            return;
        
        isSaving = true;
        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            DebugLog($"Immediate save completed: {data.Count} items");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed immediate save: {e.Message}");
        }
        finally
        {
            isSaving = false;
        }
    }
    
    private async void SavePendingChanges()
    {
        if (!isAuthenticated || isSaving || pendingChanges.Count == 0)
            return;
        
        var dataToSave = new Dictionary<string, object>(pendingChanges);
        pendingChanges.Clear();
        changedKeys.Clear();
        
        if (delayedSaveCoroutine != null)
        {
            StopCoroutine(delayedSaveCoroutine);
            delayedSaveCoroutine = null;
        }
        
        await SaveImmediately(dataToSave);
    }
    
    // КОРУТИНЫ:
    
    IEnumerator DelayedSaveRoutine()
    {
        yield return new WaitForSeconds(saveDelay);
        SavePendingChanges();
    }
    
    IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            
            if (pendingChanges.Count > 0)
            {
                DebugLog("Auto-save triggered");
                SavePendingChanges();
            }
        }
    }
    
    // МЕТОДЫ ЗАГРУЗКИ И УПРАВЛЕНИЯ:
    
    public async Task LoadAllData()
    {
        if (!isAuthenticated)
        {
            Debug.LogWarning("Cannot load data: not authenticated");
            return;
        }
        
        try
        {
            var savedData = await CloudSaveService.Instance.Data.Player.LoadAllAsync();
            
            cachedData.Clear();
            foreach (var item in savedData)
            {
                cachedData[item.Key] = item.Value.Value.GetAs<object>();
            }
            
            DebugLog($"Loaded {cachedData.Count} items from cloud");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message}");
        }
    }
    
    public async Task ForceSync()
    {
        if (pendingChanges.Count > 0)
        {
            await SavePendingChangesAsync();
        }
        await LoadAllData();
        DebugLog("Force sync completed");
    }
    
    private async Task SavePendingChangesAsync()
    {
        if (!isAuthenticated || isSaving || pendingChanges.Count == 0)
            return;
        
        var dataToSave = new Dictionary<string, object>(pendingChanges);
        pendingChanges.Clear();
        changedKeys.Clear();
        
        if (delayedSaveCoroutine != null)
        {
            StopCoroutine(delayedSaveCoroutine);
            delayedSaveCoroutine = null;
        }
        
        await SaveImmediately(dataToSave);
    }
    
    public bool HasKey(string key) => cachedData.ContainsKey(key);
    public bool IsInitialized => isInitialized;
    public bool IsAuthenticated => isAuthenticated;
    public string PlayerId => AuthenticationService.Instance.PlayerId;
    public int PendingChangesCount => pendingChanges.Count;
    
    private void DebugLog(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[CloudDataManager] {message}");
        }
    }
    
    // СОБЫТИЯ ПРИЛОЖЕНИЯ:
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && isInitialized)
        {
            DebugLog("App paused - saving all pending changes");
            SavePendingChanges();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && isInitialized)
        {
            DebugLog("App lost focus - saving all pending changes");
            SavePendingChanges();
        }
    }
    
    void OnDestroy()
    {
        if (autoSaveCoroutine != null)
            StopCoroutine(autoSaveCoroutine);
        
        if (delayedSaveCoroutine != null)
            StopCoroutine(delayedSaveCoroutine);
        
        SavePendingChanges();
    }
}