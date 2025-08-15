using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Chests : MonoBehaviour
{
    Cards Cards;
    LockDice LockDice;
    ChestContent ChestContent;
    TimerChest TimerChest;
    public GameObject ChestPanel, ChestInfoPanel;
    GameObject Finding_GameObject, Chest, Cell;
    int MinCards, MaxCards;
    string[] Rarity = { "standard", "exclusive", "legendary" };
    List<string> Rewards = new List<string>();
    public Image[] DiceImages;
    public Text[] CardCountTexts;

    public GameObject[] ChestCards;
    public GameObject CoinCard, DiamondCard;
    public Image[] NewImages;
    public Text[] RandomTexts, DiceNameTexts;
    public int RewardCount;
    public Sprite[] ChestImages;
    public GameObject OpenButton;
    public Text RewardCoin, RewardDiamond;
    string cardName, rarity;
    Color color;
    string chestType;
    bool _isBuyedWithDiamond = false;
    [HideInInspector] public bool InIt_Chest = false;
    bool _isStandard, _isExclusive, _isLegendary, _isRandom;
    List<bool> CardsCount = new List<bool>();
    int diamondChestPrice;
    int remainingTime, newChestPrice;
    string[] chests = { "Level_Chest", "Star_Chest", "Card_Chest", "Standard_Chest", "Exclusive_Chest", "Legendary_Chest" };
    bool isChestOpenFromDaily = false;

    /// GetComponents
    private GameObject DiamondText, TimerText;

    void Start()
    {
        Cards = GameObject.FindObjectOfType<Cards>();
        LockDice = GameObject.FindObjectOfType<LockDice>();
        ChestContent = GameObject.FindObjectOfType<ChestContent>();
        TimerChest = GameObject.FindObjectOfType<TimerChest>();
        ChestPanel.SetActive(false);
        ChestInfoPanel.SetActive(false);
    }
    private void OnCollisionEnter(Collision other)
    {
        // che axper chka nayi
    }
    public void SaveChestsDatas()
    {
        InIt_Chest = PlayerPrefs.GetInt("InIt_Chest") == 1 ? true : false;
        if (!InIt_Chest)
        {
            for (int j = 0; j <= chests.Length - 1; j++)
            {
                for (int i = 0; i <= Cards.St_Names.Length - 1; i++)
                {
                    PlayerPrefs.SetInt($"ChestClass{Cards.St_Names[i]}", PlayerPrefs.GetInt($"{Cards.St_Names[i]}Class"));

                    int mincardsStandard = GameObject.Find(chests[j]).GetComponent<ChestContent>().StandardCardsCount();
                    string nameToPlPf_s = $"{chests[j]}_standard_{Cards.St_Names[i]}";
                    PlayerPrefs.SetInt(nameToPlPf_s, mincardsStandard);

                    int mincardsRandom = GameObject.Find(chests[j]).GetComponent<ChestContent>().RandomCardsCount();
                    string nameToPlPf_r = $"{chests[j]}_random_{Cards.St_Names[i]}";
                    PlayerPrefs.SetInt(nameToPlPf_r, mincardsRandom);
                }

                for (int i = 0; i <= Cards.Ex_Names.Length - 1; i++)
                {
                    PlayerPrefs.SetInt($"ChestClass{Cards.Ex_Names[i]}", PlayerPrefs.GetInt($"{Cards.Ex_Names[i]}Class"));

                    int mincardsExclusive = GameObject.Find(chests[j]).GetComponent<ChestContent>().ExclusiveCardsCount();
                    string nameToPlPf_e = $"{chests[j]}_exclusive_{Cards.Ex_Names[i]}";
                    PlayerPrefs.SetInt(nameToPlPf_e, mincardsExclusive);

                    int mincardsRandom = GameObject.Find(chests[j]).GetComponent<ChestContent>().RandomCardsCount();
                    string nameToPlPf_r = $"{chests[j]}_random_{Cards.Ex_Names[i]}";
                    PlayerPrefs.SetInt(nameToPlPf_r, mincardsRandom);
                }
                for (int i = 0; i <= Cards.Lg_Names.Length - 1; i++)
                {
                    PlayerPrefs.SetInt($"ChestClass{Cards.Lg_Names[i]}", PlayerPrefs.GetInt($"{Cards.Lg_Names[i]}Class"));

                    int mincardsLegendary = GameObject.Find(chests[j]).GetComponent<ChestContent>().LegendaryCardsCount();
                    string nameToPlPf_l = $"{chests[j]}_legendary_{Cards.Lg_Names[i]}";
                    PlayerPrefs.SetInt(nameToPlPf_l, mincardsLegendary);

                    int mincardsRandom = GameObject.Find(chests[j]).GetComponent<ChestContent>().RandomCardsCount();
                    string nameToPlPf_r = $"{chests[j]}_random_{Cards.Lg_Names[i]}";
                    PlayerPrefs.SetInt(nameToPlPf_r, mincardsRandom);
                }
            }
            InIt_Chest = true;
            PlayerPrefs.SetInt("InIt_Chest", (InIt_Chest ? 1 : 0));
        }
    }

    public void OpenChestFromDaily(string ChestType)
    {

        if (RandomTexts[0].enabled)
            RandomTexts[0].enabled = false;

        if (RandomTexts[1].enabled)
            RandomTexts[1].enabled = false;

        chestType = ChestType;
        GetRarityBool();

        if (chestType != "Level_Chest" && chestType != "Star_Chest" && chestType != "Card_Chest")
        {
            isChestOpenFromDaily = true;
            if (ChestType == "Standard_Chest") // ischestdone-eri harcy kisata
            {
                ChestPanel.SetActive(true); // aktivacnuma chestpanely
                GameObject ChestCloseImage = GameObject.Find("ChestPanel/ChestCloseImage"); // gtnuma chesti pak image y 
                ChestCloseImage.GetComponent<Image>().sprite = ChestImages[0]; // dnuma chesti pak image henc irany chestimages massivic
                GameObject ChestOpenImage = GameObject.Find("ChestPanel/ChestOpenImage"); // gtnuma chesti bac image y 
                ChestOpenImage.GetComponent<Image>().sprite = ChestImages[7]; // dnuma chesti bac image henc irany chestimages massivic
            }
            else if (ChestType == "Exclusive_Chest") // ischestdone-eri harcy kisata
            {
                ChestPanel.SetActive(true);
                GameObject ChestCloseImage = GameObject.Find("ChestPanel/ChestCloseImage");
                ChestCloseImage.GetComponent<Image>().sprite = ChestImages[1];
                GameObject ChestOpenImage = GameObject.Find("ChestPanel/ChestOpenImage");
                ChestOpenImage.GetComponent<Image>().sprite = ChestImages[8];
            }
            else if (ChestType == "Legendary_Chest") // ischestdone-eri harcy kisata
            {
                ChestPanel.SetActive(true);
                GameObject ChestCloseImage = GameObject.Find("ChestPanel/ChestCloseImage");
                ChestCloseImage.GetComponent<Image>().sprite = ChestImages[2];
                GameObject ChestOpenImage = GameObject.Find("ChestPanel/ChestOpenImage");
                ChestOpenImage.GetComponent<Image>().sprite = ChestImages[9];
            }
        }
    }
    public void OpenChestPanel(string ChestType) // avelacnel naxnakan info panel qani der chesty paka (if/else)
    {
        OpenButton.SetActive(true);
        if (RandomTexts[0].enabled)
            RandomTexts[0].enabled = false;

        if (RandomTexts[1].enabled)
            RandomTexts[1].enabled = false;

        chestType = ChestType;
        GetRarityBool();

        if (chestType != "Level_Chest" && chestType != "Star_Chest" && chestType != "Card_Chest")
        {
            isChestOpenFromDaily = false;
            if (TimerChest.CurrentChest.GetComponent<TimerChest>().isEnd && ChestType == "Standard_Chest") // ischestdone-eri harcy kisata
            {
                print("mtav standard");
                ChestPanel.SetActive(true); // aktivacnuma chestpanely
                GameObject ChestCloseImage = GameObject.Find("ChestPanel/ChestCloseImage"); // gtnuma chesti pak image y 
                ChestCloseImage.GetComponent<Image>().sprite = ChestImages[0]; // dnuma chesti pak image henc irany chestimages massivic
                GameObject ChestOpenImage = GameObject.Find("ChestPanel/ChestOpenImage"); // gtnuma chesti bac image y 
                ChestOpenImage.GetComponent<Image>().sprite = ChestImages[7]; // dnuma chesti bac image henc irany chestimages massivic
            }
            else if (TimerChest.CurrentChest.GetComponent<TimerChest>().isEnd && ChestType == "Exclusive_Chest") // ischestdone-eri harcy kisata
            {
                ChestPanel.SetActive(true);
                GameObject ChestCloseImage = GameObject.Find("ChestPanel/ChestCloseImage");
                ChestCloseImage.GetComponent<Image>().sprite = ChestImages[1];
                GameObject ChestOpenImage = GameObject.Find("ChestPanel/ChestOpenImage");
                ChestOpenImage.GetComponent<Image>().sprite = ChestImages[8];
            }
            else if (TimerChest.CurrentChest.GetComponent<TimerChest>().isEnd && ChestType == "Legendary_Chest") // ischestdone-eri harcy kisata
            {
                ChestPanel.SetActive(true);
                GameObject ChestCloseImage = GameObject.Find("ChestPanel/ChestCloseImage");
                ChestCloseImage.GetComponent<Image>().sprite = ChestImages[2];
                GameObject ChestOpenImage = GameObject.Find("ChestPanel/ChestOpenImage");
                ChestOpenImage.GetComponent<Image>().sprite = ChestImages[9];
            }
            else
            {
                ChestInfoPanel.SetActive(true);
                GameObject DiamondText = GameObject.Find("ChestInfoPanel/Panel/BuyButton/DiamondText");
                DiamondText.GetComponent<Text>().text = GetCurrentChestPrice().ToString();
                // 500... 3600... 2000... 1600... 
                GetChestContent();
            }
        }
        else if (chestType == "Level_Chest" || chestType == "Star_Chest" || chestType == "Card_Chest")// ete current chest chi 
        {
            if (LevelChest._isLevelChestDone && ChestType == "Level_Chest")
            {
                ChestPanel.SetActive(true);
                GameObject ChestCloseImage = GameObject.Find("ChestPanel/ChestCloseImage");
                ChestCloseImage.GetComponent<Image>().sprite = ChestImages[3];
                GameObject ChestOpenImage = GameObject.Find("ChestPanel/ChestOpenImage");
                ChestOpenImage.GetComponent<Image>().sprite = ChestImages[10];
            }
            else if (StarChest._isStarChestDone && ChestType == "Star_Chest")
            {
                ChestPanel.SetActive(true);
                GameObject ChestCloseImage = GameObject.Find("ChestPanel/ChestCloseImage");
                ChestCloseImage.GetComponent<Image>().sprite = ChestImages[4];
                GameObject ChestOpenImage = GameObject.Find("ChestPanel/ChestOpenImage");
                ChestOpenImage.GetComponent<Image>().sprite = ChestImages[11];
            }
            else if (CardChest._isCardChestDone && ChestType == "Card_Chest")
            {
                ChestPanel.SetActive(true);
                GameObject ChestCloseImage = GameObject.Find("ChestPanel/ChestCloseImage");
                ChestCloseImage.GetComponent<Image>().sprite = ChestImages[5];
                GameObject ChestOpenImage = GameObject.Find("ChestPanel/ChestOpenImage");
                ChestOpenImage.GetComponent<Image>().sprite = ChestImages[12];
            }
            else
            {
                ChestInfoPanel.SetActive(true);
                GameObject DiamondText = GameObject.Find("ChestInfoPanel/Panel/BuyButton/DiamondText");
                DiamondText.GetComponent<Text>().text = diamondChestPrice.ToString();
                GetChestContent();
            }
        }

    }
    int GetCurrentChestPrice()
    {
        if (TimerChest.CurrentChest.GetComponent<TimerChest>().zapusk)
        {
            diamondChestPrice = GameObject.Find(chestType).GetComponent<ChestContent>().diamondPrice; //500
            int initialTime = Mathf.RoundToInt(TimerChest.CurrentChest.GetComponent<TimerChest>().restoreDuration * 3600); // 3600
            remainingTime = TimerChest.CurrentChest.GetComponent<TimerChest>().remainingTime; // 3592
            int oneDimaondtoSecondPrice = Mathf.RoundToInt(initialTime / diamondChestPrice); // 7
            int mnacord = initialTime - remainingTime; // 8
            int pakasord = mnacord / oneDimaondtoSecondPrice; // 1
            newChestPrice = diamondChestPrice - pakasord;

        }
        else
        {
            newChestPrice = GameObject.Find(chestType).GetComponent<ChestContent>().diamondPrice;
        }
        return newChestPrice;


    }
    void GetRarityBool()
    {
        _isStandard = GameObject.Find(chestType).GetComponent<ChestContent>().isStandardCard();
        _isExclusive = GameObject.Find(chestType).GetComponent<ChestContent>().isExclusiveCard();
        _isLegendary = GameObject.Find(chestType).GetComponent<ChestContent>().isLegendaryCard();
        _isRandom = GameObject.Find(chestType).GetComponent<ChestContent>().isRandomCard();
        CardsCount.Add(_isStandard);
        CardsCount.Add(_isExclusive);
        CardsCount.Add(_isLegendary);
    }

    public void BuyChestWithDiamond()
    {

        int chestPrice = GetCurrentChestPrice();
        if (Diamond.Diamonds >= chestPrice)
        {
            if (TimerChest.CurrentChest.GetComponent<TimerChest>().zapusk)
            {
                PlayerPrefs.SetInt("TimerStatus", 0);
            }
            PlayerPrefs.SetInt("BuyChestWithDiamond", 0);
            Diamond.Diamonds -= chestPrice;
            _isBuyedWithDiamond = true;
            GetChestContent();
            ChestPanel.SetActive(true);
            GameObject ChestCloseImage = GameObject.Find("ChestPanel/ChestCloseImage");
            GameObject ChestOpenImage = GameObject.Find("ChestPanel/ChestOpenImage");
            ChestInfoPanel.SetActive(false);
            if (chestType != "Level_Chest" && chestType != "Star_Chest" && chestType != "Card_Chest")
            {
                //print(TimerChest.CurrentChest.GetComponent<TimerChest>().remainingTime / diamondChestPrice);
                switch (chestType)
                {
                    case "Standard_Chest":
                        ChestCloseImage.GetComponent<Image>().sprite = ChestImages[0];
                        ChestOpenImage.GetComponent<Image>().sprite = ChestImages[7];
                        break;
                    case "Exclusive_Chest":
                        ChestCloseImage.GetComponent<Image>().sprite = ChestImages[1];
                        ChestOpenImage.GetComponent<Image>().sprite = ChestImages[8];
                        break;
                    case "Legendary_Chest":
                        ChestCloseImage.GetComponent<Image>().sprite = ChestImages[2];
                        ChestOpenImage.GetComponent<Image>().sprite = ChestImages[9];
                        break;
                }
                TimerChest.CurrentChest.GetComponent<TimerChest>().OutOfTimeClearTimerChestData();
            }
            else
            {
                if (chestType == "Level_Chest")
                {
                    ChestCloseImage.GetComponent<Image>().sprite = ChestImages[3];
                    LevelChest._isLevelChestDone = true;
                }
                else if (chestType == "Star_Chest")
                {
                    ChestCloseImage.GetComponent<Image>().sprite = ChestImages[4];
                    StarChest._isStarChestDone = true;
                }
                else if (chestType == "Card_Chest")
                {
                    ChestCloseImage.GetComponent<Image>().sprite = ChestImages[5];
                    CardChest._isCardChestDone = true;
                }
            }
        }
        else
        {
            // ete diamond chlni
        }

    }
    void GetChestContent()
    {
        GameObject ChestNameText = GameObject.Find("ChestInfoPanel/Panel/NameField/ChestNameText");
        GameObject CoinText = GameObject.Find("ChestInfoPanel/Panel/CoinSlot/CoinText");
        GameObject DiamondText = GameObject.Find("ChestInfoPanel/Panel/DiamondSlot/DiamondText");
        GameObject ChestImage = GameObject.Find("ChestInfoPanel/Panel/ChestImage");
        ///GameObject TimerText = GameObject.Find("ChestInfoPanel/Panel/TimerText");

        if (chestType == "Standard_Chest")
        {
            ChestImage.GetComponent<Image>().sprite = ChestImages[0]; // standardchest
        }
        else if (chestType == "Exclusive_Chest")
        {
            ChestImage.GetComponent<Image>().sprite = ChestImages[1]; // exclusivechest
        }
        else if (chestType == "Legendary_Chest")
        {
            ChestImage.GetComponent<Image>().sprite = ChestImages[2]; // legendarychest
        }
        else if (chestType == "Level_Chest")
        {
            ChestImage.GetComponent<Image>().sprite = ChestImages[3]; // levelchest
        }
        else if (chestType == "Star_Chest")
        {
            ChestImage.GetComponent<Image>().sprite = ChestImages[4]; // starchest
        }
        else if (chestType == "Card_Chest")
        {
            ChestImage.GetComponent<Image>().sprite = ChestImages[5]; // cardchest
        }

        string[] ChestName = chestType.Split('_');
        ChestNameText.GetComponent<Text>().text = $"{ChestName[0]} {ChestName[1]}";
        CoinText.GetComponent<Text>().text = GameObject.Find(chestType).GetComponent<ChestContent>().GetCoins().ToString();
        DiamondText.GetComponent<Text>().text = GameObject.Find(chestType).GetComponent<ChestContent>().GetDiamonds().ToString();
        //TimerText.GetComponent<Text>().text = TimerChest.CurrentChest.transform.GetChild(0).GetComponent<Text>().text;
        CardChestSlotReposition();
    }
    void Update()
    {
        try
        {
            GameObject DiamondText = GameObject.Find("ChestInfoPanel/Panel/BuyButton/DiamondText");
            DiamondText.GetComponent<Text>().text = GetCurrentChestPrice().ToString();
        }
        catch { }
        try
        {
            GameObject TimerText = GameObject.Find("ChestInfoPanel/Panel/TimerText");
            if (chestType != "Level_Chest" && chestType != "Star_Chest" && chestType != "Card_Chest")
            {
                TimerText.SetActive(true);
                TimerText.GetComponent<Text>().text = TimerChest.CurrentChest.transform.GetChild(0).GetComponent<Text>().text;
            }
            else
            {
                TimerText.SetActive(false);
            }
            if (TimerChest.CurrentChest.GetComponent<TimerChest>().isEnd && (chestType != "Level_Chest" && chestType != "Star_Chest" && chestType != "Card_Chest"))
            {
                CloseChestInfoPanel();
            }
        }
        catch { }
    }
    void CardChestSlotReposition()
    {
        GameObject Standard_Slot = GameObject.Find("ChestInfoPanel/Panel/CardSlot/Standard_Slot");
        GameObject Exclusive_Slot = GameObject.Find("ChestInfoPanel/Panel/CardSlot/Exclusive_Slot");
        GameObject Legendary_Slot = GameObject.Find("ChestInfoPanel/Panel/CardSlot/Legendary_Slot");
        GameObject Random_Slot = GameObject.Find("ChestInfoPanel/Panel/CardSlot/Random_Slot");

        Standard_Slot.SetActive(_isStandard);
        Exclusive_Slot.SetActive(_isExclusive);
        Legendary_Slot.SetActive(_isLegendary);
        Random_Slot.SetActive(_isRandom);

        int minStandardCards = GameObject.Find(chestType).GetComponent<ChestContent>().StandardCardsCount();
        int minExclusiveCards = GameObject.Find(chestType).GetComponent<ChestContent>().ExclusiveCardsCount();
        int minLegendaryCards = GameObject.Find(chestType).GetComponent<ChestContent>().LegendaryCardsCount();
        int minRandomCards = GameObject.Find(chestType).GetComponent<ChestContent>().RandomCardsCount();

        Standard_Slot.transform.GetChild(0).GetComponent<Text>().text = $"x {minStandardCards}";
        Exclusive_Slot.transform.GetChild(0).GetComponent<Text>().text = $"x {minExclusiveCards}";
        Legendary_Slot.transform.GetChild(0).GetComponent<Text>().text = $"x {minLegendaryCards}";
        Random_Slot.transform.GetChild(0).GetComponent<Text>().text = $"x {minRandomCards}";

        if (_isRandom && !_isExclusive)
        {
            Random_Slot.GetComponent<RectTransform>().anchoredPosition = Exclusive_Slot.GetComponent<RectTransform>().anchoredPosition;
        }
        if (_isRandom && _isExclusive)
        {
            Random_Slot.GetComponent<RectTransform>().anchoredPosition = Legendary_Slot.GetComponent<RectTransform>().anchoredPosition;
        }

    }

    public void CloseChestPanel()
    {
        ChestPanel.SetActive(false);
        CardsCount.Clear();
        CoinCard.SetActive(false); DiamondCard.SetActive(false);

        for (int i = 0; i <= 2; i++)
        {
            if (ChestCards[i].transform.GetChild(ChestCards[i].transform.childCount - 1).gameObject.name == "CardBar(Clone)")
                Destroy(ChestCards[i].transform.GetChild(ChestCards[i].transform.childCount - 1).gameObject);

            ChestCards[i].SetActive(false);
        }

    }
    public void CloseChestInfoPanel()
    {
        ChestInfoPanel.GetComponent<Animator>().Play("ChestInfoPanelClose");
        StartCoroutine(ChestInfoClose());
        CardsCount.Clear();
    }
    IEnumerator ChestInfoClose()
    {
        yield return new WaitForSeconds(0.6f);
        ChestInfoPanel.SetActive(false);
    }

    GameObject GetFromDiceField(string diceName, string findingName)
    {
        string FieldName = PlayerPrefs.GetString("Dice_" + diceName + "_pos");
        GameObject DiceField = GameObject.Find(FieldName);
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
    void ResizeCardBar(GameObject CardBar) // popoxel clone exac cardbari chapery
    {
        CardBar.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // cardbari scale
        CardBar.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 40); // cardbari chapery x,y
        CardBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -112); // cardbari position
        CardBar.transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(175, 35); // handle i chapery x,y
        CardBar.transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(-0.479f, 1.027f); // handle i position
        CardBar.transform.GetChild(1).GetComponent<Text>().fontSize = 25; // CardsText i fonti chaper
        CardBar.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(45, 55); // UpgradeArrow i chapery x,y
        CardBar.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, 4); // UpgradeArrow i position
    }

    void ChangeCardCountTextColor(string hex, Text text)
    {
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            text.color = color;
        }
    }
    void RewardCardsReposition(int n, GameObject Card1, GameObject Card2, GameObject Card3)
    {
        if (n == 1)
        {
            Card1.SetActive(true);
            Card1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Card1.GetComponent<RectTransform>().anchoredPosition.y);
            Card2.SetActive(false);
            Card3.SetActive(false);
        }
        else if (n == 2)
        {
            Card1.SetActive(true);
            Card1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-150, Card1.GetComponent<RectTransform>().anchoredPosition.y);
            Card2.SetActive(true);
            Card2.GetComponent<RectTransform>().anchoredPosition = new Vector2(150, Card2.GetComponent<RectTransform>().anchoredPosition.y);
            Card3.SetActive(false);
        }
        else if (n == 3)
        {
            Card1.SetActive(true);
            Card1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, Card1.GetComponent<RectTransform>().anchoredPosition.y);
            Card2.SetActive(true);
            Card2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Card2.GetComponent<RectTransform>().anchoredPosition.y);
            Card3.SetActive(true);
            Card3.GetComponent<RectTransform>().anchoredPosition = new Vector2(300, Card3.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }

    int GetCardsFromPlPf(string rarity)
    {
        string nameToPlPf = $"{chestType}_{rarity}_{cardName}";
        int totalCards = PlayerPrefs.GetInt(nameToPlPf);
        int mincards = 0;
        if (rarity == "standard")
        {
            mincards = Mathf.RoundToInt(totalCards * 30 / 100 + totalCards);
        }
        else if (rarity == "exclusive")
        {
            mincards = Mathf.RoundToInt(totalCards * 20 / 100 + totalCards);
        }
        else if (rarity == "legendary")
        {
            mincards = Mathf.RoundToInt(totalCards * 10 / 100 + totalCards);
        }
        else if (rarity == "random")
        {
            mincards = Mathf.RoundToInt(totalCards * 10 / 100 + totalCards);
        }

        return mincards;
    }
    void SetCardsToPlPf(string rarity, int cards)
    {
        string nameToPlPf = $"{chestType}_{rarity}_{cardName}";
        PlayerPrefs.SetInt(nameToPlPf, cards);
    }

    int GetDiceClass()
    {
        int DiceClass = PlayerPrefs.GetInt($"{cardName}Class");
        return DiceClass;
    }
    int GetChestClass()
    {
        int ChestClass = PlayerPrefs.GetInt($"ChestClass{cardName}");
        return ChestClass;
    }
    void SetChestClass()
    {
        PlayerPrefs.SetInt($"ChestClass{cardName}", PlayerPrefs.GetInt($"{cardName}Class"));
    }
    void ChangeCardCount(string Rarity)
    {
        //print("Hiny: " + GetChestClass());
        //print("Nerkan: " + GetDiceClass());
        if (GetDiceClass() > GetChestClass())
        {
            int tarberutyun = GetDiceClass() - GetChestClass();
            //print(tarberutyun);
            for (int i = 0; i <= tarberutyun - 1; i++)
            {
                int minCards = GetCardsFromPlPf(Rarity);

                SetCardsToPlPf(Rarity, minCards);
                SetChestClass();
            }
        }
    }
    void GetCardCountFromChestContent()
    {
        if (rarity == "standard")
        {

            ChangeCardCount(rarity);

            int mincards = GetCardsFromPlPf(rarity);
            int maxcards = Mathf.RoundToInt(mincards + (mincards * 25 / 100));

            //print($"{mincards}-ic {mincards + (mincards * 25 / 100)}");
            Cards.GiveRandomCards(mincards, maxcards, rarity); // playerprefsnery avelacni diceri cardi hamar

            // es hramany petqa ashxati classi depqum el, tarberaka lucman, classy paheluc nayev pahenq 
            // classy chesti hamar,heto hamematenq ete nerka tivy arajva tvic tarbervuma et depqumnor ani da
        }
        if (!_isRandom)
        {
            if (rarity == "exclusive")
            {
                ChangeCardCount(rarity);
                //print(cardName + ": " + thisClass);
                int mincards = GetCardsFromPlPf(rarity);
                int maxcards = Mathf.RoundToInt(mincards + (mincards * 25 / 100));

                //print($"{mincards}-ic {mincards + (mincards * 25 / 100)}");
                Cards.GiveRandomCards(mincards, maxcards, rarity); // playerprefsnery avelacni diceri cardi hamar
                                                                   //SetCardsToPlPf(rarity, mincards);
            }
            else if (rarity == "legendary")
            {
                ChangeCardCount(rarity);
                //print(cardName + ": " + thisClass);
                int mincards = GetCardsFromPlPf(rarity);
                int maxcards = Mathf.RoundToInt(mincards + (mincards * 25 / 100));

                //print($"{mincards}-ic {mincards + (mincards * 25 / 100)}");
                Cards.GiveRandomCards(mincards, maxcards, rarity); // playerprefsnery avelacni diceri cardi hamar
                                                                   //SetCardsToPlPf(rarity, mincards);
            }
        }
        else
        {
            if (rarity == "exclusive" && !_isExclusive) // standard + randomov nver
            {
                ChangeCardCount("random");
                //print(cardName + ": " + thisClass);
                int mincards = GetCardsFromPlPf("random");
                int maxcards = Mathf.RoundToInt(mincards + (mincards * 25 / 100));

                //print($"{mincards}-ic {mincards + (mincards * 25 / 100)}");
                Cards.GiveRandomCards(mincards, maxcards, rarity); // playerprefsnery avelacni diceri cardi hamar
                                                                   //SetCardsToPlPf("random", mincards);
            }

            if (rarity == "exclusive" && _isExclusive) // legendar ete lini exclusivy vercni exclusive carderic
            {
                ChangeCardCount(rarity);
                //print(cardName + ": " + thisClass);
                int mincards = GetCardsFromPlPf(rarity);
                int maxcards = Mathf.RoundToInt(mincards + (mincards * 25 / 100));

                //print($"{mincards}-ic {mincards + (mincards * 25 / 100)}");
                Cards.GiveRandomCards(mincards, maxcards, rarity); // playerprefsnery avelacni diceri cardi hamar
                                                                   //SetCardsToPlPf(rarity, mincards);
            }

            if (rarity == "legendary" && _isExclusive) // standard, exclusive + randomov nver
            {
                ChangeCardCount("random");
                //print(cardName + ": " + thisClass);
                int mincards = GetCardsFromPlPf("random");
                int maxcards = Mathf.RoundToInt(mincards + (mincards * 25 / 100));

                //print($"{mincards}-ic {mincards + (mincards * 25 / 100)}");
                Cards.GiveRandomCards(mincards, maxcards, rarity); // playerprefsnery avelacni diceri cardi hamar
                                                                   //SetCardsToPlPf("random", mincards);
            }
        }

    }
    void SetDiceNames(int n, string DiceName)
    {
        DiceNameTexts[n].text = DiceName;
    }
    public void OpenChest()
    {

        if (chestType != "Level_Chest" && chestType != "Star_Chest" && chestType != "Card_Chest" && !isChestOpenFromDaily)
        {
            try
            {
                TimerChest.CurrentChest.GetComponent<TimerChest>().ClearChestData(false);
            }
            catch { }
        }
        OpenButton.SetActive(false);
        CoinCard.SetActive(true);
        DiamondCard.SetActive(true);
        int n = 0;
        RewardCount = 0;
        for (int i = 0; i <= CardsCount.Count - 1; i++) // stanal te qani tesak card ka
        {
            if (CardsCount[i])
            {
                RewardCount += 1;
            }
        }
        if (_isRandom)
        {
            RewardCount += 1;
        }
        while (n != RewardCount) // 3 diceeri hamar card stanalu loop
        {
            //int randomType;

            switch (RewardCount)
            {
                case 1:
                    rarity = Rarity[n];
                    break;
                case 2:
                    rarity = Rarity[n];
                    break;
                case 3:
                    rarity = Rarity[n];
                    break;
            }

            Cards.GetRandomData(rarity); // stanuma patahakan Dice

            switch (rarity) // yst rarityi stanuma patahakan dicei anun
            {
                case "standard":
                    cardName = Cards.St_Names[Cards.random_dice]; // Dicei anuny
                    break;
                case "exclusive":
                    cardName = Cards.Ex_Names[Cards.random_dice]; // Dicei anuny
                    break;
                case "legendary":
                    cardName = Cards.Lg_Names[Cards.random_dice]; // Dicei anuny
                    break;
            }

            if (!Rewards.Contains(cardName)) // ete dicy chka nverneri cankum 
            {
                #region CardInformation
                RewardCardsReposition(RewardCount, ChestCards[0], ChestCards[1], ChestCards[2]);

                if (_isRandom) // 1
                {
                    RandomTexts[RewardCount - 2].enabled = true;
                }
                NewImages[n].enabled = PlayerPrefs.GetInt(cardName + "TotalCards") < 2 && !LockDice.DiceIsUnlocked(cardName) ? true : false; // stugum, ete dicy nora, NEW imagey cuyc ta
                SetDiceNames(n, cardName);
                GetCardCountFromChestContent(); // stanal carderi qanaky amen cardi hamar

                DiceImages[n].sprite = GetFromDiceField(cardName, "DiceHead").GetComponent<Image>().sprite; // Stanal Dice i imagy ira Dicefieldic
                var CardBar = Instantiate // stexcel CardBari clony Dicefieldic
                (
                   GetFromDiceField(cardName, "PowerInfo").transform.GetChild(1), // stanal Dicefieldic cardbary
                   DiceImages[n].transform.position, // cardbari pozician fixel Diceimage-i poziciayin
                   Quaternion.identity // shat gitem
                );
                CardBar.SetParent(ChestCards[n].transform); // nor stexcvac cardbarin dardznel ChestCards-i qyorpa
                ResizeCardBar(CardBar.gameObject); // uxxel cardbari chapery dinamik kerpov

                if (rarity == "standard")
                {
                    //ChangeCardCountTextColor("#FCDB57", CardCountTexts[n]);
                }
                else if (rarity == "exclusive")
                {
                    //ChangeCardCountTextColor("#79BD62", CardCountTexts[n]);
                }
                else if (rarity == "legendary")
                {
                    //ChangeCardCountTextColor("#F9563E", CardCountTexts[n]);
                }

                CardCountTexts[n].text = "x " + Cards.giftcards; // carderi qanaki texty havasarecnel giftcardi textin
                #endregion
                Rewards.Add(cardName);
                n++;
            }
        }


        int RewardCoins = GameObject.Find(chestType).GetComponent<ChestContent>().GetCoins();
        int Coins_25percent = Mathf.RoundToInt(RewardCoins + (RewardCoins * 25 / 100));
        RewardCoin.text = Coins_25percent.ToString();
        Coin.Coins += Coins_25percent;

        int RewardDiamonds = GameObject.Find(chestType).GetComponent<ChestContent>().GetDiamonds();
        int Diamonds_25percent = Mathf.RoundToInt(RewardDiamonds + (RewardDiamonds * 25 / 100));
        RewardDiamond.text = Diamonds_25percent.ToString();
        Diamond.Diamonds += Diamonds_25percent;

        if (!_isBuyedWithDiamond)
        {
            switch (chestType)
            {
                case "Level_Chest":
                    LevelChest.totalLevelCount = LevelChest.totalLevelCount - LevelChest.maxCountToOpen;
                    break;
                case "Star_Chest":
                    StarChest.totalStarCount = StarChest.totalStarCount - StarChest.maxCountToOpen;
                    break;
                case "Card_Chest":
                    CardChest.totalCardCount = CardChest.totalCardCount - CardChest.maxCountToOpen;
                    break;
            }
        }


        _isBuyedWithDiamond = false;
        Rewards.Clear();
        CardsCount.Clear();
    }
}





