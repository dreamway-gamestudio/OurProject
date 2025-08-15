using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiveGameChests : MonoBehaviour
{
    TimerChest TimerChest;
    GameObject firstEmpty;
    GameObject Chest;
    GameObject TouchBlock;
    GameObject TimerText;

    void Start()
    {
        LoadChests();
        TimerChest = FindObjectOfType<TimerChest>();
        TouchBlock = GameObject.Find("TouchBlock");
        TouchBlock.GetComponent<Image>().enabled = false;
        PlayerPrefs.SetInt($"isGiveStandard", 1);


    }
    // image
    public void LoadChests() // statrum bacel pahpanvac chesty 
    {
        for (int i = 1; i <= 3; i++)
        {
            string findingChest = $"ChestCell_{i}";
            GameObject ChestCell = GameObject.Find(findingChest);
            for (int j = 0; j <= ChestCell.transform.childCount - 1; j++)
            {

                if (ChestCell.transform.GetChild(j).name == PlayerPrefs.GetString(findingChest))
                {
                    GameObject CurrentChest = ChestCell.transform.GetChild(j).gameObject;
                    string statusText = CurrentChest.GetComponent<TimerChest>().GetStatusText();
                    CurrentChest.SetActive(true);

                    CurrentChest.transform.GetChild(1).gameObject.GetComponent<Text>().enabled = true; // status text
//                    print($"{CurrentChest.name} - {CurrentChest.GetComponent<TimerChest>().isEnd}");

                    if (CurrentChest.GetComponent<TimerChest>().isEnd)
                    {
                        CurrentChest.transform.GetChild(0).gameObject.GetComponent<Text>().enabled = false;
                    }
                    else
                    {
                        CurrentChest.transform.GetChild(0).gameObject.GetComponent<Text>().enabled = true;
                    }
                    CurrentChest.transform.GetChild(1).gameObject.GetComponent<Text>().text = statusText;


                }
            }
        }
    }
    public void GiveChest()
    {
        int randomChest = Random.Range(1, 100);
        switch (randomChest)
        {
            case int n when (n > 20 && n <= 100):
                GiveStandardChest();
                break;

            case int n when (n > 5 && n <= 20):
                GiveExclusiveChest();
                break;

            case int n when (n > 0 && n <= 5):
                GiveLegendaryChest();
                break;
        }


    }
    GameObject GetFirstEmpty(string type)
    {
        bool isFinded = false;
        for (int i = 1; i <= 3; i++)
        {
            GameObject Cell = GameObject.Find($"ChestCell_{i}");

            for (int j = 0; j <= Cell.transform.childCount - 1; j++)
            {
                if (Cell.transform.GetChild(j).name == $"{type}_Chest" && !Cell.transform.GetChild(j).gameObject.activeSelf && !CellIsEmpty(i))
                {
                    firstEmpty = Cell.transform.GetChild(j).gameObject;

                    //StartPos = Cell.transform.position;
                    Chest = firstEmpty;
                    isFinded = true;
                }
            }
            if (isFinded)
                break;
        }
        return firstEmpty;
    }



    bool CellIsEmpty(int i)
    {
        bool isEmpty = false;
        GameObject Cell = GameObject.Find($"ChestCell_{i}");
        for (int j = 0; j <= Cell.transform.childCount - 1; j++)
        {
            if (Cell.transform.GetChild(j).name.Contains("Chest") && Cell.transform.GetChild(j).gameObject.activeSelf)
            {
                isEmpty = true;
            }
        }
        return isEmpty;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {

            PlayerPrefs.SetInt($"isGiveStandard", 1);
            GetFirstEmpty("Standard").SetActive(true);
            //MoveChestAnim();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            PlayerPrefs.SetInt($"isGiveExclusive", 1);
            GetFirstEmpty("Exclusive").SetActive(true);
            //MoveChestAnim();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            PlayerPrefs.SetInt($"isGiveLegendary", 1);
            GetFirstEmpty("Legendary").SetActive(true);
            //MoveChestAnim();
        }
    }
    public void GiveGhestforTest(string button_name)
    {
        if (button_name == "S")
        {
            GetFirstEmpty("Standard").SetActive(true);
        }
        else if (button_name == "E")
        {
            GetFirstEmpty("Exclusive").SetActive(true);
        }
        else if (button_name == "L")
        {
            GetFirstEmpty("Legendary").SetActive(true);
        }
    }



    void GiveStandardChest()
    {
        //print("Standard");
    }
    void GiveExclusiveChest()
    {
        //print("Exclusive");
    }
    void GiveLegendaryChest()
    {
        //print("Legendary");
    }
}
