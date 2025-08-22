using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockDice : MonoBehaviour
{
    bool isUnlocked;
    int totalDices = 30;
    bool forReturn;
    GameObject BlockImage;
    void Start()
    {
        //print(DiceIsUnlocked("Eagle"));
    }

    public void CheckDiceBuyed()
    {
        for (int i = 1; i <= totalDices; i++) // diceri isBuyed
        {
            string name = $"DiceField_{i}"; // dice i parenti anunna, vory kpoxvi amen cikli jamanak (1...5)
            GameObject DiceField = GameObject.Find(name);

            for (int j = 0; j <= DiceField.transform.childCount - 1; j++)
            {

                if (DiceField.transform.GetChild(j).name == "DiceNameText")
                {
                    GameObject DiceText = DiceField.transform.GetChild(j).gameObject;
                    string this_name = DiceText.GetComponent<Text>().text;

                    isUnlocked = PlayerPrefs.GetInt($"Dice_{this_name}_isUnlocked") == 1 ? true : false;
                }
                if (DiceField.transform.GetChild(j).name == "BlockImage")
                {
                    BlockImage = DiceField.transform.GetChild(j).gameObject;
                    if (i > 5)
                    {
                        if (isUnlocked)
                        {
                            BlockImage.GetComponent<Image>().enabled = false;
                        }
                        else
                        {
                            BlockImage.GetComponent<Image>().enabled = true;
                        }
                    }
                }

            }
        }
    }
    public static bool DiceIsUnlocked(string diceName)
    {
        bool isUnlocked = PlayerPrefs.GetInt($"Dice_{diceName}_isUnlocked") == 1 ? true : false;
        return isUnlocked;

    }
}
