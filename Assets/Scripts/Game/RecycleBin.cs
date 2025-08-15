using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleBin : MonoBehaviour
{
    bool _isTrigger = false;
    GameObject Dice;

    void OnTriggerEnter2D(Collider2D other)
    {
        
        Dice = other.gameObject;
        if (other.gameObject.CompareTag("Dice"))
        {
            _isTrigger = true;
            try{
            Physics2D.IgnoreLayerCollision(8,10);
            } catch{}
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        _isTrigger = false;
    }
    void Update()
    {
        if (_isTrigger && Input.GetMouseButtonUp(0))
        {
            Destroy(Dice);
            _isTrigger = false;
        }
    }

}
