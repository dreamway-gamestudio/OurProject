using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElixirButtonAnim : MonoBehaviour
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
            animator.Play("ElixirOpen");
            SwipeMenu._elixir = false;
        }
        else
        {
            animator.Play("ElixirClose");
        }
    }
}
