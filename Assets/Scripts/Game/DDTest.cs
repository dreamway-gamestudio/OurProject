using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDTest : MonoBehaviour
{
    private Vector3 _currentPosition;
    private bool _drag = false;
    private bool _returning;
    void Update()
    {
        if (_returning)
        {
            ReturnPosition();
        }
    }
    private void OnMouseDown()
    {
        _currentPosition = transform.position;
        //print("BeginDrag");
    }
    private void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = objPosition;
        _drag = true;
        //print("Dragging");
    }
    private void OnMouseUp()
    {
        _drag = false;
        _returning = true;
        //print("End Drag");
    }
    private void ReturnPosition() // Dice i plavni het gnal
    {
        if (Vector2.Distance(transform.position, _currentPosition) > 0.1f)
        {
            transform.Translate((_currentPosition - transform.position) * 0.4f);
        }
        else
        {
            transform.position = _currentPosition;
           // _returning = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Slot"))
        {
            print("jamshud");
        } else if(other.CompareTag("Slot1"))
        {
            print("jamshud1");
        }
        
    }
}
