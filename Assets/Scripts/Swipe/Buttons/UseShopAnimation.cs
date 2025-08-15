using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseShopAnimation : MonoBehaviour
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
            animator.Play("UseShopOpen");
            SwipeMenu._shop = false;
        }
        else
        {
            animator.Play("UseShopClose");
        }
    }
}
