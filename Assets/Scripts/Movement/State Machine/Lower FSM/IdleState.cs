
using UnityEngine;

public class IdleState : BaseState<PlayerControllerLower>
{
    public IdleState(PlayerControllerLower player, Animator animator) : base(player, animator){ }

    public override void OnEnter()
    {
        animator.CrossFade(IdleHash, normalizedTransitionDuration: crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        player.HandleIdle();
    }

    

}
