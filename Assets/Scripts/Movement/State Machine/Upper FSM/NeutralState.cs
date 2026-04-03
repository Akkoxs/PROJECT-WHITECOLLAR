using UnityEngine;

public class NeutralState : BaseState<PlayerControllerUpper>
{
    public NeutralState(PlayerControllerUpper player, Animator animator) : base(player, animator){ }

    public override void OnEnter()
    {
        player.ResetCharge();
        animator.CrossFade(NeutralHash, crossFadeDuration);
    }
}
