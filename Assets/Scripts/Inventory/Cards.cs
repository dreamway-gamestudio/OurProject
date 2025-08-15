using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cards : MonoBehaviour
{
    public static int[] standard = { 2, 4, 7, 12, 50, 80, 150, 350, 700, 1500, 3200, 7000, 9500, 12000, 15000 };
    public static int[] exclusive = { 3, 5, 10, 15, 80, 300, 800, 1000, 2000, 3000, 4000, 5000 };
    public static int[] legendary = { 4, 7, 12, 50, 100, 150, 175, 200, 225, 250 };
    public static string[] St_Names = { "Dog", "Cat", "Rabbit", "Pig", "Fox", "Squirrel", "Snake", "Guineapig", "Hedgehog", "Turtle" };
    public static string[] Ex_Names = { "Wolf", "Monkey", "Kangaroo", "Koala", "Raccoon", "Horse", "Deer", "Zebra", "Giraffe", "Ostrich" };
    public static string[] Lg_Names = { "Bear", "Panda", "Lion", "Tiger", "Panther", "Hippopotamus", "Elephant", "Rhinoceros", "Bull", "Eagle" };
    public int random_dice, giftcards;
    int maxClass = 15;
    int all_standard, all_exclusive, all_legendary;
    Scrollbar CardBar;
    string diceName;
    DicePlayerPrefs DicePlayerPrefs;
    Inventory Inventory;
    LockDice LockDice;
    Color color;
    bool cardsCheck;
    GameObject Handle, UpgradeArrow;
    public Sprite HandleLoading, HandleDone;
    void Start()
    {
        
        AllCardCheck();
        DicePlayerPrefs = GameObject.FindObjectOfType<DicePlayerPrefs>();
        Inventory = GameObject.FindObjectOfType<Inventory>();
        LockDice = GameObject.FindObjectOfType<LockDice>();
    }
    public void GetRandomData(string rarity)
    {

        if (rarity == "standard")
        {
            random_dice = Random.Range(0, St_Names.Length);
        }
        else if (rarity == "exclusive")
        {
            random_dice = Random.Range(0, Ex_Names.Length);
        }
        else if (rarity == "legendary")
        {
            random_dice = Random.Range(0, Lg_Names.Length);
        }
    }

    public void GiveRandomCards(int min, int max, string rarity)
    {
        giftcards = Random.Range(min, max);
        if (rarity == "standard")
        {
            PlayerPrefs.SetInt(St_Names[random_dice] + "TotalCards", PlayerPrefs.GetInt(St_Names[random_dice] + "TotalCards") + giftcards);
            PlayerPrefs.SetInt($"Dice_{St_Names[random_dice]}_isUnlocked", 1);
            //print($"Dice {St_Names[random_dice]} gets {giftcards} cards");
        }
        else if (rarity == "exclusive")
        {
            PlayerPrefs.SetInt(Ex_Names[random_dice] + "TotalCards", PlayerPrefs.GetInt(Ex_Names[random_dice] + "TotalCards") + giftcards);
            //PlayerPrefs.SetInt($"Dice_{Ex_Names[random_dice]}_isUnlocked", 1);
            //print($"Dice {Ex_Names[random_dice]} gets {giftcards} cards");
        }
        else if (rarity == "legendary")
        {
            PlayerPrefs.SetInt(Lg_Names[random_dice] + "TotalCards", PlayerPrefs.GetInt(Lg_Names[random_dice] + "TotalCards") + giftcards);
            //PlayerPrefs.SetInt($"Dice_{Lg_Names[random_dice]}_isUnlocked", 1);
            //print($"Dice {Lg_Names[random_dice]} gets {giftcards} cards");
        }
        LockDice.CheckDiceBuyed();
        CardsInit();
    }
    public void TestCards(string button_name)
    {
        if (button_name == "S")
        {
            //GiveRandomCards(5, 10, "standard");
            CardsInit();
        }
        else if (button_name == "E")
        {
            //GiveRandomCards(10, 20, "exclusive");
            CardsInit();
        }
        else if (button_name == "L")
        {
            //GiveRandomCards(1, 5, "legendary");
            CardsInit();
        }
    }
    void AllCardCheck()
    {
        all_standard = 0;
        for (int i = 0; i <= standard.Length - 1; i++)
        {
            all_standard += standard[i];
        }
        PlayerPrefs.SetInt("all_standard", all_standard);

        all_exclusive = 0;
        for (int i = 0; i <= exclusive.Length - 1; i++)
        {
            all_exclusive += exclusive[i];
        }
        PlayerPrefs.SetInt("all_exclusive", all_exclusive);

        all_exclusive = 0;
        for (int i = 0; i <= legendary.Length - 1; i++)
        {
            all_legendary += legendary[i];
        }
        PlayerPrefs.SetInt("all_legendary", all_legendary);
    }
    public void CardsInit()
    {
        cardsCheck = false;
        GameObject Content = transform.GetChild(0).gameObject; // mtanq Content
        for (int i = 0; i <= Content.transform.childCount - 1; i++)
        {
            GameObject DiceField = Content.transform.GetChild(i).gameObject;

            for (int j = 0; j <= DiceField.transform.childCount - 1; j++) // mtanq DiceField
            {
                if (DiceField.transform.GetChild(j).name == "DiceNameText") // *mtanq DiceNameText
                {
                    GameObject DiceName = DiceField.transform.GetChild(j).gameObject;
                    diceName = DiceName.GetComponent<Text>().text;
                }
                if (DiceField.transform.GetChild(j).name == "PowerInfo")
                {
                    GameObject PowerInfo = DiceField.transform.GetChild(j).gameObject; // mtanq PowerInfo
                    for (int k = 0; k <= PowerInfo.transform.childCount - 1; k++)
                    {
                        if (PowerInfo.transform.GetChild(k).name == "ClassText")
                        {
                            GameObject ClassText = PowerInfo.transform.GetChild(k).gameObject; // *mtanq ClassText

                            ClassText.GetComponent<Text>().text = "Class " + PlayerPrefs.GetInt(diceName + "Class");

                        }
                        if (PowerInfo.transform.GetChild(k).name == "CardBar")
                        {
                            GameObject CardBar = PowerInfo.transform.GetChild(k).gameObject;
                            for (int m = 0; m <= CardBar.transform.childCount - 1; m++) // mtanq CardBar
                            {
                                if (CardBar.transform.GetChild(m).name == "SlidingArea")
                                {
                                    GameObject SlidingArea = CardBar.transform.GetChild(m).gameObject;
                                    for (int g = 0; g <= SlidingArea.transform.childCount - 1; g++)
                                    {
                                        if (SlidingArea.transform.GetChild(g).name == "Handle")
                                        {
                                            Handle = SlidingArea.transform.GetChild(g).gameObject;
                                        }
                                    }
                                }
                                if (CardBar.transform.GetChild(m).name == "CardsText")
                                {
                                    GameObject CardsText = CardBar.transform.GetChild(m).gameObject;
                                    int totalCards = PlayerPrefs.GetInt(diceName + "TotalCards");

                                    int thisClass = PlayerPrefs.GetInt(diceName + "Class");
                                    if (thisClass > 0 && thisClass < 15)
                                    {
                                        if (PlayerPrefs.GetString(diceName + "Rarity") == "Standard")
                                        {
                                            CardsText.GetComponent<Text>().text = $"{totalCards.ToString()}/{standard[thisClass - 1]}";
                                            float percent = (float)totalCards / (float)standard[thisClass - 1];
                                            //CardBar.GetComponent<Scrollbar>().size = percent;
                                            Handle.GetComponent<Image>().fillAmount = percent;
                                            if (Class.DiceIsUpgrade(diceName, PlayerPrefs.GetString(diceName + "Rarity")))
                                            {
                                                //ChangeHandleColor("#65FF3D", Handle);
                                                Handle.GetComponent<Image>().sprite = HandleDone;
                                            }
                                            else
                                            {
                                                //ChangeHandleColor("#64C9FF", Handle);
                                                Handle.GetComponent<Image>().sprite = HandleLoading;
                                            }
                                        }
                                        else if (PlayerPrefs.GetString(diceName + "Rarity") == "Exclusive")
                                        {
                                            CardsText.GetComponent<Text>().text = $"{totalCards.ToString()}/{exclusive[thisClass - 3]}";
                                            float percent = (float)totalCards / (float)exclusive[thisClass - 3];
                                            //CardBar.GetComponent<Scrollbar>().size = percent;
                                            Handle.GetComponent<Image>().fillAmount = percent;
                                            if (Class.DiceIsUpgrade(diceName, PlayerPrefs.GetString(diceName + "Rarity")))
                                            {
                                                //ChangeHandleColor("#65FF3D", Handle);
                                                Handle.GetComponent<Image>().sprite = HandleDone;
                                            }
                                            else
                                            {
                                                //ChangeHandleColor("#64C9FF", Handle);
                                                Handle.GetComponent<Image>().sprite = HandleLoading;
                                            }
                                        }
                                        else if (PlayerPrefs.GetString(diceName + "Rarity") == "Legendary")
                                        {
                                            CardsText.GetComponent<Text>().text = $"{totalCards.ToString()}/{legendary[thisClass - 5]}";
                                            float percent = (float)totalCards / (float)legendary[thisClass - 5];
                                            //CardBar.GetComponent<Scrollbar>().size = percent;
                                            Handle.GetComponent<Image>().fillAmount = percent;
                                            if (Class.DiceIsUpgrade(diceName, PlayerPrefs.GetString(diceName + "Rarity")))
                                            {
                                                //ChangeHandleColor("#65FF3D", Handle);
                                                Handle.GetComponent<Image>().sprite = HandleDone;
                                            }
                                            else
                                            {
                                                //ChangeHandleColor("#64C9FF", Handle);
                                                Handle.GetComponent<Image>().sprite = HandleLoading;
                                            }
                                        }

                                    }
                                    else if (thisClass > 14) //maxtext
                                    {
                                        CardsText.GetComponent<Text>().text = "MAX";
                                        Handle.GetComponent<Image>().enabled = false;
                                    }
                                }
                                if (CardBar.transform.GetChild(m).name == "UpgradeArrow")
                                {
                                    UpgradeArrow = CardBar.transform.GetChild(m).gameObject;
                                    if (PlayerPrefs.GetString(diceName + "Rarity") == "Standard")
                                    {
                                        if (Class.DiceIsUpgrade(diceName, PlayerPrefs.GetString(diceName + "Rarity")))
                                        {
                                            UpgradeArrow.GetComponent<Image>().enabled = true;
                                        }
                                        else
                                        {
                                            UpgradeArrow.GetComponent<Image>().enabled = false;
                                        }
                                    }
                                    else if (PlayerPrefs.GetString(diceName + "Rarity") == "Exclusive")
                                    {
                                        if (Class.DiceIsUpgrade(diceName, PlayerPrefs.GetString(diceName + "Rarity")))
                                        {
                                            UpgradeArrow.GetComponent<Image>().enabled = true;
                                        }
                                        else
                                        {
                                            UpgradeArrow.GetComponent<Image>().enabled = false;
                                        }
                                    }
                                    else if (PlayerPrefs.GetString(diceName + "Rarity") == "Legendary")
                                    {
                                        if (Class.DiceIsUpgrade(diceName, PlayerPrefs.GetString(diceName + "Rarity")))
                                        {
                                            UpgradeArrow.GetComponent<Image>().enabled = true;
                                        }
                                        else
                                        {
                                            UpgradeArrow.GetComponent<Image>().enabled = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        try
        {
            Inventory.InventoryClass();
        }
        catch { }
    }
    void ChangeHandleColor(string hex, GameObject Handle)
    {
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            Handle.GetComponent<Image>().color = color;
        }
    }
}
