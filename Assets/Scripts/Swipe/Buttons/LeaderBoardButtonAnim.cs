using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardButtonAnim : MonoBehaviour
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
            animator.Play("LeaderBoardOpen");
        }
        else
        {
            animator.Play("LeaderBoardClose");
        }
    }
}
