using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideFields : MonoBehaviour
{
    DiceInfoPanel DiceInfoPanel;
    private Canvas GameCanvas;
    void Start()
    {
        DiceInfoPanel = GameObject.FindObjectOfType<DiceInfoPanel>();
        GameCanvas = gameObject.GetComponent<Canvas>();
    }
    void Update()
    {
        if (DiceInfoPanel._isDiceUsed)
        {
            GameCanvas.overrideSorting = true;
        } 
        else
        {
            GameCanvas.overrideSorting = false;
        }
    }
}
