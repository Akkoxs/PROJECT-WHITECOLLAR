using System;

//a wrapper class that takes a function and turns it into a predicate
public class FuncPredicate : IPredicate
{
    //a delegate that takes no params and returns a bool
    // if you did Func<int, bool> it would take an int and return a bool 
    readonly Func<bool> func; 
    
    //contructor, passes delegate
    public FuncPredicate(Func<bool> func)
    {
        this.func = func;
    }

    //then evaluate the delegate by invoking 
    public bool Evaluate() => func.Invoke();
    // same as below
        // public bool Evaluate()
        // {
        //     return func.Invoke();
        // }
}
