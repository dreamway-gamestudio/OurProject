using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;

public class DicePlayerPrefs : MonoBehaviour
{
    
    GameObject Dice;
    public GameObject[] Dices;
    [HideInInspector] public bool ClassInit = false;
    const string InitKey = "ClassInit";
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            MulAttack("Bear", 1.2f);
        }
    }
    public void UpgradeDice(string DiceName) // levelup i jamanak diceri hzoracum
    {
        print("xaxi yntacqum hzoracum ste grenq"); 
    }

    public async void InitDiceInfo() // заполняем сервер один раз из префабов
    {
        // Если уже инициализировано на сервере — выходим
        ClassInit = DataSave.GetBool("ClassInit", false);
        if (ClassInit) return;

        // Собираем все стартовые значения в один батч
        var batch = new Dictionary<string, object>();

        for (int i = 0; i < Dices.Length; i++)
        {
            var d = Dices[i].GetComponent<Dice>();
            string n = d.DiceName;

            batch[n + "Rarity"]      = d.Rarity.ToString();
            batch[n + "Attack"]      = d.Attack;
            batch[n + "ReloadTime"]  = d.ReloadTime;
            batch[n + "ShootSpeed"]  = d.shootSpeed;
            batch[n + "Target"]      = d.Target.ToString();
            batch[n + "DiamondPrice"]= d.priceWithDiamond;

            // Класс по редкости
            int cls = d.Rarity.ToString() == "Standard"  ? 1 :
                      d.Rarity.ToString() == "Exclusive" ? 3 :
                      5; // Legendary
            batch[n + "Class"] = cls;

            // Стартовые карты
            batch[n + "TotalCards"] = 0;
        }

        // Один батч → на сервер + ставим флаг
        DataSave.SetBatch(batch);                    // батч в облако
        DataSave.SetBoolCritical("ClassInit", true); // флаг инициализации
        DataSave.Save();                             // форсим флуш (на всякий)
        ClassInit = true;
    }

    public async Task SeedFromPrefabsOnceAsync()
    {
        // 1) Гарантируем онлайн и логин
        await WaitCloudAsync();

        // 2) Проверяем ФЛАГ НА СЕРВЕРЕ (без локального кэша)
        if (await IsSeededOnServerAsync()) return;

        // 3) Собираем стартовые данные из префабов
        var batch = new System.Collections.Generic.Dictionary<string, object>();

        for (int i = 0; i < Dices.Length; i++)
        {
            var d = Dices[i].GetComponent<Dice>();
            string n = d.DiceName;

            batch[n + "Rarity"]        = d.Rarity.ToString();
            batch[n + "Attack"]        = d.Attack;
            batch[n + "ReloadTime"]    = d.ReloadTime;
            batch[n + "ShootSpeed"]    = d.shootSpeed;
            batch[n + "Target"]        = d.Target.ToString();
            batch[n + "DiamondPrice"]  = d.priceWithDiamond;

            // Класс по редкости
            int cls = d.Rarity.ToString() == "Standard"  ? 1 :
                      d.Rarity.ToString() == "Exclusive" ? 3 : 5; // Legendary
            batch[n + "Class"] = cls;

            // Стартовые карты
            batch[n + "TotalCards"] = 0;
        }

        // 4) Один раз пишем на сервер
        DataSave.SetBatch(batch);
        DataSave.SetBoolCritical("ClassInit", true);

        // форс-флуш так, чтобы точно увидеть на сервере
        if (CloudDataManager.Instance != null)
            await CloudDataManager.Instance.ForceSync();
    }
    // ===== Хелперы =====
    private static async Task WaitCloudAsync()
    {
        while (!DataSave.IsCloudAvailable()) // ждём CloudDataManager
            await System.Threading.Tasks.Task.Yield();
    }

    private static async Task<bool> IsSeededOnServerAsync()
{
    try
    {
        var keys = new HashSet<string> { InitKey };
        var dict = await Unity.Services.CloudSave.CloudSaveService.Instance.Data.Player.LoadAsync(keys);

        if (dict != null && dict.TryGetValue(InitKey, out var item))
        {
            // item.Value : IDeserializable — извлекаем через GetAs<T>()
            try { return item.Value.GetAs<bool>(); } catch { }
            try { return item.Value.GetAs<int>() != 0; } catch { }
            try { return item.Value.GetAs<long>() != 0L; } catch { }

            try
            {
                var s = item.Value.GetAs<string>();
                if (bool.TryParse(s, out var bp)) return bp;
                if (int.TryParse(s, out var ip)) return ip != 0;
            }
            catch { }

            // если ключ существует, но тип неожиданный — считаем инициализированным
            return true;
        }
    }
    catch
    {
        // при сетевой ошибке считаем, что НЕ инициализировано (чтобы попытаться посеять позже)
    }
    return false;
}

    #region GetValues

    public static string GetRarity(string name)
        => DataSave.GetString(name + "Rarity", "Standard");

    public static int GetAttack(string name)
        => DataSave.GetInt(name + "Attack", 0);

    public static string GetTarget(string name)
        => DataSave.GetString(name + "Target", "Single");

    public static float GetReloadTime(string name)
        => DataSave.GetFloat(name + "ReloadTime", 1f);

    public static float GetShootSpeed(string name)
        => DataSave.GetFloat(name + "ShootSpeed", 1f);

    public static int GetDiamondPrice(string name)
        => DataSave.GetInt(name + "DiamondPrice", 0);

    #endregion

    #region SetValues
    // ===== БАЗОВЫЕ УТИЛИТЫ (работают через DataSave) =====
    
    // ===== ATTACK =====
    public int AddAttack(string name, int delta)                      // +N урона
        => AddInt(name + "Attack", delta, 0);

    public int MulAttack(string name, float mul)                      // × множитель (1.10f = +10%)
        => MulInt(name + "Attack", mul, 0);

    // ===== RELOAD TIME (меньше = лучше) =====
    public float ReduceReloadTime(string name, float delta)           // -N сек, с нижним пределом
        => AddFloat(name + "ReloadTime", -Mathf.Abs(delta), 0.05f);

    public float MulReloadTime(string name, float mul)                // × множитель (0.9f = -10%)
        => MulFloat(name + "ReloadTime", Mathf.Clamp(mul, 0.1f, 10f), 0.05f);

    // ===== SHOOT SPEED (больше = быстрее) =====
    public float AddShootSpeed(string name, float delta)              // +N скорости
        => AddFloat(name + "ShootSpeed", delta, 0.1f);

    public float MulShootSpeed(string name, float mul)                // × множитель (1.15f = +15%)
        => MulFloat(name + "ShootSpeed", Mathf.Max(0.1f, mul), 0.1f);

    // ===== TOTAL CARDS (материал для апгрейда) =====
    public int AddCards(string name, int delta)                       // +N/−N карт, не ниже 0
        => AddInt(name + "TotalCards", delta, 0);

    public void SetCards(string name, int value)                      // жёсткая установка (минимум 0)
        => DataSave.SetInt(name + "TotalCards", Mathf.Max(0, value));

    // ===== DIAMOND PRICE (для баланса/админки) =====
    public int AddDiamondPrice(string name, int delta)                // +N/−N к цене, не ниже 0
        => AddInt(name + "DiamondPrice", delta, 0);

    public void SetDiamondPrice(string name, int value)               // жёсткая установка цены
        => DataSave.SetInt(name + "DiamondPrice", Mathf.Max(0, value));



    static int AddInt(string key, int delta, int min = int.MinValue, int max = int.MaxValue)
    {
        int cur = DataSave.GetInt(key, 0);
        int nv = Mathf.Clamp(cur + delta, min, max);
        if (nv != cur) DataSave.SetInt(key, nv);
        return nv;
    }

    static float AddFloat(string key, float delta, float min = float.MinValue, float max = float.MaxValue)
    {
        float cur = DataSave.GetFloat(key, 0f);
        float nv = Mathf.Clamp(cur + delta, min, max);
        if (!Mathf.Approximately(nv, cur)) DataSave.SetFloat(key, nv);
        return nv;
    }

    static int MulInt(string key, float mul, int min = 0, int max = int.MaxValue)
    {
        int cur = DataSave.GetInt(key, 0);
        int nv = Mathf.Clamp(Mathf.RoundToInt(cur * mul), min, max);
        if (nv != cur) DataSave.SetInt(key, nv);
        return nv;
    }

    static float MulFloat(string key, float mul, float min = 0f, float max = float.MaxValue)
    {
        float cur = DataSave.GetFloat(key, 0f);
        float nv = Mathf.Clamp(cur * mul, min, max);
        if (!Mathf.Approximately(nv, cur)) DataSave.SetFloat(key, nv);
        return nv;
    }

    #endregion

}
