using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButtonAnim : MonoBehaviour
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
        if (SwipeMenu._shop)
        {
            animator.Play("ShopOpen");
        }
        else
        {
            animator.Play("ShopClose");
        }
    }
}
