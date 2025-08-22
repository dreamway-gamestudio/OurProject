using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InfoPanel : MonoBehaviour
{
    public GameObject Info_Panel;
    GameObject Finding_GameObject;
    Image DiceImage;

    DicePlayerPrefs DicePlayerPrefs;
    Inventory Inventory;
    LockDice LockDice;
    Class Class;
    string diceName;
    GameObject Count;
    Cards Cards;
    GameObject DiceField;
    int diamondPrice;
    // Start is called before the first frame update
    void Start()
    {
        Info_Panel.SetActive(false);
        Inventory = GameObject.FindObjectOfType<Inventory>();
        Cards = GameObject.FindObjectOfType<Cards>();
        LockDice = GameObject.FindObjectOfType<LockDice>();
        Class = GameObject.FindObjectOfType<Class>();
        //DicePlayerPrefs = GameObject.FindObjectOfType<DicePlayerPrefs>();
    }
    void Update()
    {
        if (Class._isUpgarded)
        {
            UpdateCardBarInfo();
            Class._isUpgarded = false;
        }
    }

    public GameObject GetFromInfoPanel(string findingName)
    {
        for (int i = 0; i <= transform.GetChild(0).transform.GetChild(0).childCount - 1; i++)
        {
            if (transform.GetChild(0).transform.GetChild(0).GetChild(i).name == findingName)
            {
                Finding_GameObject = transform.GetChild(0).transform.GetChild(0).GetChild(i).gameObject;
                break;
            }
        }
        return Finding_GameObject;
    }


    public GameObject GetFromDiceField(string findingName)
    {
        if (DataSave.GetString("InfoPanelOpened") == "DiceOriginal")
        {
            //print("original");
            DiceField = EventSystem.current.currentSelectedGameObject.transform.parent.parent.gameObject;
            //print(DiceField.name);
            for (int i = 0; i <= DiceField.transform.childCount - 1; i++)
            {
                if (DiceField.transform.GetChild(i).name == findingName)
                {
                    Finding_GameObject = DiceField.transform.GetChild(i).gameObject;
                    break;
                }
            }
        }
        else if (DataSave.GetString("InfoPanelOpened") == "DragSlot")
        {
            DiceField = GameObject.Find(DragSlot.ParentName);
            //print(DiceField.name);
            for (int i = 0; i <= DiceField.transform.childCount - 1; i++)
            {
                if (DiceField.transform.GetChild(i).name == findingName)
                {
                    Finding_GameObject = DiceField.transform.GetChild(i).gameObject;
                    break;
                }
            }
        }
        else if (DataSave.GetString("InfoPanelOpened") == "LockedDice" || DataSave.GetString("InfoPanelOpened") == "DiceIsUpgrade")
        {
            // print("locked");
            DiceField = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
            //print(DiceField.name);
            for (int i = 0; i <= DiceField.transform.childCount - 1; i++)
            {
                if (DiceField.transform.GetChild(i).name == findingName)
                {
                    Finding_GameObject = DiceField.transform.GetChild(i).gameObject;
                    break;
                }
            }
        }
        else
        {
            //print("InfoPanel Error");
        }

        return Finding_GameObject;

    }
    public GameObject GetFromCurrentDiceField(string findingName)
    {
        for (int i = 0; i <= DiceField.transform.childCount - 1; i++)
        {
            if (DiceField.transform.GetChild(i).name == findingName)
            {
                Finding_GameObject = DiceField.transform.GetChild(i).gameObject;
                break;
            }
        }
        return Finding_GameObject;
    }
    public GameObject GetSlotChild(string SlotName)
    {
        GameObject CountSlot = GetFromInfoPanel(SlotName).gameObject;
        for (int i = 0; i <= CountSlot.transform.childCount - 1; i++)
        {
            if (CountSlot.transform.GetChild(i).name == "Count")
            {
                Count = CountSlot.transform.GetChild(i).gameObject;
                break;
            }
        }
        return Count;
    }

    void ResizeCardBar(GameObject CardBar) // popoxel clone exac cardbari chapery
    {
        CardBar.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // cardbari scale
        CardBar.GetComponent<RectTransform>().sizeDelta = new Vector2(230, 50); // cardbari chapery x,y
        CardBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(-265, 145); // cardbari position
        CardBar.transform.GetChild(1).GetComponent<Text>().fontSize = 37; // CardsText i fonti chaper
        CardBar.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(60, 70); // UpgradeArrow i chapery x,y
        CardBar.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(-100, 3); // UpgradeArrow i position
    }
    public void UpdateCardBarInfo()
    {
        if(GameObject.Find("CardBar(Clone)"))
        {
            Destroy(GameObject.Find("CardBar(Clone)"));
        }
        var CardBar = Instantiate(GetFromCurrentDiceField("PowerInfo").transform.GetChild(1), //CardBar = GetFromDiceField("PowerInfo");
        GetFromInfoPanel("DiceClass_Text").transform.position,
        Quaternion.identity
        );

        CardBar.transform.SetParent(transform.GetChild(0).transform.GetChild(0).transform);
        ResizeCardBar(CardBar.gameObject); // uxxel cardbari chapery dinamik kerpov

    }

    public void UpdateDiceInfoPanelTextes()
    {
        GetSlotChild("Attack_Slot").GetComponent<Text>().text = "" + DicePlayerPrefs.GetAttack(diceName);
        GetSlotChild("ReloadTime_Slot").GetComponent<Text>().text = "" + DicePlayerPrefs.GetReloadTime(diceName);
        GetSlotChild("ShootSpeed_Slot").GetComponent<Text>().text = "" + DicePlayerPrefs.GetShootSpeed(diceName);
    }
    public void OpenPanel()
    {
        diceName = GetFromDiceField("DiceNameText").GetComponent<Text>().text;

        if (DataSave.GetInt(diceName + "Class") > 14)
        {
            GetFromInfoPanel("DiceClass_Text").GetComponent<Text>().text = "MAX"; // maxtext
        }
        else
        {
            GetFromInfoPanel("DiceClass_Text").GetComponent<Text>().text = "Class " + DataSave.GetInt(diceName + "Class");
        }

        UpdateCardBarInfo();
        diamondPrice = DicePlayerPrefs.GetDiamondPrice(diceName);
        GetFromInfoPanel("Name_Text").GetComponent<Text>().text = GetFromDiceField("DiceNameText").GetComponent<Text>().text;
        GetFromInfoPanel("Type_Text").GetComponent<Text>().text = GetFromDiceField("TypeNameText").GetComponent<Text>().text;
        GetFromInfoPanel("Dice_Image").GetComponent<Image>().sprite = GetFromDiceField("DiceButton").GetComponent<Image>().sprite;
        GetSlotChild("DiamondButton").GetComponent<Text>().text = "" + diamondPrice;
        // dice type
        GetSlotChild("Rarity_Slot").GetComponent<Text>().text = "" + DicePlayerPrefs.GetRarity(diceName);
        GetSlotChild("Attack_Slot").GetComponent<Text>().text = "" + DicePlayerPrefs.GetAttack(diceName);
        GetSlotChild("Target_Slot").GetComponent<Text>().text = "" + DicePlayerPrefs.GetTarget(diceName);
        GetSlotChild("ReloadTime_Slot").GetComponent<Text>().text = "" + DicePlayerPrefs.GetReloadTime(diceName);
        GetSlotChild("ShootSpeed_Slot").GetComponent<Text>().text = "" + DicePlayerPrefs.GetShootSpeed(diceName);
        

        // UseButton(SetActive), UpgradeButton(position.x), UpgradeButton(SetActive), ContinueButton(SetActive),CoinButton(SetActive),DiamondButton(SetActive)
        if (GetFromDiceField("Buttons").transform.GetChild(1).GetComponent<Button>().interactable == false)
        {
            IP_ButtonsReposition(false, 0, true, false, false, false); // upgrade - erb uni dice u inventoruma
            UpgradeButtonCheck();
            print("erb uni dice u inventoruma");
        }
        else if (GetFromDiceField("Buttons").transform.GetChild(1).GetComponent<Button>().interactable == true && LockDice.DiceIsUnlocked(diceName))
        {
            IP_ButtonsReposition(true, -170f, true, false, false, false); // use - upgrade  (uni dice ev drac chi inventory)
            UpgradeButtonCheck();
            print("uni dice ev drac chi inventory");
        }
        else if (!LockDice.DiceIsUnlocked(diceName))
        {
            if (DicePlayerPrefs.GetRarity(diceName) == "Standard")
            {
                IP_ButtonsReposition(false, 0f, false, true, false, false); // continue - locked (erb paka dicey)
            }
            else if (DicePlayerPrefs.GetRarity(diceName) == "Exclusive")
            {
                IP_ButtonsReposition(false, 0f, false, false, true, false);
            }
            else if (DicePlayerPrefs.GetRarity(diceName) == "Legendary")
            {
                IP_ButtonsReposition(false, 0f, false, false, false, true);
            }

            print("erb paka dicey");
        }

        Info_Panel.SetActive(true);
        Inventory.HideButtons_DIP();
    }
    void IP_ButtonsReposition(bool isActive_UseBtn, float x_UpgradeBtn, bool isActive_UpgradeBtn, bool isActive_ContinueBtn, bool isActive_CoinBtn, bool isActive_DiamondBtn)
    {
        for (int i = 0; i <= Info_Panel.transform.GetChild(0).transform.childCount - 1; i++)
        {
            if (Info_Panel.transform.GetChild(0).transform.GetChild(i).name == "UseButton_Panel")
            {
                GameObject UseButton_Panel = Info_Panel.transform.GetChild(0).transform.GetChild(i).gameObject;
                UseButton_Panel.SetActive(isActive_UseBtn);
            }
            if (Info_Panel.transform.GetChild(0).transform.GetChild(i).name == "UpgradeButton")
            {
                GameObject UpgradeButton = Info_Panel.transform.GetChild(0).transform.GetChild(i).gameObject;
                UpgradeButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(x_UpgradeBtn, UpgradeButton.GetComponent<RectTransform>().anchoredPosition.y);
                UpgradeButton.SetActive(isActive_UpgradeBtn);
            }
            if (Info_Panel.transform.GetChild(0).transform.GetChild(i).name == "ContinueButton")
            {
                GameObject ContinueButton = Info_Panel.transform.GetChild(0).transform.GetChild(i).gameObject;
                ContinueButton.SetActive(isActive_ContinueBtn);
            }
            if (Info_Panel.transform.GetChild(0).transform.GetChild(i).name == "CoinButton")
            {
                GameObject CoinButton = Info_Panel.transform.GetChild(0).transform.GetChild(i).gameObject;
                CoinButton.SetActive(isActive_CoinBtn);
            }
            if (Info_Panel.transform.GetChild(0).transform.GetChild(i).name == "DiamondButton")
            {
                GameObject DiamondButton = Info_Panel.transform.GetChild(0).transform.GetChild(i).gameObject;
                DiamondButton.SetActive(isActive_DiamondBtn);
            }
        }
    }

    public void UpgradeButtonCheck()
    {

        if (DicePlayerPrefs.GetRarity(diceName) == "Standard")
        {
            if (DataSave.GetInt(diceName + "Class") < 15 && DataSave.GetInt(diceName + "TotalCards") >= Cards.standard[DataSave.GetInt(diceName + "Class") - 1])
            {
                GetFromInfoPanel("UpgradeButton").GetComponent<Button>().interactable = true;
            }
            else
            {
                GetFromInfoPanel("UpgradeButton").GetComponent<Button>().interactable = false;
            }
        }
        else if (DicePlayerPrefs.GetRarity(diceName) == "Exclusive")
        {
            if (DataSave.GetInt(diceName + "Class") < 15 && DataSave.GetInt(diceName + "TotalCards") >= Cards.exclusive[DataSave.GetInt(diceName + "Class") - 3])
            {
                GetFromInfoPanel("UpgradeButton").GetComponent<Button>().interactable = true;
            }
            else
            {
                GetFromInfoPanel("UpgradeButton").GetComponent<Button>().interactable = false;
            }
        }
        else if (DicePlayerPrefs.GetRarity(diceName) == "Legendary")
        {
            if (DataSave.GetInt(diceName + "Class") < 15 && DataSave.GetInt(diceName + "TotalCards") >= Cards.legendary[DataSave.GetInt(diceName + "Class") - 5])
            {
                GetFromInfoPanel("UpgradeButton").GetComponent<Button>().interactable = true;
            }
            else
            {
                GetFromInfoPanel("UpgradeButton").GetComponent<Button>().interactable = false;
            }
        }
    }
    public void UseDiceFromPanel()
    {

        print(diceName);
        GameObject thisDiceField = GameObject.Find(DataSave.GetString($"Dice_{diceName}_pos"));
        thisDiceField.GetComponent<DiceInfoPanel>().UseDice();
        ClosePanel();
    }
    public void BuyDiceWithCoin()
    {
        if (Coin.Coins >= 500)
        {
            Coin.Coins -= 500;
            DataSave.SetIntCritical($"Dice_{diceName}_isUnlocked", 1);
            ClosePanel();
            LockDice.CheckDiceBuyed();
        }
        else
        {
            print("Coin chunes ay harif");
        }
    }

    public void BuyDiceWithDiamond()
    {
        print(diamondPrice);
        if (Diamond.Diamonds >= diamondPrice)
        {
            Diamond.Diamonds -= diamondPrice;
            DataSave.SetIntCritical($"Dice_{diceName}_isUnlocked", 1);
            ClosePanel();
            LockDice.CheckDiceBuyed();
        }
        else
        {
            print($"Der petq e havaqes {diamondPrice - Diamond.Diamonds} diamond");
        }
    }
    public void ClosePanel()
    {
        Destroy(GameObject.Find("CardBar(Clone)"));
        Info_Panel.SetActive(false);
        DataSave.SetString("InfoPanelOpened", "null");
    }
}
