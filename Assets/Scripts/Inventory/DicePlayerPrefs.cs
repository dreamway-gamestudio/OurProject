using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicePlayerPrefs : MonoBehaviour
{
    GameObject Dice;
    public GameObject[] Dices;
    [HideInInspector] public bool ClassInit = false;
    public void InitDiceInfo()  // ashxatel mek angam iranc vric qerel mnacacy plpfov
    {
        ClassInit = PlayerPrefs.GetInt("ClassInit") == 1 ? true : false;
        
        if (!ClassInit)
        {
            for (int i = 0; i <= Dices.Length - 1; i++)
        {
            
            PlayerPrefs.SetString(Dices[i].GetComponent<Dice>().DiceName + "Rarity", Dices[i].GetComponent<Dice>().Rarity.ToString());
            PlayerPrefs.SetInt(Dices[i].GetComponent<Dice>().DiceName + "Attack", Dices[i].GetComponent<Dice>().Attack);
            PlayerPrefs.SetFloat(Dices[i].GetComponent<Dice>().DiceName + "ReloadTime", Dices[i].GetComponent<Dice>().ReloadTime);
            PlayerPrefs.SetFloat(Dices[i].GetComponent<Dice>().DiceName + "ShootSpeed", Dices[i].GetComponent<Dice>().shootSpeed);
            PlayerPrefs.SetString(Dices[i].GetComponent<Dice>().DiceName + "Target", Dices[i].GetComponent<Dice>().Target.ToString());
            PlayerPrefs.SetInt(Dices[i].GetComponent<Dice>().DiceName + "DiamondPrice", Dices[i].GetComponent<Dice>().priceWithDiamond);

            }
            print("mtav 2");
            for (int i = 0; i <= Dices.Length - 1; i++)
            {
                if (Dices[i].GetComponent<Dice>().Rarity.ToString() == "Standard")
                {
                    PlayerPrefs.SetInt(Dices[i].GetComponent<Dice>().DiceName + "Class", 1);
                }
                else if (Dices[i].GetComponent<Dice>().Rarity.ToString() == "Exclusive")
                {
                    PlayerPrefs.SetInt(Dices[i].GetComponent<Dice>().DiceName + "Class", 3);
                }
                else if (Dices[i].GetComponent<Dice>().Rarity.ToString() == "Legendary")
                {
                    PlayerPrefs.SetInt(Dices[i].GetComponent<Dice>().DiceName + "Class", 5);
                }

                PlayerPrefs.SetInt(Dices[i].GetComponent<Dice>().DiceName + "TotalCards", 0);
            }
            ClassInit = true;
            PlayerPrefs.SetInt("ClassInit", (ClassInit ? 1 : 0));
        }


    }
    public void UpgradeDice(string DiceName) // levelup i jamanak diceri hzoracum
    {

        if(DiceName == "Squirrel")
        {
            UpgradeShootSpeed(DiceName, 3);
        }
        //UpgradeAttack(DiceName, 5);
        //UpgradeReloadTime(DiceName, 2);
        //UpgradeShootSpeed(DiceName, 1);
        //InitDiceInfo();
    }

    #region GetDiceValue
    public static string GetRarity(string name)
    {
        return PlayerPrefs.GetString(name + "Rarity");
    }
    public static int GetAttack(string name)
    {
        return PlayerPrefs.GetInt(name + "Attack");
    }
    public static string GetTarget(string name)
    {
        return PlayerPrefs.GetString(name + "Target");
    }
    public static float GetReloadTime(string name)
    {
        return PlayerPrefs.GetFloat(name + "ReloadTime");
    }
    public static float GetShootSpeed(string name)
    {
        return PlayerPrefs.GetFloat(name + "ShootSpeed");
    }
    public static int GetDiamondPrice(string name)
    {
        return PlayerPrefs.GetInt(name + "DiamondPrice");
    }
    #endregion

    #region ChangeDiceValue
    void UpgradeAttack(string diceName, int value)
    {
        PlayerPrefs.SetInt(diceName + "Attack", PlayerPrefs.GetInt(diceName + "Attack") + value);
    }

    void UpgradeReloadTime(string diceName, float value)
    {
        PlayerPrefs.SetFloat(diceName + "ReloadTime", PlayerPrefs.GetFloat(diceName + "ReloadTime") + value);
    }

    void UpgradeShootSpeed(string diceName, float value)
    {
        PlayerPrefs.SetFloat(diceName + "ShootSpeed", PlayerPrefs.GetFloat(diceName + "ShootSpeed") + value);
    }

    #endregion
}
