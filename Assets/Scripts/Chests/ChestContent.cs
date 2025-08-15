using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestContent : MonoBehaviour
{
    public int diamondPrice;
    public int MinRewardCoins, MinRewardDiamonds;
    public bool _isStandardCard, _isExclusiveCard, _isLegendaryCard, _isRandomCard;
    public int MinStandardCards, MinExclusiveCards, MinLegendaryCards, MinRandomCards;

    public int GetCoins()
    {
        return MinRewardCoins;
    }
    public int GetDiamonds()
    {
        return MinRewardDiamonds;
    }


    public bool isStandardCard()
    {
        return _isStandardCard;
    }
    public bool isExclusiveCard()
    {
        return _isExclusiveCard;
    }
    public bool isLegendaryCard()
    {
        return _isLegendaryCard;
    }
    public bool isRandomCard()
    {
        return _isRandomCard;
    }


    public int StandardCardsCount()
    {
        return MinStandardCards;
    }
    public int ExclusiveCardsCount()
    {
        return MinExclusiveCards;
    }
    public int LegendaryCardsCount()
    {
        return MinLegendaryCards;
    }
    public int RandomCardsCount()
    {
        return MinRandomCards;
    }
}
