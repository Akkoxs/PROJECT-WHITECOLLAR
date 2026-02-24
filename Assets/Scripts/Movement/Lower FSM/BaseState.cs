using UnityEngine;

public abstract class BaseState: IState {
    protected readonly PlayerControllerLower player; //all states will need to know abt player
    protected readonly Animator animator;

    //ints that represent the respective animations in the animator 
    protected static readonly int IdleHash = Animator.StringToHash(name:"Idle"); 
    protected static readonly int WalkHash = Animator.StringToHash(name:"Walk"); 
    protected static readonly int RunHash = Animator.StringToHash(name:"Run"); 
    protected static readonly int JumpHash = Animator.StringToHash(name:"Jump");

    //Time to transition between animations 
    protected const float crossFadeDuration = 0.1f; 

    //constructor, we pass a reference to the player controller/animator to any class that derives from the base state
    protected BaseState(PlayerControllerLower player, Animator animator){
        this.player = player;
        this.animator = animator;
    }

    public virtual void OnEnter()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void OnExit()
    {

    }
    
}