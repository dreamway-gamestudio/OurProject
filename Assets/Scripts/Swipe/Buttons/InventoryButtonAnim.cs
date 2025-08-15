using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButtonAnim : MonoBehaviour
{
    Animator animator;
    SwipeMenu SwipeMenu;
    void Start()
    {
        animator = GetComponent<Animator>();
        SwipeMenu = FindObjectOfType<SwipeMenu>();
    }
    void Update()
    {
        if (SwipeMenu._inventory)
        {
            animator.Play("InventoryOpen");
        }
        else
        {
            animator.Play("InventoryClose");
        }
    }
}
