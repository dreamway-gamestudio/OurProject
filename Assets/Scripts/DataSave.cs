using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;


/// <summary>
/// Простая обёртка над CloudDataManager:
/// - обычные Set идут в очередь (дебаунс/автосейв),
/// - Critical вызывает немедленный флуш очереди,
/// - при отсутствии облака — падаем на PlayerPrefs.
/// </summary>
public static class DataSave
{
    // -------- Обычные (коалесинг + дебаунс через очередь) --------
    public static void SetInt(string key, int value)
    {
        if (CloudDataManager.Instance != null)
            CloudDataManager.Instance.SetInt(key, value);
        else
            PlayerPrefs.SetInt(key, value);
    }

    public static void SetFloat(string key, float value)
    {
        if (CloudDataManager.Instance != null)
            CloudDataManager.Instance.SetFloat(key, value);
        else
            PlayerPrefs.SetFloat(key, value);
    }

    public static void SetString(string key, string value)
    {
        if (CloudDataManager.Instance != null)
            CloudDataManager.Instance.SetString(key, value);
        else
            PlayerPrefs.SetString(key, value);
    }

    public static void SetBool(string key, bool value)
    {
        if (CloudDataManager.Instance != null)
            CloudDataManager.Instance.SetBool(key, value);
        else
            PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    // -------- Критические (кладём в очередь и сразу флушим) --------
    public static async void SetIntCritical(string key, int value)
    {
        try
        {
            if (CloudDataManager.Instance != null)
                await CloudDataManager.Instance.SetIntCritical(key, value);
            else
            {
                PlayerPrefs.SetInt(key, value);
                PlayerPrefs.Save();
            }
        }
        catch (Exception e) { Debug.LogError($"SetIntCritical error: {e.Message}"); }
    }

    public static async void SetFloatCritical(string key, float value)
    {
        if (CloudDataManager.Instance != null)
            await CloudDataManager.Instance.SetFloatCritical(key, value);
        else { PlayerPrefs.SetFloat(key, value); PlayerPrefs.Save(); }
    }

    public static async void SetStringCritical(string key, string value)
    {
        if (CloudDataManager.Instance != null)
            await CloudDataManager.Instance.SetStringCritical(key, value);
        else { PlayerPrefs.SetString(key, value); PlayerPrefs.Save(); }
    }

    public static async void SetBoolCritical(string key, bool value)
    {
        if (CloudDataManager.Instance != null)
            await CloudDataManager.Instance.SetBoolCritical(key, value);
        else { PlayerPrefs.SetInt(key, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    // -------- Пакет --------
    public static async void SetBatch(Dictionary<string, object> data)
    {
        try
        {
            if (CloudDataManager.Instance != null)
                await CloudDataManager.Instance.SetBatch(data);
            else
            {
                foreach (var kv in data)
                {
                    switch (kv.Value)
                    {
                        case int i: PlayerPrefs.SetInt(kv.Key, i); break;
                        case float f: PlayerPrefs.SetFloat(kv.Key, f); break;
                        case bool b: PlayerPrefs.SetInt(kv.Key, b ? 1 : 0); break;
                        default: PlayerPrefs.SetString(kv.Key, kv.Value?.ToString() ?? ""); break;
                    }
                }
                PlayerPrefs.Save();
            }
        }
        catch (Exception e) { Debug.LogError($"SetBatch error: {e.Message}"); }
    }

    // -------- Get --------
    public static int GetInt(string key, int def = 0)
    {
        if (CloudDataManager.Instance != null && CloudDataManager.Instance.IsInitialized)
            return CloudDataManager.Instance.GetInt(key, def);
        return PlayerPrefs.GetInt(key, def);
    }

    public static float GetFloat(string key, float def = 0f)
    {
        if (CloudDataManager.Instance != null && CloudDataManager.Instance.IsInitialized)
            return CloudDataManager.Instance.GetFloat(key, def);
        return PlayerPrefs.GetFloat(key, def);
    }

    public static string GetString(string key, string def = "")
    {
        if (CloudDataManager.Instance != null && CloudDataManager.Instance.IsInitialized)
            return CloudDataManager.Instance.GetString(key, def);
        return PlayerPrefs.GetString(key, def);
    }

    public static bool GetBool(string key, bool def = false)
    {
        if (CloudDataManager.Instance != null && CloudDataManager.Instance.IsInitialized)
            return CloudDataManager.Instance.GetBool(key, def);
        return PlayerPrefs.GetInt(key, def ? 1 : 0) == 1;
    }

    // -------- Сервис --------
    public static async void Save()
    {
        try
        {
            if (CloudDataManager.Instance != null)
                await CloudDataManager.Instance.ForceSync();
            else PlayerPrefs.Save();
        }
        catch (Exception e) { Debug.LogError($"Save error: {e.Message}"); }
    }

    public static bool HasKey(string key)
    {
        if (CloudDataManager.Instance != null && CloudDataManager.Instance.IsInitialized)
            return CloudDataManager.Instance.HasKey(key);
        return PlayerPrefs.HasKey(key);
    }

    public static bool IsCloudAvailable() =>
        CloudDataManager.Instance != null && CloudDataManager.Instance.IsInitialized;

    public static int GetPendingChangesCount() =>
        CloudDataManager.Instance != null ? CloudDataManager.Instance.PendingChangesCount : 0;

    public static string GetPlayerId() =>
        CloudDataManager.Instance != null && CloudDataManager.Instance.IsAuthenticated
        ? CloudDataManager.Instance.PlayerId
        : "Not authenticated";
}
