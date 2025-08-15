using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceLVL : MonoBehaviour
{
    
    public Text LevelText, LevelUpPriceText;
    string thisDiceName;
    [HideInInspector] public int levelStartFrom = 1;
    int priceStartFrom = 20;
    RectTransform LevelTextRT;
    string[] DiceNames = { "Dog", "Cat", "Rabbit", "Pig", "Fox", "Squirrel", "Snake", "Guineapig", "Hedgehog", "Turtle",
                           "Wolf", "Monkey", "Kangaroo", "Koala", "Raccoon", "Horse", "Deer", "Zebra", "Giraffe", "Ostrich",
                           "Bear", "Panda", "Lion", "Tiger", "Panther", "Hippopotamus", "Elephant", "Rhinoceros", "Bull", "Eagle"};
    
    void Start()
    {
        // Invoke("GetDiceName", 0.1f);
        for (int i = 0; i < DiceNames.Length; i++)
        {
            PlayerPrefs.SetInt($"PowerUp{DiceNames[i]}",1);
            PlayerPrefs.SetInt($"GameAttack{DiceNames[i]}", DicePlayerPrefs.GetAttack(DiceNames[i]));
        }
        levelStartFrom = 1;
        priceStartFrom = 20;
        LevelTextRT = LevelText.GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            // print(PlayerPrefs.GetInt($"PowerUpDog"));
            //DicePlayerPrefs.GetAttack(GetDiceName()) = 50;
            // print(DicePlayerPrefs.GetAttack(GetDiceName()));
            //PlayerPrefs.SetInt(GetDiceName() + "Attack", 15000);
        }

        if (levelStartFrom < 5)
        {
            LevelText.text = $"LVL {levelStartFrom}";
            LevelUpPriceText.text = $"{priceStartFrom}";
        }
        else
        {
            LevelText.text = $"MAX";
            LevelTextRT.anchoredPosition = new Vector2(LevelTextRT.anchoredPosition.x, -25);
            LevelUpPriceText.text = $"";
        }

    }

    string GetDiceName()
    {
        thisDiceName = GetComponent<Image>().sprite.name;
        return thisDiceName;
    }

    public void DiceLevelUp()
    {
        // print(GetDiceName());

        if (levelStartFrom < 5)
        {
            if (GamePlayCoins.Coins >= priceStartFrom)
            {
                GamePlayCoins.Coins -= priceStartFrom;
                levelStartFrom++;
                priceStartFrom += 30;
                PlayerPrefs.SetInt($"PowerUp{GetDiceName()}", levelStartFrom);
                // print(PlayerPrefs.GetInt($"GameAttack{GetDiceName()}"));
                // print();
                PlayerPrefs.SetInt($"GameAttack{GetDiceName()}", PlayerPrefs.GetInt($"GameAttack{GetDiceName()}")+(PlayerPrefs.GetInt($"GameAttack{GetDiceName()}")/10*levelStartFrom));
            }
        }
    }
}
