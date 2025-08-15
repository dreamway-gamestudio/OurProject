using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseElixirAnimation : MonoBehaviour
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
        if (SwipeMenu._elixir)
        {
            animator.Play("UseElixirOpen");
        }
        else
        {
            animator.Play("UseElixirClose");
        }
    }
}
