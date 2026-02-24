using UnityEngine;

public class RunState : BaseState
{
    public RunState(PlayerControllerLower player, Animator animator) : base(player, animator){ }
    
    public override void OnEnter()
    {
        animator.CrossFade(RunHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        //
    }
}
