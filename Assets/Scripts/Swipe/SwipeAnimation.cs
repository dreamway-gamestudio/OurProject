using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeAnimation : MonoBehaviour
{
    Animator animator;
    SwipeMenu SwipeMenu;
    void Start()
    {
        animator = GetComponent<Animator>();
        SwipeMenu = FindObjectOfType<SwipeMenu>();
        animator.Play("Home");
    }
    void Update()
    {
        if (SwipeMenu._shop)
        {
            animator.Play("Shop");
            SwipeMenu._shop = false;
        }
        if (SwipeMenu._inventory)
        {
            animator.Play("Inventory");
            SwipeMenu._inventory = false;
        }
        if (SwipeMenu._home)
        {
            animator.Play("Home");
            SwipeMenu._home = false;
        }
        if (SwipeMenu._elixir)
        {
            animator.Play("Elixir");
            SwipeMenu._elixir = false;
        }
        if (SwipeMenu._leaderboard)
        {
            animator.Play("LeaderBoard");
            SwipeMenu._leaderboard = false;
        }
    }
}
