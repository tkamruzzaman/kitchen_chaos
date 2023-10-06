using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private Player player;

    private Animator animator;
    private const string IS_WALKING = "IsWalking";

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner) { return; }

        if (animator == null) { return; }
        if (player == null) { return; }

        animator.SetBool(IS_WALKING, player.IsWalking);
    }
}
