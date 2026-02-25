//All from https://youtu.be/--_CH5DYz0M?si=6o_Hs2S42OX07Nvc
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerControllerLower : MonoBehaviour
{

    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;
    [SerializeField] GroundChecker groundChecker;
    [SerializeField] CinemachineCamera topDownCam;
    [SerializeField] InputReader input; 

    [Header("Walk Settings")]
    [SerializeField] float walkSpeed = 300f; 
    [SerializeField] float rotationSpeed = 15f; 
    [SerializeField] float smoothTime = 0.2f; //for animations 

    [Header("Run Settings")]
    [SerializeField] float runSpeed = 600f;
    //TEMPORARY
    [SerializeField] float runDuration = 2.0f;
    [SerializeField] float runCooldown = 0.5f;
    

    Transform mainCam; 

    Vector3 movement;
    Vector3 adjustedDirection;
    float currentSpeed;
    float velocity;

    List<Timer> timers;
    //TEMPORARY
    CountdownTimer runTimer;
    CountdownTimer runCooldownTimer; 

    StateMachine stateMachine; 

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        groundChecker = GetComponent<GroundChecker>();

        //camera stuff
        mainCam = Camera.main.transform;
        rb.freezeRotation = true; //so player does not tip over

        //Setup timers
        runTimer = new CountdownTimer(runDuration);
        runCooldownTimer = new CountdownTimer(runCooldown);
        timers = new List<Timer> {runTimer, runCooldownTimer};

        //When timer stop action is invoked on the runtimer, immediatly start the cooldown timer
        runTimer.OnTimerStop += () => runCooldownTimer.Start(); //+= adds listener to an event/action () => is the lambda operator (a func. that takes no params. and runs .Start()

        //State Machine
        stateMachine = new StateMachine();

        //Declare states
        var idleState = new IdleState(player: this, animator);
        var walkState = new WalkState(player: this, animator);
        var runState = new RunState(player: this, animator);

        //Define transitions
        At(idleState, walkState, new FuncPredicate(() => movement.magnitude > 0f)); 
        At(walkState, idleState, new FuncPredicate(() => Mathf.Approximately(movement.magnitude, 0f))); 
        At(walkState, runState, new FuncPredicate(() => movement.magnitude > 0f && runTimer.IsRunning));
        At(runState, walkState, new FuncPredicate(() => movement.magnitude > 0f && !runTimer.IsRunning));
        //Any(idleState, new FuncPredicate(ReturnToIdleState));

        //Initial State
        stateMachine.SetState(idleState);
    }

    void Start() => input.EnablePlayerActions();

    void OnEnable()
    {
        input.Run += OnRun;
    }

    void OnDisable()
    {
        input.Run -= OnRun;
    }

    //bool ReturnToIdleState()
    //{
    //    return 
    //}

    void Update()
    {
        movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
        adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
        stateMachine.Update();
        HandleTimers();
    }

    void FixedUpdate()
    {
        HandleRotation();
        stateMachine.FixedUpdate();
    }


    public void HandleRotation()
    {
        if(adjustedDirection == Vector3.zero) return; //only handle rotation when moving

        Quaternion targetRotation = Quaternion.LookRotation(adjustedDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.LookAt(transform.position + adjustedDirection);
    }
    
    public void HandleIdle()
    {
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }

    public void HandleWalking() 
    {
        Vector3 velocity = adjustedDirection * walkSpeed * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    public void HandleRunning()
    {
        Vector3 velocity = adjustedDirection * runSpeed * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    //subscribed to run input action, starts and stops run timers 
    void OnRun(bool performed)
    {
        if (performed && !runTimer.IsRunning && !runCooldownTimer.IsRunning && groundChecker.IsGrounded)
        {
            runTimer.Start();
        }
        else if (!performed && runTimer.IsRunning) //if cancelled and it was running, stop it
        {
            runTimer.Stop();
        }
    }

    void HandleTimers(){
        foreach (var timer in timers){
            timer.Tick(Time.deltaTime);
        }
    }

    //Helper methods 
    void SmoothSpeed(float value) => currentSpeed = Mathf.SmoothDamp(current: currentSpeed, target:value, ref velocity, smoothTime);
    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState from, IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);



}
