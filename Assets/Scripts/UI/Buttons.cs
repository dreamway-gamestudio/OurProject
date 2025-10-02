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
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(270f, 105f);

        Transform child = gameObject.transform.Find("ButtonText");
        if (child != null)
        {
            var text = child.GetComponent<Text>();
            if (text != null)
                text.fontSize = 42;
        }
    }
    public void PointerUp()
    {
        print("Up");
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(290f, 110f);

        Transform child = gameObject.transform.Find("ButtonText");
        if (child != null)
        {
            var text = child.GetComponent<Text>();
            if (text != null)
                text.fontSize = 45;
        }
    }

    public void PointerDownDiceField()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);

        Transform child = gameObject.transform.Find("ButtonText");
        if (child != null)
        {
            var text = child.GetComponent<Text>();
            if (text != null)
                text.fontSize = 30;
        }
    }
    public void PointerUpDiceField()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(110f, 100f);

        Transform child = gameObject.transform.Find("ButtonText");
        if (child != null)
        {
            var text = child.GetComponent<Text>();
            if (text != null)
                text.fontSize = 35;
        }
    }
}
