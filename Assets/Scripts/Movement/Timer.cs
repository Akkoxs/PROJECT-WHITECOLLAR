
using System;

public abstract class Timer
{
    protected float initialTime; 
    protected float Time {get; set; }
    public bool IsRunning {get; protected set;}

    public Action OnTimerStart = delegate{ }; //actions are delegates in Unity that return void 
    public Action OnTimerStop = delegate{ };

    protected Timer (float value)
    {
        initialTime = value;
        IsRunning = false;
    }

    public void Start()
    {
        Time = initialTime;
        if (!IsRunning)
        {
            IsRunning = true;
            OnTimerStart.Invoke();
        }
    }

    public void Stop()
    {
        if (IsRunning)
        {
            IsRunning = false;
            OnTimerStop.Invoke();
        }
    }

    public void Resume() => IsRunning = true;

    public void Pause() => IsRunning = false;

    public abstract void Tick(float deltaTime);

}

//2 types of timers

//Type 1 :: cooldown timer 
public class CountdownTimer : Timer
{
    public CountdownTimer(float value) : base(value) { } //constructor (passed from other constructor)

    public override void Tick(float deltaTime)
    {
        if (IsRunning && Time > 0)
        {
            Time -= deltaTime;
        }

        if (IsRunning && Time <= 0)
        {
            Stop();
        }
    }
    
    public bool IsFinished => Time <= 0;

    //method overloading
    public void Reset() => Time = initialTime; //v1, resets the time back to initial

    public void Reset(float newTime) //v2, resets the time back to a new value 
    {
        initialTime = newTime; 
        Reset();
    }
}

//Type 2 :: stopwatch timer
public class StopwatchTimer : Timer
{
    public StopwatchTimer() : base(0) { }

    public override void Tick(float deltaTime)
    {
        if (IsRunning)
        {
            Time += deltaTime;
        }
    }

    public void Reset() => Time = 0;

    public float GetTime() => Time;
}





