using UnityEngine;

public class ChestHotkeys : MonoBehaviour
{
    public ChestManager manager;

    [Header("Chest Ids (из вашего ChestCatalog)")]
    public string chestH = "Chest_1"; // H
    public string chestJ = "Chest_2"; // J
    public string chestK = "Chest_3"; // K
    public string chestL = "Chest_4"; // L

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) TryPut(chestH);
        if (Input.GetKeyDown(KeyCode.J)) TryPut(chestJ);
        if (Input.GetKeyDown(KeyCode.K)) TryPut(chestK);
        if (Input.GetKeyDown(KeyCode.L)) TryPut(chestL);
    }

    void TryPut(string id)
    {
        var idx = manager.TryPutIntoFirstEmptySlot(id);
        if (idx >= 0) Debug.Log($"Положен {id} в слот {idx}");
    }
}
