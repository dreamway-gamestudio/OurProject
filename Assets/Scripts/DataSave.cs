using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Оптимизированная обертка для облачных сохранений с разными стратегиями
/// </summary>
public static class DataSave
{
    // ОБЫЧНЫЕ МЕТОДЫ (отложенное сохранение - РЕКОМЕНДУЕТСЯ)
    public static void SetInt(string key, int value)
    {
        if (CloudDataManager.Instance != null)
        {
            CloudDataManager.Instance.SetInt(key, value);
        }
        else
        {
            DataSave.SetInt(key, value);
        }
    }
    
    public static void SetFloat(string key, float value)
    {
        if (CloudDataManager.Instance != null)
        {
            CloudDataManager.Instance.SetFloat(key, value);
        }
        else
        {
            DataSave.SetFloat(key, value);
        }
    }
    
    public static void SetString(string key, string value)
    {
        if (CloudDataManager.Instance != null)
        {
            CloudDataManager.Instance.SetString(key, value);
        }
        else
        {
            DataSave.SetString(key, value);
        }
    }
    
    public static void SetBool(string key, bool value)
    {
        if (CloudDataManager.Instance != null)
        {
            CloudDataManager.Instance.SetBool(key, value);
        }
        else
        {
            DataSave.SetInt(key, value ? 1 : 0);
        }
    }
    
    // КРИТИЧЕСКИЕ МЕТОДЫ (немедленное сохранение)
    public static async void SetIntCritical(string key, int value)
    {
        if (CloudDataManager.Instance != null)
        {
            await CloudDataManager.Instance.SetIntCritical(key, value);
        }
        else
        {
            DataSave.SetInt(key, value);
            DataSave.Save();
        }
    }
    
    public static async void SetFloatCritical(string key, float value)
    {
        if (CloudDataManager.Instance != null)
        {
            await CloudDataManager.Instance.SetFloatCritical(key, value);
        }
        else
        {
            DataSave.SetFloat(key, value);
            DataSave.Save();
        }
    }
    
    public static async void SetStringCritical(string key, string value)
    {
        if (CloudDataManager.Instance != null)
        {
            await CloudDataManager.Instance.SetStringCritical(key, value);
        }
        else
        {
            DataSave.SetString(key, value);
            DataSave.Save();
        }
    }
    
    public static async void SetBoolCritical(string key, bool value)
    {
        if (CloudDataManager.Instance != null)
        {
            await CloudDataManager.Instance.SetBoolCritical(key, value);
        }
        else
        {
            DataSave.SetInt(key, value ? 1 : 0);
            DataSave.Save();
        }
    }
    
    // МАССОВОЕ СОХРАНЕНИЕ
    public static async void SetBatch(Dictionary<string, object> data)
    {
        if (CloudDataManager.Instance != null)
        {
            await CloudDataManager.Instance.SetBatch(data);
        }
        else
        {
            foreach (var kvp in data)
            {
                string key = kvp.Key;
                object value = kvp.Value;
                
                if (value is int intVal)
                    DataSave.SetInt(key, intVal);
                else if (value is float floatVal)
                    DataSave.SetFloat(key, floatVal);
                else if (value is bool boolVal)
                    DataSave.SetInt(key, boolVal ? 1 : 0);
                else if (value is string stringVal)
                    DataSave.SetString(key, stringVal);
                else
                    DataSave.SetString(key, value.ToString());
            }
            DataSave.Save();
        }
    }
    
    // МЕТОДЫ ПОЛУЧЕНИЯ ДАННЫХ
    public static int GetInt(string key, int defaultValue = 0)
    {
        if (CloudDataManager.Instance != null && CloudDataManager.Instance.IsInitialized)
        {
            return CloudDataManager.Instance.GetInt(key, defaultValue);
        }
        else
        {
            return DataSave.GetInt(key, defaultValue);
        }
    }
    
    public static float GetFloat(string key, float defaultValue = 0f)
    {
        if (CloudDataManager.Instance != null && CloudDataManager.Instance.IsInitialized)
        {
            return CloudDataManager.Instance.GetFloat(key, defaultValue);
        }
        else
        {
            return DataSave.GetFloat(key, defaultValue);
        }
    }
    
    public static string GetString(string key, string defaultValue = "")
    {
        if (CloudDataManager.Instance != null && CloudDataManager.Instance.IsInitialized)
        {
            return CloudDataManager.Instance.GetString(key, defaultValue);
        }
        else
        {
            return DataSave.GetString(key, defaultValue);
        }
    }
    
    public static bool GetBool(string key, bool defaultValue = false)
    {
        if (CloudDataManager.Instance != null && CloudDataManager.Instance.IsInitialized)
        {
            return CloudDataManager.Instance.GetBool(key, defaultValue);
        }
        else
        {
            return DataSave.GetInt(key, defaultValue ? 1 : 0) == 1;
        }
    }
    
    // УТИЛИТЫ
    public static bool HasKey(string key)
    {
        if (CloudDataManager.Instance != null && CloudDataManager.Instance.IsInitialized)
        {
            return CloudDataManager.Instance.HasKey(key);
        }
        else
        {
            return DataSave.HasKey(key);
        }
    }
    
    public static async void Save()
    {
        if (CloudDataManager.Instance != null)
        {
            await CloudDataManager.Instance.ForceSync();
        }
        else
        {
            DataSave.Save();
        }
    }
    
    // ИНФОРМАЦИЯ О СИСТЕМЕ
    public static bool IsCloudAvailable()
    {
        return CloudDataManager.Instance != null && CloudDataManager.Instance.IsInitialized;
    }
    
    public static int GetPendingChangesCount()
    {
        if (CloudDataManager.Instance != null)
        {
            return CloudDataManager.Instance.PendingChangesCount;
        }
        return 0;
    }
    
    public static string GetPlayerId()
    {
        if (CloudDataManager.Instance != null && CloudDataManager.Instance.IsAuthenticated)
        {
            return CloudDataManager.Instance.PlayerId;
        }
        return "Not authenticated";
    }
}