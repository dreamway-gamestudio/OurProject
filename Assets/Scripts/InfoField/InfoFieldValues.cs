using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InfoFieldValues : MonoBehaviour
{
    public Text CoinText,DiamondText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CoinText.text = Coin.Coins.ToString();
        DiamondText.text = Diamond.Diamonds.ToString();
    }
}
