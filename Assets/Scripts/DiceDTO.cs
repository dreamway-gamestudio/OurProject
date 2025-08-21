using System;
using UnityEngine;

[Serializable]
public class DiceDTO
{
    public string Name;
    public string Rarity;        // Standard/Exclusive/Legendary
    public int Attack;
    public float ReloadTime;
    public float ShootSpeed;
    public string Target;        // e.g., "Nearest", "Strongest"
    public int DiamondPrice;
    public int Class;            // 1/3/5 (по твоей логике)
    public int TotalCards;       // если нужно
}
