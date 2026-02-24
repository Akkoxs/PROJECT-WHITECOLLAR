
//from https://www.youtube.com/watch?v=NnH6ZK5jt7o

//To add a new state
//1. Need an event from the input reader
//2. Need an animation for said state
//3. Need a timer
//4. Need a State
//5. Affect the RB velocity 


using System;
using System.Collections.Generic;

public class StateMachine
{
    StateNode current; 
    Dictionary <Type, StateNode> nodes = new();
    HashSet<ITransition> anyTransitions = new();

    public void Update()
    {
        var transition = GetTransition();
        //if there is something to transition to, then transition to targetState
        if (transition != null)
        {
            ChangeState(transition.TargetState);
        }

        current.State?.Update();
    }

    //runs on fixed timestep (0.02s)
    public void FixedUpdate()
    {
        current.State?.FixedUpdate(); // ?. is the null conditional operator 
    }

    //set state from outside of the machine 
    public void SetState(IState state)
    {
        current = nodes[state.GetType()]; //grab the state type 
        current.State?.OnEnter(); //if we can enter the state, do it 
    }

    void ChangeState(IState state)
    {
        if (state == current.State) return; //if state is tryign to transition into itself, bail out

        var previousState = current.State;
        var nextState = nodes[state.GetType()].State;

        previousState?.OnExit();
        nextState?.OnEnter();
        current = nodes[state.GetType()];
    }

    ITransition GetTransition()
    {
        foreach (var transition in anyTransitions)
            if (transition.Condition.Evaluate())
                return transition; 

        foreach (var transition in current.Transitions)
            if (transition.Condition.Evaluate())
                return transition;

        return null;
    }

    public void AddTransition(IState previousState, IState targetState, IPredicate condition)
    {
        GetOrAddNode(previousState).AddTransition(GetOrAddNode(targetState).State, condition);
    }

    public void AddAnyTransition(IState targetState, IPredicate condition)
    {
        anyTransitions.Add(item: new Transition(GetOrAddNode(targetState).State, condition));
    }

    StateNode GetOrAddNode(IState state)
    {
        var node = nodes.GetValueOrDefault(key:state.GetType());

        if(node == null)
        {
            node = new StateNode(state);
            nodes.Add(state.GetType(), node);
        }

        return node; 
    }

    //represents a state and all of its transition
    //internal class of state machine 
    public class StateNode
    {
        public IState State {get;}
        public HashSet<ITransition> Transitions {get;} //hash sets are tables that have fast lookup, does not allow duplicates and does not keep order. 

        //constructor, ensures a state node always has a state definition 
        public StateNode(IState state)
        {
            State = state;
            Transitions = new HashSet<ITransition>();
        }

        public void AddTransition(IState targetState, IPredicate condition)
        {
            Transitions.Add(item: new Transition (targetState, condition));
        }
    }
}


