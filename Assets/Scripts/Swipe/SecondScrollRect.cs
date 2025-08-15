using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(ScrollRect))]
public class SecondScrollRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    //[HideInInspector] public ScrollRect OtherScrollRect;
    [HideInInspector] public ScrollRect _myScrollRect;
    //Это отслеживает, должен ли другой прокручиваться вместо текущего.
    private bool scrollOther;
    //Это отслеживает, должен ли другой прокручиваться по горизонтали или вертикали.
    private bool scrollOtherHorizontally;
    SwipeMenu SwipeMenu;
    [HideInInspector] public bool _beginDrag = false;
    void Start()
    {
        SwipeMenu = GameObject.FindObjectOfType<SwipeMenu>();
    }

    void Awake()
    {
        //Получите текущий прямоугольник прокрутки, чтобы мы могли отключить его, если другой прокручивается
        _myScrollRect = this.GetComponent<ScrollRect>();
        
        //Если в текущем прямоугольнике прокрутки отмечена вертикальная прокрутка, тогда другая будет прокручиваться по горизонтали.
        scrollOtherHorizontally = _myScrollRect.vertical;
        //Проверьте некоторые атрибуты, чтобы сообщить пользователю, работает ли это не так, как ожидалось.
    }
    //IBeginDragHandler
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Получите абсолютные значения разностей x и y, чтобы мы могли увидеть, какой из них больше, и соответственно прокрутить другой прямоугольник прокрутки.
        float horizontal = Mathf.Abs(eventData.position.x - eventData.pressPosition.x);
        float vertical = Mathf.Abs(eventData.position.y - eventData.pressPosition.y);
        if (scrollOtherHorizontally)
        {
            if (horizontal > vertical)
            {
                scrollOther = true;
                //отключить текущий прямоугольник прокрутки, чтобы он не двигался.
                _myScrollRect.enabled = false;

                SwipeMenu.OnBeginDrag(eventData);
            }
        }
        else if (vertical > horizontal)
        {
            scrollOther = true;
            //отключить текущий прямоугольник прокрутки, чтобы он не двигался.
            _myScrollRect.enabled = false;
            SwipeMenu.OnBeginDrag(eventData);
        }

    }
    //IEndDragHandler
    public void OnEndDrag(PointerEventData eventData)
    {
        if (scrollOther)
        {
            scrollOther = false;
            _myScrollRect.enabled = true;
            SwipeMenu.OnEndDrag(eventData);
        }
    }
    //IDragHandler
    public void OnDrag(PointerEventData eventData)
    {
        if (scrollOther)
        {
            SwipeMenu.OnDrag(eventData);
        }
    }
}