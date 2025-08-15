using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    Dice Dice;
    void Start()
    {
        Dice = GameObject.FindObjectOfType<Dice>();
    }
    public void PowerUpDice()
    {
        if (Dice.DiceName == "Blue")
        {
            Dice.Attack += 15;
            print("Damage++");
        }
    }
}
