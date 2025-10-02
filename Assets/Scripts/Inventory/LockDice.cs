using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockDice : MonoBehaviour
{
    bool isUnlocked;
    int totalDices = 30;
    bool forReturn;
    GameObject Chain, PowerInfo;
    void Start()
    {
        print(DiceIsUnlocked("Dog"));
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

                    isUnlocked = DataSave.GetInt($"Dice_{this_name}_isUnlocked") == 1 ? true : false;
                }
                if (DiceField.transform.GetChild(j).name == "Chain")
                {
                    Chain = DiceField.transform.GetChild(j).gameObject;
                    if (i > 5)
                    {
                        if (isUnlocked)
                        {
                            Chain.GetComponent<Image>().enabled = false;
                        }
                        else
                        {
                            Chain.GetComponent<Image>().enabled = true;
                        }
                    }
                }
                if (DiceField.transform.GetChild(j).name == "PowerInfo")
                {
                    PowerInfo = DiceField.transform.GetChild(j).gameObject;
                    if (i > 5)
                    {
                        if (isUnlocked)
                        {
                            PowerInfo.SetActive(true);
                        }
                        else
                        {
                            PowerInfo.SetActive(false);
                        }
                    }
                }

            }
        }
    }
    public static bool DiceIsUnlocked(string diceName)
    {
        bool isUnlocked = DataSave.GetInt($"Dice_{diceName}_isUnlocked") == 1 ? true : false;
        return isUnlocked;

    }
}
