using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGamePlay : MonoBehaviour
{
    public Sprite[] AllDiceImages;
    void Start()
    {
        InitInventoryImagesInGamePlay();
    }
    void InitInventoryImagesInGamePlay()
    {
        for(int i = 1; i <= 5; i++)
        {
            string cellDice = PlayerPrefs.GetString($"Dice{i}");
            string[] arrayStr = cellDice.Split(char.Parse("_"));
            string diceName = arrayStr[1];
            GameObject Inventory = GameObject.Find("Inventory");
            for(int j = 0; j<=AllDiceImages.Length-1; j++)
            {
                if(AllDiceImages[j].name == diceName)
                {
                    Inventory.transform.GetChild(i-1).GetComponent<Image>().sprite = AllDiceImages[j];
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
