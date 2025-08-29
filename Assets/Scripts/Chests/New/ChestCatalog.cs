// Assets/Scripts/Chests/ChestCatalog.cs
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Chest Catalog")]
public class ChestCatalog : ScriptableObject
{
    public ChestDefinition[] All;

    public ChestDefinition ById(string id) => All.FirstOrDefault(x => x.Id == id);
}
