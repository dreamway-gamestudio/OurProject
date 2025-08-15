using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeButtonAnim : MonoBehaviour
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
        if (SwipeMenu._home)
        {
            animator.Play("HomeOpen");
        }
        else
        {
            animator.Play("HomeClose");
        }
    }
}
