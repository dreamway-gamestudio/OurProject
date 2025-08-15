// Assets/Scripts/Systems/UgsSave.cs
// Drop-in замена DataSave на UGS Cloud Save.
// Примеры:
//   UgsSave.SetInt("Coins", 100);
//   int coins = UgsSave.GetInt("Coins", 0);
//   UgsSave.SetString("Nickname", "Wolf");
//   string name = UgsSave.GetString("Nickname", "Player");
//   UgsSave.DataSave("Wave", 12); // авто-тип

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;

public static class UgsSave
{
    // ==== Настройки ====
    public static string EnvironmentName = "production";   // или "development"
    public static string Profile         = "default";      // для мульти-профилей в редакторе
    public static float  FlushInterval   = 5f;             // периодический батч (сек)
    public static int    MaxBatchSize    = 20;             // сколько ключей копим до принудительного сейва
    public static string QueueFilePath   = Path.Combine(Application.persistentDataPath, "ugs_pending.json");

    // ==== Состояние ====
    static bool _initialized;
    static bool _flushing;
    static float _nextFlushAt;
    static readonly Dictionary<string,string> _cache    = new();  // локальные значения как строки
    static readonly HashSet<string> _dirtyKeys          = new();  // ключи, которые надо отправить
    static readonly object _lock = new();

    // ==== ПУБЛИЧНЫЙ API (drop-in) ====

    // Инициализация (вызови один раз на старте проекта или доверяй авто-инициализации)
    public static async Task InitAsync()
    {
        if (_initialized) return;

        // подхватим неотправленную очередь из файла
        LoadPendingFromDisk();

        var opts = new InitializationOptions()
            .SetEnvironmentName(EnvironmentName)
            .SetProfile(Profile);

        await UnityServices.InitializeAsync(opts);

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        _initialized = true;
        _nextFlushAt = Time.realtimeSinceStartup + FlushInterval;

        // Авто-помпа (чтобы не плодить MonoBehaviour): подпишемся на обновления кадра
        UgsSavePump.Ensure().onUpdate = Pump;
        UgsSavePump.Instance.onPause  = () => _ = FlushNowAsync();
        UgsSavePump.Instance.onQuit   = () => _ = FlushNowAsync();
    }

    // ---- INT ----
    public static void   SetInt(string key, int value)         => SetString(key, value.ToString(CultureInfo.InvariantCulture));
    public static int    GetInt(string key, int def = 0)       => TryGetInt(key, out var v) ? v : def;

    // ---- FLOAT ----
    public static void   SetFloat(string key, float value)     => SetString(key, value.ToString(CultureInfo.InvariantCulture));
    public static float  GetFloat(string key, float def = 0f)  => TryGetFloat(key, out var v) ? v : def;

    // ---- STRING ----
    public static void   SetString(string key, string value)
    {
        EnsureInit();
        lock (_lock)
        {
            _cache[key] = value ?? string.Empty;
            _dirtyKeys.Add(key);
        }
    }
    public static string GetString(string key, string def = "")
    {
        EnsureInit();
        lock (_lock) { return _cache.TryGetValue(key, out var v) ? v : def; }
    }

    // ---- BOOL ----
    public static void   SetBool(string key, bool value)       => SetString(key, value ? "1" : "0");
    public static bool   GetBool(string key, bool def=false)
    {
        var s = GetString(key, def ? "1" : "0");
        return s == "1" || s.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    // ---- Универсальный (авто-тип) ----
    public static void DataSave<T>(string key, T value)
    {
        switch (value)
        {
            case null: SetString(key, ""); break;
            case int i: SetInt(key, i); break;
            case float f: SetFloat(key, f); break;
            case bool b: SetBool(key, b); break;
            case string s: SetString(key, s); break;
            case double d: SetString(key, d.ToString(CultureInfo.InvariantCulture)); break;
            case long l: SetString(key, l.ToString(CultureInfo.InvariantCulture)); break;
            default:
                // сложные объекты сериализуем в JSON
                SetString(key, JsonUtility.ToJson(value));
                break;
        }
    }

    public static async Task<string> DataGetStringAsync(string key, string def = "")
    {
        EnsureInit();
        // быстрый возврат из кэша
        lock (_lock) { if (_cache.TryGetValue(key, out var v)) return v; }
        // фоновой fetch
        await FetchKeyAsync(key);
        lock (_lock) { return _cache.TryGetValue(key, out var v) ? v : def; }
    }

    // Сейв принудительно (например, перед сценой)
    public static Task FlushNowAsync() => FlushInternalAsync(force:true);

    // ==== ВНУТРЕННЯЯ ЛОГИКА ====

    static void EnsureInit()
    {
        if (_initialized) return;
        // без await: запустим инициализацию в фоне, чтобы не блокировать игровой цикл
        _ = InitAsync();
    }

    static void Pump()
    {
        if (!_initialized) return;

        // периодический батч
        if (Time.realtimeSinceStartup >= _nextFlushAt)
        {
            _ = FlushInternalAsync();
            _nextFlushAt = Time.realtimeSinceStartup + FlushInterval;
        }

        // триггер по размеру
        lock (_lock)
        {
            if (_dirtyKeys.Count >= MaxBatchSize)
                _ = FlushInternalAsync();
        }
    }

    static async Task FetchKeyAsync(string key)
    {
        try
        {
            var res = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { key }); // Dictionary<string,string>
            if (res != null && res.TryGetValue(key, out var val))
            {
                lock (_lock) { _cache[key] = val ?? ""; }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"UgsSave Fetch '{key}' failed: {e.Message}");
        }
    }

    static async Task FlushInternalAsync(bool force = false)
    {
        if (!_initialized || _flushing) return;

        Dictionary<string, object> payload = null;
        lock (_lock)
        {
            if (_dirtyKeys.Count == 0 && !force) return;
            payload = new Dictionary<string, object>(_dirtyKeys.Count + 1);
            foreach (var k in _dirtyKeys)
                if (_cache.TryGetValue(k, out var v)) payload[k] = v ?? "";
        }

        if (payload == null || payload.Count == 0) return;

        try
        {
            _flushing = true;
            await CloudSaveService.Instance.Data.ForceSaveAsync(payload);
            lock (_lock) { _dirtyKeys.Clear(); }
            // очистим файл очереди
            TryDeleteQueueFile();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"UgsSave Flush failed: {e.Message} — will persist queue and retry later.");
            // запишем очередь на диск
            SavePendingToDisk();
        }
        finally { _flushing = false; }
    }

    static bool TryGetInt(string key, out int value)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var s) &&
                int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                return true;
        }
        value = default;
        return false;
    }

    static bool TryGetFloat(string key, out float value)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var s) &&
                float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                return true;
        }
        value = default;
        return false;
    }

    // ==== Диск: сохранение очереди на случай закрытия ====
    static void SavePendingToDisk()
    {
        try
        {
            Dictionary<string, string> pending;
            lock (_lock)
            {
                pending = new Dictionary<string, string>(_dirtyKeys.Count);
                foreach (var k in _dirtyKeys)
                    if (_cache.TryGetValue(k, out var v)) pending[k] = v ?? "";
            }
            if (pending.Count == 0) return;

            var json = JsonUtility.ToJson(new Pending { items = pending.ToListSerializable() });
            File.WriteAllText(QueueFilePath, json);
        }
        catch (Exception e) { Debug.LogWarning($"UgsSave queue write failed: {e.Message}"); }
    }

    static void LoadPendingFromDisk()
    {
        try
        {
            if (!File.Exists(QueueFilePath)) return;
            var json = File.ReadAllText(QueueFilePath);
            var obj = JsonUtility.FromJson<Pending>(json);
            if (obj?.items != null)
            {
                lock (_lock)
                {
                    foreach (var kv in obj.items)
                    {
                        _cache[kv.key] = kv.value ?? "";
                        _dirtyKeys.Add(kv.key);
                    }
                }
            }
        }
        catch (Exception e) { Debug.LogWarning($"UgsSave queue read failed: {e.Message}"); }
    }

    static void TryDeleteQueueFile()
    {
        try { if (File.Exists(QueueFilePath)) File.Delete(QueueFilePath); }
        catch { /* ignore */ }
    }

    [Serializable] class Pending { public List<KV> items; }
    [Serializable] class KV { public string key; public string value; }
    static List<KV> ToListSerializable(this Dictionary<string, string> dict)
    {
        var list = new List<KV>(dict.Count);
        foreach (var kv in dict) list.Add(new KV { key = kv.Key, value = kv.Value });
        return list;
    }

    // ==== внутренняя помпа без MonoBehaviour в каждом файле ====
    class UgsSavePump : MonoBehaviour
    {
        public static UgsSavePump Instance { get; private set; }
        public Action onUpdate, onPause, onQuit;

        public static UgsSavePump Ensure()
        {
            if (Instance != null) return Instance;
            var go = new GameObject("UgsSavePump");
            DontDestroyOnLoad(go);
            Instance = go.AddComponent<UgsSavePump>();
            return Instance;
        }
        void Update() => onUpdate?.Invoke();
        void OnApplicationPause(bool pause) { if (pause) onPause?.Invoke(); }
        void OnApplicationQuit() => onQuit?.Invoke();
    }
}