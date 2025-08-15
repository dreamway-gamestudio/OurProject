using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Class : MonoBehaviour
{
    Cards Cards;
    InfoPanel InfoPanelCode;
    Inventory Inventory;
    DicePlayerPrefs DicePlayerPrefs;
    [HideInInspector]
    public string DiceName, RarityName;
    GameObject ReturningGameObject;
    public bool _isUpgarded = false;
    Chests Chests;
    void Start()
    {
        DicePlayerPrefs = GameObject.FindObjectOfType<DicePlayerPrefs>();
        Chests = GameObject.FindObjectOfType<Chests>();
        Cards = GameObject.FindObjectOfType<Cards>();
        InfoPanelCode = GameObject.FindObjectOfType<InfoPanel>();
        Inventory = GameObject.FindObjectOfType<Inventory>();
    }
    public GameObject FindFromInfoPanel(string finding)
    {
        GameObject InfoPanel = GameObject.Find("InfoPanel");
        for (int i = 0; i <= InfoPanel.transform.childCount - 1; i++)
        {
            if (InfoPanel.transform.GetChild(i).name == finding)
            {
                ReturningGameObject = InfoPanel.transform.GetChild(i).gameObject;
                //print(ReturningGameObject.transform.childCount);
                break;
            }
        }
        return ReturningGameObject;
    }
    public void ClassUp()
    {
        if(Coin.Coins >= 500)
        {
            Coin.Coins -= 500;
            _isUpgarded = true;
            RarityName = FindFromInfoPanel("Rarity_Slot").transform.GetChild(1).GetComponent<Text>().text;
            DiceName = FindFromInfoPanel("Name_Text").GetComponent<Text>().text;

            if (RarityName == "Standard")
            { // diceName, rarity
                if (DiceIsUpgrade(DiceName, RarityName)) // ստուգում թե կարա՞ հզորացնի, ստուգումա կլասը ու քարտերի քանակը վերադարձնումա bool
                {
                    PlayerPrefs.SetInt(DiceName + "TotalCards", PlayerPrefs.GetInt(DiceName + "TotalCards") - Cards.standard[PlayerPrefs.GetInt(DiceName + "Class") - 1]); // պահանջվող քարտերը հանումա
                    PlayerPrefs.SetInt(DiceName + "Class", PlayerPrefs.GetInt(DiceName + "Class") + 1); // Class 1-ով ավելացնումա
                    DicePlayerPrefs.UpgradeDice(DiceName);
                    InfoPanelCode.UpdateDiceInfoPanelTextes();
                }
            }
            else if (RarityName == "Exclusive")
            {
                if (DiceIsUpgrade(DiceName, RarityName))
                {
                    PlayerPrefs.SetInt(DiceName + "TotalCards", PlayerPrefs.GetInt(DiceName + "TotalCards") - Cards.exclusive[PlayerPrefs.GetInt(DiceName + "Class") - 3]);
                    PlayerPrefs.SetInt(DiceName + "Class", PlayerPrefs.GetInt(DiceName + "Class") + 1);
                    DicePlayerPrefs.UpgradeDice(DiceName);
                    InfoPanelCode.UpdateDiceInfoPanelTextes();
                }
            }
            else if (RarityName == "Legendary")
            {
                if (DiceIsUpgrade(DiceName, RarityName))
                {
                    PlayerPrefs.SetInt(DiceName + "TotalCards", PlayerPrefs.GetInt(DiceName + "TotalCards") - Cards.legendary[PlayerPrefs.GetInt(DiceName + "Class") - 5]);
                    PlayerPrefs.SetInt(DiceName + "Class", PlayerPrefs.GetInt(DiceName + "Class") + 1);
                    DicePlayerPrefs.UpgradeDice(DiceName);
                    InfoPanelCode.UpdateDiceInfoPanelTextes();
                }
            }
            Cards.CardsInit();
            //DicePlayerPrefs.InitDiceInfo();
            if (PlayerPrefs.GetInt(DiceName + "Class") > 14)
            {
                FindFromInfoPanel("DiceClass_Text").GetComponent<Text>().text = "MAX"; // maxtext
            }
            else
            {
                FindFromInfoPanel("DiceClass_Text").GetComponent<Text>().text = "Class " + PlayerPrefs.GetInt(DiceName + "Class");
            }
            //FindFromInfoPanel("DiceClass_Text").GetComponent<Text>().text = "Class " + PlayerPrefs.GetInt(DiceName + "Class");
            InfoPanelCode.UpgradeButtonCheck();
        }
    }
    public static bool DiceIsUpgrade(string diceName, string rarity)
    {
        bool isUpgrade = false;
        if (rarity == "Standard")
        {
            if (PlayerPrefs.GetInt(diceName + "Class") < 15 && PlayerPrefs.GetInt(diceName + "TotalCards") >= Cards.standard[PlayerPrefs.GetInt(diceName + "Class") - 1])
            {
                isUpgrade = true;
            }
        }
        else if (rarity == "Exclusive")
        {
            if (PlayerPrefs.GetInt(diceName + "Class") < 15 && PlayerPrefs.GetInt(diceName + "TotalCards") >= Cards.exclusive[PlayerPrefs.GetInt(diceName + "Class") - 3])
            {
                isUpgrade = true;
            }
        }
        else if (rarity == "Legendary")
        {
            if (PlayerPrefs.GetInt(diceName + "Class") < 15 && PlayerPrefs.GetInt(diceName + "TotalCards") >= Cards.legendary[PlayerPrefs.GetInt(diceName + "Class") - 5])
            {
                isUpgrade = true;
            }
        }
        return isUpgrade;
    }
}
