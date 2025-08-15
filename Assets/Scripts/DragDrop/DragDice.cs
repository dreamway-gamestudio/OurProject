using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragDice : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    public bool _returning;
    private Vector3 _currentPosition;
    DragSlot DragSlot;
    [HideInInspector]
    public Transform StartTransform;
    DiceInfoPanel DiceInfoPanel;
    Inventory Inventory;
    public static string diceStartParentName;
    public Vector3 StartPos;
    public bool _isUsed = false;
    GameObject objectBeingDragged;
    public static bool _isDrag = false;


    void Start()
    {

        //_currentPosition = transform.position;

        StartPos = transform.position;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponent<Canvas>();
        DragSlot = GameObject.FindObjectOfType<DragSlot>();
        DiceInfoPanel = GameObject.FindObjectOfType<DiceInfoPanel>();
        canvas.overrideSorting = false;

        
    }
    void Awake()
    {
        Input.multiTouchEnabled = false;
    }
    private void Update()
    {
        if (_returning)
        {
            ReturnPosition();
        }
        UseButton_interactible();
    }
    public void UseButton_interactible()
    {
        if (gameObject.transform.parent.name.Substring(0, 4) == "Item" && !_isUsed)
        {

            GameObject DiceParent = GameObject.Find(PlayerPrefs.GetString($"{gameObject.name}_pos"));
            for (int i = 0; i <= DiceParent.transform.childCount - 1; i++)
            {
                if (DiceParent.transform.GetChild(i).name == "Buttons")
                {
                    DiceParent.transform.GetChild(i).transform.GetChild(1).GetComponent<Button>().interactable = false;
                    
                }
            }
            _isUsed = true;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_returning)
        {
            _currentPosition = transform.localPosition;
        }
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, 1f);
        canvasGroup.blocksRaycasts = false;
        DragSlot._onDrop = false;
        canvas.overrideSorting = true;
        
        var slotTransform = rectTransform.parent;
        slotTransform.SetAsLastSibling();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!_returning)
        {
            _isDrag = true;
            gameObject.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        }
        //Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        //print("1 drag");
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //gameObject.GetComponent<Canvas>().overrideSorting = true;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        _isDrag = false;
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        canvasGroup.blocksRaycasts = true;

        if (!DragSlot._onDrop)
        {
            _returning = true;
            canvas.overrideSorting = false;
        }
        else
        {
            canvas.overrideSorting = false;
        }
    }
    public void ReturnPosition() // Dice i plavni het gnal
    {

        if (Vector2.Distance(transform.localPosition, _currentPosition) > 0.1f)
        {
            transform.Translate((_currentPosition - transform.localPosition) * 0.025f);
        }
        else
        {
            _returning = false;
            transform.localPosition = _currentPosition;
        }
    }
}
