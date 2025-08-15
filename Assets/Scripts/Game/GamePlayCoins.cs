using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GamePlayCoins : MonoBehaviour
{
    Text CoinText;

    public static int Coins;
    void Start()
    {
        CoinText = GetComponent<Text>();
        Coins = 0;
    }
    void Update()
    {
        PlayerPrefs.SetInt("CoinsGP", Coins);
        CoinText.text = Coins + "";
    }
}
