using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : MonoBehaviour
{
    Inventory Inventory; // only one time, init, InitDiceInfo, InventoryClass, CardsInit
    DicePlayerPrefs DicePlayerPrefs;
    Cards Cards;
    LockDice LockDice;
    Chests Chests;
    [HideInInspector] public bool InIt = false;
    void Start()
    {
        Inventory = GameObject.FindObjectOfType<Inventory>();
        DicePlayerPrefs = GameObject.FindObjectOfType<DicePlayerPrefs>();
        Cards = GameObject.FindObjectOfType<Cards>();
        LockDice = GameObject.FindObjectOfType<LockDice>();
        Chests = GameObject.FindObjectOfType<Chests>();

        Inventory.OnlyOneTime();
        Inventory.Init();
        DicePlayerPrefs.InitDiceInfo(); 
        Inventory.InventoryClass();
        Cards.CardsInit();
        LockDice.CheckDiceBuyed();
        Chests.SaveChestsDatas();
    }
}
