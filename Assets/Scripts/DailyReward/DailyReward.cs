using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DailyReward : MonoBehaviour
{
    public GameObject DailyRewardPanel, CollectButton;
    public GameObject[] OriginalCards;
    public GameObject[] ShuffledCards;
    private GameObject currentCard;
    private List<Vector2> CardPositions = new List<Vector2>();
    private List<Vector2> CardStartPositions = new List<Vector2>();
    GameObject CurrentSelected;

    bool _isDailyDone, _isCardOpened = false;
    Chests Chests;
    DailyTimer DailyTimer;
    string currentSelectedName;

    void Start()
    {
        SaveStartPositions();
        Chests = GameObject.FindObjectOfType<Chests>();
        DailyTimer = GameObject.FindObjectOfType<DailyTimer>();
        DailyRewardPanel.SetActive(false);
        CollectButton.SetActive(false);

    }
    void SaveStartPositions()
    {
        for (int i = 0; i < OriginalCards.Length; i++) // pahpanuma Vector3[] arrayi mej bolor xarnac Carderi dirqery
        {
            CardStartPositions.Add(new Vector2(OriginalCards[i].GetComponent<RectTransform>().anchoredPosition.x, OriginalCards[i].GetComponent<RectTransform>().anchoredPosition.y));
        }
    }
    void ShuffleArray()  // xarnuma carderi massivy
    {
        for (int i = 0; i < ShuffledCards.Length; i++)
        {
            int rnd = Random.Range(0, ShuffledCards.Length);
            currentCard = ShuffledCards[rnd];
            ShuffledCards[rnd] = ShuffledCards[i];
            ShuffledCards[i] = currentCard;
        }
        //print("Xarnec");

        //ShuffleCards();
        StartCoroutine(ShuffleTest());
    }

    IEnumerator ShuffleTest()
    {
        yield return new WaitForSeconds(1.4f);
        GameObject.Find("DailyRewardPanel").GetComponent<Animator>().enabled = false;
        ShuffleCards();

    }
    void ShuffleCards() // xarnuma Carderi texery
    {
        CardPositions.Clear();

        for (int i = 0; i < OriginalCards.Length; i++) // pahpanuma Vector3[] arrayi mej bolor xarnac Carderi dirqery
        {
            CardPositions.Add(new Vector2(ShuffledCards[i].GetComponent<RectTransform>().anchoredPosition.x, ShuffledCards[i].GetComponent<RectTransform>().anchoredPosition.y));
        }

        for (int i = 0; i < OriginalCards.Length; i++) // Original Carderi dirqery hertakanutyamb poxuma yst xarnac array-i
        {
            OriginalCards[i].GetComponent<RectTransform>().anchoredPosition = CardPositions[i];//ShuffledCards[i].GetComponent<RectTransform>().anchoredPosition ;
        }
        print("xarnec irar mazy");
        //GameObject.Find("DailyRewardPanel").GetComponent<Animator>().enabled = true;
    }
    void ResetToStartPositions()
    {
        for (int i = 0; i < OriginalCards.Length; i++) // Original Carderi dirqery hertakanutyamb poxuma yst xarnac array-i
        {
            OriginalCards[i].GetComponent<RectTransform>().anchoredPosition = CardStartPositions[i];//ShuffledCards[i].GetComponent<RectTransform>().anchoredPosition ;
        }
    }
    void ResetAllCards() // pakuma bolor CloseImage-nery u xarnuma noric dirqery
    {
        ResetToStartPositions();
        ResizeCards();
        ChooseImage(false);
        HideOrShowCloseImage(true);
        ShuffleArray();
    }
    void HideOrShowCloseImage(bool isActive)
    {
        for (int i = 0; i <= OriginalCards.Length - 1; i++)
        {
            GameObject CloseImage = GameObject.Find($"DailyCard_{i + 1}/CloseImage");
            CloseImage.GetComponent<Image>().enabled = isActive;
        }
    }
    void ChooseImage(bool isActive)
    {
        GameObject ChooseImageDailyPanel = GameObject.Find("ChooseImageDailyPanel"); // gtnuma daily paneli meji sev choose image y 
        ChooseImageDailyPanel.GetComponent<Image>().enabled = isActive; // bacuma choose image i image enabled y 
    }
    void ResizeCards()
    {
        for (int i = 0; i < OriginalCards.Length; i++) // Original Carderi dirqery hertakanutyamb poxuma yst xarnac array-i
        {

            OriginalCards[i].transform.localScale = new Vector3(1f, 1f, 1f);
            OriginalCards[i].GetComponent<Canvas>().overrideSorting = false;
        }
    }
    public void OpenDailyPanel()
    {

        if (DailyTimer.isReady) // sa paymany klni ete done a dailyn u 
        {
            DailyRewardPanel.SetActive(true);
            GameObject.Find("DailyRewardPanel").GetComponent<Animator>().enabled = true;
            ResetAllCards();
            _isDailyDone = false; // dailin stacav arden false kanenq verevi ifi paymany vor el chbaci 
            _isCardOpened = false; // der bac card chka
            ChangeTextLineText("Choose a card  to get a \n reward!");

        }
        else
        {

        }
    }
    public void OpenCurrentCard()
    {
        if (!_isCardOpened && !GameObject.Find("DailyRewardPanel").GetComponent<Animator>().enabled)
        {

            CurrentSelected = EventSystem.current.currentSelectedGameObject;
            currentSelectedName = CurrentSelected.transform.parent.name;
            StartCoroutine(CloseImageFalse()); // pakuma sexmvac CloseImage-y
            //CurrentSelected.transform.parent.localScale = new Vector3(1.25f, 1.25f, 1f); // mecacnuma yntrac cardy
            CurrentSelected.transform.parent.GetComponent<Canvas>().overrideSorting = true; // miacnuma yntrac cardi override sorting y
            ChooseImage(true);
            StartCoroutine(CollectButtonOpenORClose(1f, true)); // aktivacnuma collect buttony 
            CollectButton.GetComponent<Animator>().Play("CollectButtonOpen"); // CollectButtoni arandzin animacia
            ChangeTextLineText("Come back tomorrow \n for another reward!");
            CurrentSelected.transform.parent.GetComponent<Animator>().Play("DailyCardFlip");
            CollectButton.GetComponent<Canvas>().overrideSorting = true; // miacnuma collect buttoni override sorting y 
            StartCoroutine(ShowAllCards());

            _isCardOpened = true;
            print("card opened - collect done");
        }

    }
    void ChangeTextLineText(string text)
    {
        GameObject.Find("DailyRewardPanel/TextLine/Text").GetComponent<Text>().text = text;
    }
    IEnumerator CloseImageFalse()
    {
        yield return new WaitForSeconds(0.45f);
        CurrentSelected.GetComponent<Image>().enabled = false;
    }
    IEnumerator CloseImageFalseOther(GameObject Other)
    {
        yield return new WaitForSeconds(0.45f);
        Other.GetComponent<Image>().enabled = false;
    }
    IEnumerator ShowAllCards()
    {
        yield return new WaitForSeconds(1f);
        try
        {
            for (int i = 0; i <= OriginalCards.Length - 1; i++)
            {
                if (OriginalCards[i].name == currentSelectedName) continue;
                OriginalCards[i].GetComponent<Animator>().Play("DailyCardFlipOther");
                StartCoroutine(CloseImageFalseOther(GameObject.Find($"DailyCard_{i + 1}/CloseImage")));
            }

        }
        catch { }
    }
    IEnumerator GiveRewardCount(string rewardType, int rewardCount)
    {
        yield return new WaitForSeconds(1.5f);
        switch (rewardType)
        {
            case "coin":
                print($"{rewardType}: " + rewardCount);
                Coin.Coins += rewardCount;
                break;
            case "diamond":
                print($"{rewardType}: " + rewardCount);
                Diamond.Diamonds += rewardCount;
                break;
            case "elixir":
                print($"{rewardType}: " + rewardCount);
                Elixir.AddElixir(rewardCount);
                break;
            case "Standard_Chest":
                print($"{rewardType}: " + rewardCount);
                Chests.OpenChestFromDaily(rewardType);
                break;
            case "Exclusive_Chest":
                print($"{rewardType}: " + rewardCount);
                Chests.OpenChestFromDaily(rewardType);
                break;
            case "Legendary_Chest":
                print($"{rewardType}: " + rewardCount);
                Chests.OpenChestFromDaily(rewardType);
                break;
        }
    }
    public void GetDailyReward() // collect reward
    {
        string rewardType = CurrentSelected.transform.parent.GetComponent<GiveDailyReward>().rewardType;
        int rewardCount = CurrentSelected.transform.parent.GetComponent<GiveDailyReward>().rewardCount;
        StartCoroutine(GiveRewardCount(rewardType, rewardCount));
        CollectButton.GetComponent<Animator>().Play("CollectButtonClose");
        StartCoroutine(CollectButtonOpenORClose(0.5f, false));
        for (int i = 0; i <= OriginalCards.Length - 1; i++)
        {
            OriginalCards[i].GetComponent<Animator>().Play("DailyCardClose");
        }
        StartCoroutine(DailyPanelAnimatorTrue());
        StartCoroutine(DailyPanelClose());
        DailyTimer.ClearDailyRewardData();
    }
    IEnumerator DailyPanelAnimatorTrue()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("DailyRewardPanel").GetComponent<Animator>().enabled = true;
    }
    IEnumerator DailyPanelClose()
    {
        yield return new WaitForSeconds(1.15f);
        CloseDailyPanel();
    }
    IEnumerator CollectButtonOpenORClose(float seconds, bool isActive)
    {
        yield return new WaitForSeconds(seconds);
        CollectButton.SetActive(isActive);
        CollectButton.GetComponent<Canvas>().overrideSorting = isActive;
    }
    public void CloseDailyPanel()
    {

        DailyRewardPanel.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _isDailyDone = true;
        }
    }
}
