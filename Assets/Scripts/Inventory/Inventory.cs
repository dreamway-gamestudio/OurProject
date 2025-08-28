using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    DiceInfoPanel DiceInfoPanel;
    Cards Cards;
    DragDice DragDice;
    int totalDices = 30;
    GameObject this_Dice;
    [HideInInspector] public bool InIt = false;
    void Start()
    {
        Cards = GameObject.FindObjectOfType<Cards>();
    }
    public void InventoryClass()
    {
        for (int i = 0; i <= transform.childCount - 1; i++)
        {
            if (transform.GetChild(i).name == "ItemPanel")
            {
                GameObject ItemPanel = transform.GetChild(i).gameObject;
                for (int j = 1; j <= 5; j++)
                {
                    string finding = $"ClassText_{j}";
                    GameObject ClassText = GameObject.Find(finding);

                    string finding_item = $"Item{j}";
                    GameObject Item = GameObject.Find(finding_item);

                    string diceName = Item.transform.GetChild(0).name;
                    string[] splitArray = diceName.Split(char.Parse("_"));

                    if (DataSave.GetInt(splitArray[1] + "Class") > 14)
                    {
                        ClassText.GetComponent<Text>().text = "MAX"; //maxtext
                    }
                    else
                    {
                        ClassText.GetComponent<Text>().text = "Class " + DataSave.GetInt(splitArray[1] + "Class");
                    }
                }
            }
        }
    }
    public void HideButtons_DIP()
    {
        DiceInfoPanel._isDiceUsed = false;
        for (int i = 1; i <= totalDices; i++)
        {
            string find = $"DiceField_{i}";
            GameObject Vichabanutyun = GameObject.Find(find);
            Vichabanutyun.GetComponent<DiceInfoPanel>().HideButtons();
        }
        for (int j = 1; j <= totalDices; j++)
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
                    BlackImage.GetComponent<Image>().enabled = false;
                    break;
                }

            }
        }
    }
    public void OnlyOneTime() // mi angam skzbum ashxatox funkcia
    {
        InIt = DataSave.GetInt("InIt") == 1 ? true : false;
        if (!InIt)
        {
            Elixir.SetElixir(5); // poxel
            DataSave.SetInt("Coins", 0);
            DataSave.SetInt("Diamonds", 0);
            for (int j = 1; j <= totalDices; j++) // sa diceri parentnerin pahpanelu hamara, vor heto karenanq dragi vaxt het qcenq ira texy
            {
                string name = $"DiceField_{j}"; // dice i parenti anunna, vory kpoxvi amen cikli jamanak (1...5)
                                                // = GameObject.Find(name).transform.GetChild(GameObject.Find(name).transform.childCount - 1).gameObject; // DiceFieldi verji childy stanuma aysinqn tvyal dicey
                GameObject DiceField = GameObject.Find(name);
                for (int k = 0; k <= DiceField.transform.childCount - 1; k++)
                {
                    try
                    {
                        if (DiceField.transform.GetChild(k).name.Substring(0, 5) == "Dice_")
                        {
                            this_Dice = DiceField.transform.GetChild(k).gameObject;
                            DataSave.SetString($"{this_Dice.name}_pos", this_Dice.transform.parent.name);
                            //print(this_Dice.name);
                        }
                    }
                    catch { }
                }

                //GameObject.Find(name).GetComponent<DiceInfoPanel>().UseDice(); // usa anum myus kodic vor dicy darna true


            }
            for (int i = 1; i <= 5; i++) // skzbi 5 haty amenaskzbum pahuma inventori mej
            {

                string name = $"DiceField_{i}"; // dice i parenti anunna, vory kpoxvi amen cikli jamanak (1...5)
                string firstParents = $"Item{i}"; // Inventori itemneri anunnery, vor karenanq arajin 5 dicerin texavorenq sranc mej
                //GameObject this_Dice = GameObject.Find(name).transform.GetChild(GameObject.Find(name).transform.childCount - 1).gameObject; // DiceFieldi verji childy stanuma aysinqn tvyal dicey
                //GameObject.Find(name).GetComponent<DiceInfoPanel>().UseDice(); // usa anum myus kodic vor dicy darna true
                GameObject DiceField = GameObject.Find(name);
                for (int k = 0; k <= DiceField.transform.childCount - 1; k++)
                {
                    try
                    {
                        if (DiceField.transform.GetChild(k).name.Substring(0, 5) == "Dice_")
                        {
                            this_Dice = DiceField.transform.GetChild(k).gameObject;
                            DataSave.SetString($"{this_Dice.name}_pos", this_Dice.transform.parent.name);
                            //print(this_Dice.name);
                        }
                    }
                    catch { }
                }
                GameObject this_Item = GameObject.Find(firstParents); // gtnuma hamapatasxan item verevi anunov vor hamapatasxan dicy texavori sra mej
                DataSave.SetString($"Dice{i}", this_Dice.name);
                //DataSave.SetString($"Dice{i}_pos", this_Dice.transform.parent.name);
                this_Dice.transform.SetParent(this_Item.transform); // dicy texavoruma itemi mej
                this_Dice.transform.GetComponent<Image>().raycastTarget = false; // vor sa chem grum verevum irar mej karum es poxes
                this_Dice.transform.localPosition = Vector3.zero; // dicy dirqy zroyacnuma ira parenti dirqin` voncor reset
            }
            for (int m = 1; m <= totalDices; m++) // diceri isBuyed
            {
                string name = $"DiceField_{m}"; // dice i parenti anunna, vory kpoxvi amen cikli jamanak (1...5)
                GameObject DiceField = GameObject.Find(name);

                for (int k = 0; k <= DiceField.transform.childCount - 1; k++)
                {
                    if (DiceField.transform.GetChild(k).name == "DiceNameText")
                    {
                        GameObject DiceText = DiceField.transform.GetChild(k).gameObject;
                        string this_name = DiceText.GetComponent<Text>().text;
                        if (m > 0 && m < 6)
                        {
                            DataSave.SetInt($"Dice_{this_name}_isUnlocked", 1);
                        }
                        else if (m > 5)
                        {

                            DataSave.SetInt($"Dice_{this_name}_isUnlocked", 0);
                        }
                    }
                }
            }
        }
    }
    public void Init() // sa amen startum ktexavori dicery inventorum
    {
        for (int i = 1; i <= 5; i++)
        {
            string diceName = DataSave.GetString($"Dice{i}"); // "Dice_Black" naprimer
            GameObject Dice = GameObject.Find(diceName);
            Dice.GetComponent<Image>().enabled = true;
            Dice.transform.SetParent(GameObject.Find($"Item{i}").transform);
            Dice.transform.GetComponent<Image>().raycastTarget = false; // vor sa chem grum verevum irar mej karum es poxes
            Dice.transform.localPosition = Vector3.zero; // dicy dirqy zroyacnuma ira parenti dirqin` voncor reset
            Dice.GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 200f);
            Dice.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector2(0f, -30f);
            Dice.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 20f);
            InIt = true;
            DataSave.SetInt("InIt", (InIt ? 1 : 0));
        }

    }
    public void CheckInventory() // uzum em stanam listi mej bolor dicery inventori` *chisht hertakanutyamb*
    {
        for (int i = 1; i <= 5; i++)
        {

            string name = $"Item{i}";
            GameObject Dice = GameObject.Find(name);
            //print($"The {name} has {Dice.transform.childCount} child");
        }
    }
}

