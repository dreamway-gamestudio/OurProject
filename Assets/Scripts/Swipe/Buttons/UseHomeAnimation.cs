using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseHomeAnimation : MonoBehaviour
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
            animator.Play("UseHomeOpen");
            SwipeMenu._home = false;
        }
        else
        {
            animator.Play("UseHomeClose");
        }
    }
}
