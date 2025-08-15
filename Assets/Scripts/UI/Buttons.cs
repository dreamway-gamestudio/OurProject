using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    void OnMouseDown() { transform.localScale = new Vector2(0.93f, 0.93f); }
    void OnMouseUp() { transform.localScale = new Vector2(1f, 1f); }
    public void PointerDown()
    {
        print("Down");
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 (250f, 250f);
    }
    public void PointerUp()
    {
        print("Up");
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 (300f, 300f);
    }
}
