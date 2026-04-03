using UnityEngine;

public class DashState : BaseState<PlayerControllerLower>
{
    public DashState(PlayerControllerLower player, Animator animator) : base(player, animator){ }

    public override void OnEnter()
    {
        animator.CrossFade(DashHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        player.HandleDashing();
    }
}
