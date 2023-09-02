using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{   
    private Player player;

    private Animator animator;
    private const string IS_WALKING = "IsWalking";   

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (animator != null && player != null)
        {
            print(player.IsWalking);
            animator.SetBool(IS_WALKING, player.IsWalking);
        }
    }
}
