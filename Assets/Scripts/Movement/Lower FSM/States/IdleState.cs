
using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(PlayerControllerLower player, Animator animator) : base(player, animator){ }

    public override void OnEnter()
    {
        animator.CrossFade(IdleHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        //
    }

    

}
