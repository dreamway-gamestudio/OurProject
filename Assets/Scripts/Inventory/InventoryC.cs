using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryC : MonoBehaviour, IPointerClickHandler
{
    Inventory Inventory;
    void Start()
    {
        Inventory = GameObject.FindObjectOfType<Inventory>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
        Inventory.HideButtons_DIP();
    }
}
