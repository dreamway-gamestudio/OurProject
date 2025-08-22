using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DiceInfoPanel : MonoBehaviour
{
    InfoPanel InfoPanel;
    Inventory Inventory;
    Text DiceName;
    string diceName;
    public GameObject DiceButton, DrageblleDice;
    GameObject PowerInfo, Buttons;

    GameObject f_Buttons;// fori hamar
    int DicesCount = 30; // bolor diceri qanaky fori hamar

    public static bool _isDiceUsed = false;
    GameObject Finding_GameObject;
    void Start()
    {
        DataSave.SetString("InfoPanelOpened", "DiceOriginal");
        DataSave.SetInt("upgradforinfo", 1);
        InfoPanel = GameObject.FindObjectOfType<InfoPanel>();
        Inventory = GameObject.FindObjectOfType<Inventory>();
        string[] splitArray = DrageblleDice.name.Split(char.Parse("_"));
        diceName = splitArray[1];

        for (int i = 0; i <= transform.childCount - 1; i++)
        {
            if (gameObject.transform.GetChild(i).name == "PowerInfo")
            {
                PowerInfo = gameObject.transform.GetChild(i).gameObject;
            }
            if (gameObject.transform.GetChild(i).name == "Buttons")
            {
                Buttons = gameObject.transform.GetChild(i).gameObject;
            }
        }
        HideButtons();

    }
    void Update()
    {
        if (DragSlot._onDrop)
        {
            BlackImages(false);
            DragSlot._onDrop = false;
        }
    }
    public void HideButtons()
    {
        //print("HideButtons"); // mtnuma es funkcia 
        PowerInfo.SetActive(true);
        Buttons.SetActive(false);
        if (DrageblleDice.transform.parent.name.Substring(0, 4) != "Item") // ete item i mej chi tox dragable paki
        {
            DrageblleDice.GetComponent<Image>().enabled = false;
            DrageblleDice.transform.GetChild(0).GetComponent<Image>().enabled = false; // Drag exnox dice i doti image y bacel
            DiceButton.GetComponent<Image>().raycastTarget = true;
            Buttons.transform.GetChild(1).GetComponent<Button>().interactable = true;
        }


    }
    public void ChooseDice()
    {

		DataSave.SetString("InfoPanelOpened", "DiceOriginal");
		if (LockDice.DiceIsUnlocked(diceName) && !Class.DiceIsUpgrade(diceName, DataSave.GetString(diceName + "Rarity", "")))
        {
            BlackImages(false);
            HidePower();
            PowerInfo.SetActive(false);
            Buttons.SetActive(true);
            _isDiceUsed = false;
            InfoButton_Interactible(true);
        }
        else if (Class.DiceIsUpgrade(diceName, DataSave.GetString(diceName + "Rarity")) && LockDice.DiceIsUnlocked(diceName))
        {
            if (_isDiceUsed)
            {
                Inventory.HideButtons_DIP();
            }
            else
            {
			DataSave.SetString("InfoPanelOpened", "DiceIsUpgrade");
                OpenInfoPanel();
            }
		} else if (!LockDice.DiceIsUnlocked(diceName))
        {
            if (_isDiceUsed)
            {
                Inventory.HideButtons_DIP();
            }
            else
            {
				DataSave.SetString("InfoPanelOpened", "LockedDice");
                OpenInfoPanel();
            }
        }

    }
    void OpenInfoPanel()
    {

        InfoPanel.OpenPanel();
    }

    public void BlackImages(bool isActive)
    {
        for (int j = 1; j <= DicesCount; j++)
        {
            string name = $"DiceField_{j}";
            GameObject DiceField = GameObject.Find(name);
            //print(DiceField.name);
            for (int k = 0; k <= DiceField.transform.childCount - 1; k++)
            {

                if (DiceField.transform.GetChild(k).name == "BlackCloseImage")
                {
                    GameObject BlackImage = DiceField.transform.GetChild(k).gameObject;
                    //print("Gtav: " + BlackImage.name);
                    BlackImage.GetComponent<Image>().enabled = isActive;
                    break;
                }

            }
        }
    }
    public void HidePower()
    {
        GameObject ActiveDice = GameObject.Find(DataSave.GetString("ActiveDice"));
        for (int i = 1; i <= DicesCount; i++)
        {
            string name = $"DiceField_{i}";
            GameObject DiceField = GameObject.Find(name);

            //            print(DiceField.name);
            for (int j = 0; j <= DiceField.transform.childCount - 1; j++)
            {
                if (DiceField.transform.GetChild(j).name == "Buttons")
                {
                    f_Buttons = DiceField.transform.GetChild(j).gameObject;
                }

            }
            if (f_Buttons.activeSelf == true)
            {
                DiceField.GetComponent<DiceInfoPanel>().HideButtons();
                //print(f_Buttons.name + " is active");
            }
            try
            {
                ActiveDice.transform.parent.GetComponent<DiceInfoPanel>().HideButtons();
            }
            catch { }
        }
    }


    public void UseDice()
    {

        _isDiceUsed = true;
        BlackImages(true); // bolor black image nery pakuma
        for (int i = 0; i <= transform.childCount - 1; i++) // menak ira black image bacuma
        {
            if (transform.GetChild(i).name == "BlackCloseImage")
            {
                GameObject BlackImage = transform.GetChild(i).gameObject;
                BlackImage.GetComponent<Image>().enabled = false;

                break;
            }
        }
        for (int i = 0; i <= gameObject.transform.childCount - 1; i++)
        {
            try
            {
                if (transform.GetChild(i).gameObject.name.Substring(0, 5) == "Dice_")
                {
                    GameObject Dice = transform.GetChild(i).gameObject;
                    DataSave.SetString("ActiveDice", Dice.name);
                    break;
                }
                else
                {
                    continue;
                }
            }
            catch { }
        }
        InfoButton_Interactible(false);
        DrageblleDice.GetComponent<Image>().enabled = true;
        DrageblleDice.transform.GetChild(0).GetComponent<Image>().enabled = true; // Drag exnox dice i doti image y pakel
        DiceButton.GetComponent<Image>().raycastTarget = false;
        Buttons.transform.GetChild(1).GetComponent<Button>().interactable = false;
    }
    void InfoButton_Interactible(bool isActive)
    {
        for (int i = 0; i <= gameObject.transform.childCount - 1; i++)
        {
            if (transform.GetChild(i).gameObject.name == "Buttons")
            {
                GameObject Buttons = transform.GetChild(i).gameObject;
                for (int j = 0; j <= Buttons.transform.childCount - 1; j++)
                {
                    if (Buttons.transform.GetChild(j).gameObject.name == "InfoButton")
                    {
                        GameObject InfoButton = Buttons.transform.GetChild(j).gameObject;
                        InfoButton.GetComponent<Button>().interactable = isActive;
                    }
                }
            }
        }
    }
}
