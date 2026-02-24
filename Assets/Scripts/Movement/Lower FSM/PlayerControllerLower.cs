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
    [SerializeField] CinemachineCamera freeLookVCam;
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
        animator = GetComponent<Animator>();
        groundChecker = GetComponent<GroundChecker>();

        //camera stuff
        mainCam = Camera.main.transform;
        freeLookVCam.Follow = transform; //setting targets
        freeLookVCam.LookAt = transform; 
        //invoke event when observed transform is teleported, adjusting freelookVCam's position accordingly 
        //positionDelta is a param from Cinemachine which we are passing our transform.position into?
        freeLookVCam.OnTargetObjectWarped(transform, positionDelta: transform.position - freeLookVCam.transform.position - Vector3.forward);

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
        //var idleState = new IdleState(player: this, animator);
        //var walkState = new WalkState(player: this, animator);
        //var runState = new RunState(player: this, animator);

        //Define transitions
        //At(from:idleState, to:walkState, condition:new FuncPredicate() =>  ) 
        //At
        //Any(idleState, new FuncPredicate(ReturnToIdleState));

        //Initial State
        //stateMachine.SetState(idleState);
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
        HandleTimers();
        //stateMachine.Update();
    }

    void FixedUpdate()
    {
        HandleMovement();
        //stateMachine.FixedUpdate();
    }

    public void HandleMovement() 
    {
        //rotate movement direction to match camera rotation 
        Vector3 adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
        if (adjustedDirection.magnitude > 0f)
        {
            HandleRotation(adjustedDirection);
            HandleHorizontalMovement(adjustedDirection);
            SmoothSpeed(adjustedDirection.magnitude);
        }
        else
        {
            SmoothSpeed(0f);

            //reset horizontal velocity for a snappy stop
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }
    
    void HandleHorizontalMovement(Vector3 adjustedDirection) //this actually handles walking?
    {
        if (runTimer.IsRunning)
        {
            Vector3 velocity = adjustedDirection * runSpeed * Time.fixedDeltaTime;
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        }
        else
        {
            Vector3 velocity = adjustedDirection * walkSpeed * Time.fixedDeltaTime;
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        }
        
    }

    void HandleRun(Vector3 adjustedDirection)
    {
        Vector3 velocity = adjustedDirection * runSpeed * Time.fixedDeltaTime;
        if (runTimer.IsRunning)
        {
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.y);
        }
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

    void HandleRotation(Vector3 adjustedDirection)
    {
        //adjusted rotation to match the movement direction
        Quaternion targetRotation = Quaternion.LookRotation(adjustedDirection);
        transform.rotation = Quaternion.RotateTowards(from: transform.rotation, to: targetRotation, maxDegreesDelta: rotationSpeed * Time.deltaTime);
        transform.LookAt(worldPosition: transform.position + adjustedDirection);
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
