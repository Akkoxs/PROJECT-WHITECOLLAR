//All from https://youtu.be/--_CH5DYz0M?si=6o_Hs2S42OX07Nvc
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerLower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;
    [SerializeField] GroundChecker groundChecker;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] InputReader input; 

    [Header("Mouse Settings")]
    [SerializeField] float rotationSpeed = 3f;

    [Header("Walk Settings")]
    [SerializeField] float walkSpeed = 300f; 

    [Header("Run Settings")]
    [SerializeField] float runSpeed = 600f;

    [Header("Dash Settings")]
    [SerializeField] float dashSpeed = 2000f;

    //TEMPORARY
    [SerializeField] float runDuration = 2.0f;
    [SerializeField] float runCooldown = 0.5f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 0.5f;
    
    Camera mainCam; 

    //also probably mouse aiming 
    Vector3 movement;
    Vector3 adjustedDirection;

    //mouse aiming 
    Vector3 mouse;
    Quaternion lookRotation;
    Vector3 directionTarget;
    
    //timers
    List<Timer> timers;
    //temproary
    CountdownTimer runTimer;
    CountdownTimer runCooldownTimer; 
    CountdownTimer dashTimer; 
    CountdownTimer dashCooldownTimer;

    StateMachine stateMachine; 
    public string CurrentStateName => stateMachine.CurrentState.GetType().Name;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        groundChecker = GetComponent<GroundChecker>();

        //camera stuff
        mainCam = Camera.main;
        rb.freezeRotation = true; //so player does not tip over

        //Setup timers
        runTimer = new CountdownTimer(runDuration);
        runCooldownTimer = new CountdownTimer(runCooldown);
        dashTimer = new CountdownTimer(dashDuration); 
        dashCooldownTimer = new CountdownTimer(dashCooldown);
        timers = new List<Timer> {runTimer, runCooldownTimer, dashTimer, dashCooldownTimer};

        //When timer stop action is invoked on the runtimer, immediatly start the cooldown timer
        runTimer.OnTimerStop += () => runCooldownTimer.Start(); //+= adds listener to an event/action () => is the lambda operator (a func. that takes no params. and runs .Start()
        dashTimer.OnTimerStop += () => dashCooldownTimer.Start();

        //State Machine
        stateMachine = new StateMachine();

        //Declare states
        var idleState = new IdleState(player: this, animator);
        var walkState = new WalkState(player: this, animator);
        var runState = new RunState(player: this, animator);
        var dashState = new DashState(player: this, animator);

        //Define transitions--------------------------------------------------------------------------------------------------------------
        //walking
        At(idleState, walkState, new FuncPredicate(() => movement.magnitude > 0f)); 
        At(walkState, idleState, new FuncPredicate(() => Mathf.Approximately(movement.magnitude, 0f))); 
        //running
        At(walkState, runState, new FuncPredicate(() => movement.magnitude > 0f && runTimer.IsRunning));
        At(runState, walkState, new FuncPredicate(() => movement.magnitude > 0f && !runTimer.IsRunning));
        //dashing 
        At(idleState, dashState, new FuncPredicate(() => dashTimer.IsRunning));
        At(dashState, idleState, new FuncPredicate(() => !dashTimer.IsRunning && Mathf.Approximately(movement.magnitude, 0f)));
        At(walkState, dashState, new FuncPredicate(() => dashTimer.IsRunning && movement.magnitude > 0f));
        At(dashState, walkState, new FuncPredicate(() => !dashTimer.IsRunning && movement.magnitude > 0f));
        At(runState, dashState, new FuncPredicate(() => dashTimer.IsRunning && movement.magnitude > 0f && runTimer.IsRunning));
        At(dashState, runState, new FuncPredicate(() => !dashTimer.IsRunning && movement.magnitude > 0f && runTimer.IsRunning));
        //Any(idleState, new FuncPredicate(ReturnToIdleState));
        //-------------------------------------------------------------------------------------------------------------------------------

        //Initial State
        stateMachine.SetState(idleState);
    }

    void Start() => input.EnablePlayerActions();

    void OnEnable()
    {
        input.Run += OnRun;
        input.Dash += OnDash;
    }

    void OnDisable()
    {
        input.Run -= OnRun;
        input.Dash -= OnDash;
    }

    void Update()
    {
        movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
        stateMachine.Update();
        HandleTimers();
    }

    void FixedUpdate()
    {
        HandleAiming();
        stateMachine.FixedUpdate();
    }

    public void HandleAiming()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,groundLayer))
        {
            mouse = hit.point;
        }
        else
        {
            //fallback if a ground not detected to intersect at a plane at the player Y pos. 
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            if (groundPlane.Raycast(ray, out float distance))
            {
                mouse = ray.GetPoint(distance);
            }
        }

        directionTarget = new Vector3(mouse.x - transform.position.x, 0f, mouse.z - transform.position.z).normalized;
        
        if (directionTarget != Vector3.zero) // guard against zero vector
        {
            lookRotation = Quaternion.LookRotation(directionTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
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
        adjustedDirection = Quaternion.AngleAxis(mainCam.transform.eulerAngles.y, Vector3.up) * movement; //for movement calcs
        Vector3 velocity = adjustedDirection * walkSpeed * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    public void HandleRunning()
    {
        adjustedDirection = Quaternion.AngleAxis(mainCam.transform.eulerAngles.y, Vector3.up) * movement; //for movement calcs
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

    public void HandleDashing()
    {
        Vector3 dashDirection;

        if (movement.magnitude > 0f)
        {
            dashDirection = Quaternion.AngleAxis(mainCam.transform.eulerAngles.y, Vector3.up) * movement;
        }
        else
        {
            dashDirection = transform.forward;
        }

        Vector3 velocity = dashDirection.normalized * dashSpeed * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    void OnDash(bool performed)
    {
        if (performed && !dashTimer.IsRunning && !dashCooldownTimer.IsRunning && groundChecker.IsGrounded)
        {
            dashTimer.Start();
        }
        else if (!performed && dashTimer.IsRunning) //if cancelled and it was running, stop it
        {
            dashTimer.Stop();
        }
    }

    void HandleTimers(){
        foreach (var timer in timers){
            timer.Tick(Time.deltaTime);
        }
    }

    //Helper methods 
    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState from, IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);



}
