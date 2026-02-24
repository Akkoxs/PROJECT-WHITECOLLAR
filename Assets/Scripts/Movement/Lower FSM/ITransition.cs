
//defines condition that must be met to move to another state 
public interface ITransition
{
    IState TargetState {get;} //which state we move to 
    IPredicate Condition {get;} //based on x condition 
}
