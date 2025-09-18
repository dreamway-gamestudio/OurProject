// Assets/Editor/ChestCatalogValidator.cs
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChestCatalog))]
public class ChestCatalogValidator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var catalog = (ChestCatalog)target;
        if (catalog.All == null) return;

        var dups = catalog.All
            .Where(x => x != null)
            .GroupBy(x => x.Id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        if (dups.Length > 0)
        {
            EditorGUILayout.HelpBox("Дублирующиеся Id: " + string.Join(", ", dups), MessageType.Error);
        }
        else
        {
            EditorGUILayout.HelpBox("Id уникальны — всё ок.", MessageType.Info);
        }
    }
}
#endif
