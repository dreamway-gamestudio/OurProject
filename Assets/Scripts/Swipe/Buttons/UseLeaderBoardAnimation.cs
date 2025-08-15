using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseLeaderBoardAnimation : MonoBehaviour
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
        if (SwipeMenu._leaderboard)
        {
            animator.Play("UseLeaderBoardOpen");
            SwipeMenu._leaderboard = false;
        }
        else
        {
            animator.Play("UseLeaderBoardClose");
        }
    }
}
