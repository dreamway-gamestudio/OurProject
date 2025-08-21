using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static int Coins;
    
    [Header("Settings")]
    public int coinsPerClick = 50;
    
    [Header("Save Strategy")]
    [Tooltip("Сохранять ли монеты немедленно при каждом изменении")]
    public bool saveImmediately = false;
    
    [Tooltip("Порог для критического сохранения (если набрано больше - сохранить сразу)")]
    public int criticalSaveThreshold = 1000;
    
    void Start()
    {
        StartCoroutine(InitializeCoins());
    }
    ////
    IEnumerator InitializeCoins()
    {
        // Ждем инициализации
        while (!CloudDataManager.Instance.IsInitialized)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        // Загружаем монеты
        Coins = CloudDataManager.Instance.GetInt("Coins", 0);
        Debug.Log($"Loaded {Coins} coins from cloud save");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CloudDataManager.Instance.Delete("Coins");
        }
        
        // Принудительное сохранение для тестирования
        if (Input.GetKeyDown(KeyCode.S))
        {
            ForceSave();
        }
    }
    
    public static void AddCoins(int amount)
    {
        Coins += amount;
        Debug.Log($"Added {amount} coins. Total: {Coins}");
        
        // Выбираем стратегию сохранения
        SaveCoinsWithStrategy();
    }
    
    public static void SpendCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            Debug.Log($"Spent {amount} coins. Remaining: {Coins}");
            
            // При тратах тоже сохраняем
            SaveCoinsWithStrategy();
        }
        else
        {
            Debug.LogWarning("Not enough coins!");
        }
    }
    
    private static void SaveCoinsWithStrategy()
    {
        if (CloudDataManager.Instance == null || !CloudDataManager.Instance.IsInitialized)
            return;
        
        var coinScript = FindObjectOfType<Coin>();
        if (coinScript == null) return;
        
        // СТРАТЕГИЯ 1: Немедленное сохранение (для критически важных моментов)
        if (coinScript.saveImmediately)
        {
            _ = CloudDataManager.Instance.SetIntCritical("Coins", Coins);
            Debug.Log("Coins saved immediately (critical mode)");
            return;
        }
        
        // СТРАТЕГИЯ 2: Критическое сохранение при превышении порога
        if (Coins >= coinScript.criticalSaveThreshold)
        {
            _ = CloudDataManager.Instance.SetIntCritical("Coins", Coins);
            Debug.Log($"Coins saved immediately (threshold {coinScript.criticalSaveThreshold} reached)");
            return;
        }
        
        // СТРАТЕГИЯ 3: Обычное отложенное сохранение (РЕКОМЕНДУЕТСЯ)
        CloudDataManager.Instance.SetInt("Coins", Coins);
        Debug.Log("Coins queued for delayed save");
    }
    
    // Принудительное сохранение (например, перед важными операциями)
    public static void ForceSave()
    {
        if (CloudDataManager.Instance != null)
        {
            _ = CloudDataManager.Instance.SetIntCritical("Coins", Coins);
            Debug.Log("Coins force-saved!");
        }
    }
    
    void OnDestroy()
    {
        // При уничтожении объекта сохраняем критически
        if (CloudDataManager.Instance != null)
        {
            _ = CloudDataManager.Instance.SetIntCritical("Coins", Coins);
        }
    }
}