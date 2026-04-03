using UnityEngine;

public abstract class BaseState<TPlayer>: IState {
    protected readonly TPlayer player; //all states will need to know abt player upper or lower
    protected readonly Animator animator;

    //ints that represent the respective animations in the animator 
    protected static readonly int IdleHash = Animator.StringToHash("Idle"); 
    protected static readonly int WalkHash = Animator.StringToHash("Walk"); 
    protected static readonly int RunHash = Animator.StringToHash("Run"); 
    protected static readonly int DashHash = Animator.StringToHash("Dash");
    protected static readonly int NeutralHash = Animator.StringToHash("Neutral");   
    protected static readonly int ChargeHash = Animator.StringToHash("Charge");
    protected static readonly int SwingHash = Animator.StringToHash("Swing");

    //Time to transition between animations 
    protected const float crossFadeDuration = 0.1f; 

    //constructor, we pass a reference to the player controller/animator to any class that derives from the base state
    protected BaseState(TPlayer player, Animator animator){
        this.player = player;
        this.animator = animator;
    }

    public virtual void OnEnter(){}
    public virtual void Update(){}
    public virtual void FixedUpdate(){}
    public virtual void OnExit(){}
    
}