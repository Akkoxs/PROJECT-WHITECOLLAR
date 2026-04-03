using UnityEngine;

public class WalkState : BaseState<PlayerControllerLower>
{
    public WalkState(PlayerControllerLower player, Animator animator) : base(player, animator){ }

    public override void OnEnter()
    {
        animator.CrossFade(WalkHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        player.HandleWalking();
    }
}
