using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class SwipeMenu : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 panelLocation, newLocation;
    private float n_location;
    public float percentThreshold = 0.2f;
    public float easing_Swipe;
    public float easing_Tween;
    public int totalPages;
    [HideInInspector]
    public int currentPage = 3;
    public RectTransform PanelHolder;
    [HideInInspector] public bool _shop;
    [HideInInspector] public bool _inventory;
    [HideInInspector] public bool _home;
    [HideInInspector] public bool _elixir;
    [HideInInspector] public bool _leaderboard;
    [HideInInspector] public bool _dragging = false;
    SecondScrollRect SecondScrollRect;
    Inventory Inventory;

    void Start()
    {
        panelLocation = transform.position;
        SecondScrollRect = GameObject.FindObjectOfType<SecondScrollRect>();
        Inventory = GameObject.FindObjectOfType<Inventory>();
    }
    void Update()
    {

        if (currentPage == 1)
        {
            _shop = true;
        }
        if (currentPage == 2)
        {
            _inventory = true;
        }
        if (currentPage == 3)
        {
            _home = true;
        }
        if (currentPage == 4)
        {
            _elixir = true;
        }
        if (currentPage == 5)
        {
            _leaderboard = true;
        }
    }
    public void PanelHolderMove(string button_name)
    {

        int w = Screen.width;
        int h = Screen.height;
        
        if (button_name == "Shop")
        {
            currentPage = 1;
            panelLocation = new Vector3(w / 2 + w * 2, h / 2, 0);
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing_Tween));
            
        }
        else if (button_name == "Inventory")
        {
            currentPage = 2;
            panelLocation = new Vector3(w / 2 + w, h / 2, 0);
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing_Tween));

        }
        else if (button_name == "Home")
        {
            currentPage = 3;
            panelLocation = new Vector3(w / 2, h / 2, 0);
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing_Tween));
        }
        else if (button_name == "Elixir")
        {
            currentPage = 4;
            panelLocation = new Vector3((w / 2) - w, h / 2, 0);
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing_Tween));
            
        }
        else if (button_name == "Leaderboard")
        {
            currentPage = 5;
            panelLocation = new Vector3((w / 2) - w * 2, h / 2, 0);
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing_Tween));

        }

    }
    public void OnBeginDrag(PointerEventData data)
    {

    }
    public void OnDrag(PointerEventData data)
    {
        float difference = data.pressPosition.x - data.position.x;
        transform.position = panelLocation - new Vector3(difference, 0, 0);
        _dragging = true;

    }
    public void OnEndDrag(PointerEventData data)
    {
        
        float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
        if (Mathf.Abs(percentage) >= percentThreshold)
        {
            newLocation = panelLocation;
            if (percentage > 0 && currentPage < totalPages)
            {
                currentPage++;
                newLocation += new Vector3(-Screen.width, 0, 0);

            }
            else if (percentage < 0 && currentPage > 1)
            {
                currentPage--;
                newLocation += new Vector3(Screen.width, 0, 0);
            }
            StartCoroutine(SmoothMove(transform.position, newLocation, easing_Swipe));
            panelLocation = newLocation;
        }
        else
        {
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing_Swipe));
        }
        _dragging = false;
        if (currentPage != 2)
        {
            Inventory.HideButtons_DIP();
        }
    }
    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }
}
