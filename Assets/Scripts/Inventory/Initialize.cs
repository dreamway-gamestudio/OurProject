using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks; // <- добавить

public class Initialize : MonoBehaviour
{
    Inventory Inventory;
    DicePlayerPrefs DicePlayerPrefs;
    Cards Cards;
    LockDice LockDice;
    Chests Chests;

    [HideInInspector] public bool InIt = false;

    // делаем Start асинхронным
    async void Start()
    {
        Inventory      = FindObjectOfType<Inventory>();
        DicePlayerPrefs= FindObjectOfType<DicePlayerPrefs>();
        Cards          = FindObjectOfType<Cards>();
        LockDice       = FindObjectOfType<LockDice>();
        Chests         = FindObjectOfType<Chests>();

        // Дожидаемся инициализации облака ЕДИНЫМ способом
        if (CloudDataManager.Instance != null)
            await CloudDataManager.Instance.WaitUntilReadyAsync();
        else
            while (!DataSave.IsCloudAvailable())
                await System.Threading.Tasks.Task.Yield();

        // Дальше твоя логика
        await DicePlayerPrefs.SeedFromPrefabsOnceAsync();
        Inventory.OnlyOneTime();
        Inventory.Init();
        Inventory.InventoryClass();
        Cards.CardsInit();
        LockDice.CheckDiceBuyed();
        Chests.SaveChestsDatas();
    }

}
