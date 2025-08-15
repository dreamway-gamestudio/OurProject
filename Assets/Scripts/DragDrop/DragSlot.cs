using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour, IDropHandler
{
    public static bool _onDrop = false;
    Inventory Inventory;
    GameObject DiceForDrag;
    DiceInfoPanel DiceInfoPanel;
    InfoPanel InfoPanel;
    GameObject Finding_GameObject;
    DragDice DragDice;
    public static string ParentName;
    void Start()
    {
        Inventory = GameObject.FindObjectOfType<Inventory>();
        DiceInfoPanel = GameObject.FindObjectOfType<DiceInfoPanel>();
        InfoPanel = GameObject.FindObjectOfType<InfoPanel>();
        DragDice = GameObject.FindObjectOfType<DragDice>();
    }
    public void MouseUp()
    {
        try
        {
            GameObject ActiveDice = GameObject.Find(PlayerPrefs.GetString("ActiveDice"));
            //print("_isDrag " + DragDice._isDrag);
            //print("_returning " + ActiveDice.GetComponent<DragDice>()._returning);
            if (!DragDice._isDrag && !ActiveDice.GetComponent<DragDice>()._returning)
            {
                if (DiceInfoPanel._isDiceUsed)
                {
                    _onDrop = true;
                    GameObject DiceInfo = GameObject.Find(PlayerPrefs.GetString($"{ActiveDice.gameObject.name}_pos"));
                    DiceInfo.GetComponent<DiceInfoPanel>().HideButtons();
                    ActiveDice.transform.SetParent(transform);
                    ActiveDice.transform.localPosition = Vector3.zero;


                    PlayerPrefs.SetString($"Dice{gameObject.name[4]}", ActiveDice.gameObject.name);
                    Rakirovka();
                    DiceInfoPanel._isDiceUsed = false;
                    PlayerPrefs.SetString("ActiveDice","null");
                }
                else if (!DiceInfoPanel._isDiceUsed)
                {
                    OpenInfoPanel();

                }
            }
        }
        catch
        {
            OpenInfoPanel();
        }
    }
    void OpenInfoPanel()
    {
		PlayerPrefs.SetString("InfoPanelOpened", "DragSlot");
		ParentName = PlayerPrefs.GetString(transform.GetChild(0).name + "_pos", "");
        InfoPanel.OpenPanel();
    }
    public void OnDrop(PointerEventData eventData)
    {
        //DiceInfoPanel.HidePower();
        //DiceInfoPanel.DiceButton.GetComponent<Image>().raycastTarget = true;
        //eventData.pointerDrag.gameObject.GetComponent<DragDice>().UseButton_interactible();
        DiceInfoPanel._isDiceUsed = false;
        _onDrop = true; // sax harcy sa er vor hanel eir, gas zangi you are here? yes, can i call you?
        if (eventData.pointerDrag != null && eventData.pointerDrag.transform)
        {
            try
            {
                GameObject DiceInfo = GameObject.Find(PlayerPrefs.GetString($"{eventData.pointerDrag.transform.gameObject.name}_pos"));
                DiceInfo.GetComponent<DiceInfoPanel>().HideButtons();
                //eventData.pointerDrag.transform.gameObject.GetComponent<DiceInfoPanel>();
                eventData.pointerDrag.GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 200f);
                eventData.pointerDrag.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector2(0f, -30f);
                eventData.pointerDrag.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 20f);
                eventData.pointerDrag.transform.GetChild(0).GetComponent<Image>().enabled = true;
                PlayerPrefs.SetString($"Dice{gameObject.name[4]}", eventData.pointerDrag.transform.gameObject.name);
                var otherDiceTransform = eventData.pointerDrag.transform;
                otherDiceTransform.transform.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                otherDiceTransform.SetParent(transform);
                otherDiceTransform.localPosition = Vector3.zero;
                print("on drop with drag");
            }
            catch { }
        }
        if (transform.childCount > 1) //rakirovka
        {
            Rakirovka();
        }
    }

    void Rakirovka()
    {
        string parentFind = PlayerPrefs.GetString($"{transform.GetChild(0).name}_pos");
        DiceForDrag = GameObject.Find(parentFind);
        GameObject DiceReposition = transform.GetChild(0).gameObject;
        /*   Nor dice   */

        transform.GetChild(1).gameObject.GetComponent<Image>().raycastTarget = false;
        transform.GetChild(1).gameObject.GetComponent<Image>().enabled = true;
        transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(200f, 200f);
        transform.GetChild(1).transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector2(0f, -30f);
        transform.GetChild(1).transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 20f);
        transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().enabled = true;
        //transform.GetChild(1).gameObject.GetComponent<DragDice>().UseButton_interactible();

        /*   Het gnacox dice   */

        GameObject DiceParent = GameObject.Find(PlayerPrefs.GetString($"{DiceReposition.name}_pos"));
        for (int i = 0; i <= DiceParent.transform.childCount - 1; i++)
        {
            if (DiceParent.transform.GetChild(i).name == "Buttons")
            {
                DiceParent.transform.GetChild(i).transform.GetChild(1).GetComponent<Button>().interactable = true;
            }
        }
        DiceReposition.GetComponent<Image>().enabled = false;
        DiceReposition.GetComponent<Image>().raycastTarget = true;
        DiceReposition.transform.SetParent(DiceForDrag.transform);
        DiceReposition.GetComponent<RectTransform>().sizeDelta = new Vector2(260f, 260f);
        DiceReposition.transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector2(0f, -40f);
        DiceReposition.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(25f, 25f);
        DiceReposition.transform.GetChild(0).GetComponent<Image>().enabled = false;

        GameObject DiceButton;
        for (int i = 0; i <= DiceForDrag.transform.childCount - 1; i++)
        {
            if (DiceForDrag.transform.GetChild(i).name == "DiceButton")
            {
                DiceButton = DiceForDrag.transform.GetChild(i).gameObject;
                DiceReposition.transform.localPosition = DiceButton.transform.localPosition;
                break;
            }
        }
        Inventory.InventoryClass();
    }
}
