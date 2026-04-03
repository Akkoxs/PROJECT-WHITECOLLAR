using UnityEngine;

public class SwingState : BaseState<PlayerControllerUpper>
{
    
    public SwingState(PlayerControllerUpper player, Animator animator) : base(player, animator){ }

    public override void OnEnter()
    {
        animator.CrossFade(SwingHash, crossFadeDuration);
        player.HandleSwing();
    }
}
