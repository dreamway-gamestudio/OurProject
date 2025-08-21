using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    public static int Diamonds;

    [Header("Save Strategy")]
    public bool saveImmediately = false;
    public int criticalSaveThreshold = 250;

    void Start()
    {
        StartCoroutine(InitializeDiamonds());
    }

    IEnumerator InitializeDiamonds()
    {
        while (!CloudDataManager.Instance.IsInitialized)
            yield return new WaitForSeconds(0.1f);

        Diamonds = CloudDataManager.Instance.GetInt("Diamonds", 0);
        Debug.Log($"Loaded {Diamonds} diamonds from cloud save");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            AddDiamonds(100);

        if (Input.GetKeyDown(KeyCode.K))
            ForceSave(); // тестовая горячая клавиша
    }

    public static void AddDiamonds(int amount)
    {
        Diamonds += amount;
        SaveDiamondsWithStrategy();
    }

    public static bool SpendDiamonds(int amount)
    {
        if (Diamonds < amount) return false;
        Diamonds -= amount;
        SaveDiamondsWithStrategy();
        return true;
    }

    static void SaveDiamondsWithStrategy()
    {
        if (CloudDataManager.Instance == null || !CloudDataManager.Instance.IsInitialized) return;
        var script = FindObjectOfType<Diamond>(); if (script == null) return;

        if (script.saveImmediately || Diamonds >= script.criticalSaveThreshold)
            DataSave.SetIntCritical("Diamonds", Diamonds);
        else
            DataSave.SetInt("Diamonds", Diamonds);
    }

    public static void ForceSave()
    {
        DataSave.SetIntCritical("Diamonds", Diamonds);
        Debug.Log("Diamonds force-saved!");
    }

    
}