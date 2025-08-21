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
        Inventory      = GameObject.FindObjectOfType<Inventory>();
        DicePlayerPrefs= GameObject.FindObjectOfType<DicePlayerPrefs>();
        Cards          = GameObject.FindObjectOfType<Cards>();
        LockDice       = GameObject.FindObjectOfType<LockDice>();
        Chests         = GameObject.FindObjectOfType<Chests>();
    
        // 1) один раз заливаем из префабов -> сервер
        while (!DataSave.IsCloudAvailable())
            await System.Threading.Tasks.Task.Yield();

        await DicePlayerPrefs.SeedFromPrefabsOnceAsync();

        // 2) теперь безопасно читать/инициализировать всё остальное
        Inventory.OnlyOneTime();
        Inventory.Init();
        Inventory.InventoryClass();
        Cards.CardsInit();
        LockDice.CheckDiceBuyed();
        Chests.SaveChestsDatas();
    }
}
