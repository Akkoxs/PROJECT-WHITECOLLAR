using UnityEngine;

public class ChargeState : BaseState<PlayerControllerUpper>
{
    public ChargeState(PlayerControllerUpper player, Animator animator) : base(player, animator){ }

    public override void OnEnter()
    {
        player.ResetCharge();
        player.ShowChargeUI(true);
        animator.CrossFade(ChargeHash, crossFadeDuration);
    }

    public override void Update()
    {
        player.TickCharge();
    }

    public override void OnExit()
    {
        player.ShowChargeUI(false);
    }

}
