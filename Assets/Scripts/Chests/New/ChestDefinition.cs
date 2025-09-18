// Assets/Scripts/Chests/ChestDefinition.cs
using UnityEngine;
using System.Collections.Generic;

public enum ChestRarity { Common, Rare, Epic, Legendary }

[CreateAssetMenu(menuName = "Game/Chest Definition")]
// + добавили визуальные поля для UI
public class ChestDefinition : ScriptableObject
{
    public string Id;                   // уникальный ключ
    public string DisplayName;          // текст в UI
    public ChestRarity Rarity;          // редкость
    public int DurationSeconds;         // таймер открытия (сек)
    public int GemOpenNowCost;          // цена мгновенного открытия
    public bool RewardedSkipAllowed;    // можно ли сокращать рекламой

    [Header("Rewards (Ranges)")]
    public Vector2Int CoinsRange;       // диапазон монет
    public Vector2Int CardsRange;       // диапазон карт (или фрагментов)

    [Header("UI (optional)")]
    public Sprite Icon;                 // иконка сундука
    public Color Accent = Color.white;  // цвет подсветки/рамки по редкости
}
// комментарии в коде помогут тебе быстро понять, что за что отвечает

