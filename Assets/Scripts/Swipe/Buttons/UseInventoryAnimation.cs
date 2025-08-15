using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseInventoryAnimation : MonoBehaviour
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
            animator.Play("UseInventoryOpen");
            SwipeMenu._inventory = false;
        }
        else
        {
            animator.Play("UseInventoryClose");
        }
    }
}
