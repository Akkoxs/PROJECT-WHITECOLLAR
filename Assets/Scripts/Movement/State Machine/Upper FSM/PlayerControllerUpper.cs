//All from https://youtu.be/--_CH5DYz0M?si=6o_Hs2S42OX07Nvc
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerUpper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;
    [SerializeField] GroundChecker groundChecker;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] InputReader input; 
    [SerializeField] Canvas AttackCanvas;

    [Header("Mouse Settings")]
    [SerializeField] float rotationSpeed = 3f;

    [Header("Charge Settings")]
    [SerializeField] public float chargeSpeed = 0.45f;      
    [SerializeField] public float sweetspotMin = 0.7f;  
    [SerializeField] public float sweetspotMax = 0.9f;   
    //[SerializeField] public float movementDebuffMultiplier = 0.5f;

    [Header("Swing Settings")]
    [SerializeField] float swingDuration = 0.4f;
    [SerializeField] float swingCooldown = 0.3f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float attackDamage = 1f;
    [SerializeField] LayerMask enemyLayer;

    [Header("UI Effects")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private Vector3 textOffset = new Vector3(0, 2f, 0);

    PlayerControllerLower playerLower; 

    //timers 
    List<Timer> timers;
    CountdownTimer swingTimer;
    CountdownTimer swingCooldownTimer;

    // Charge value read by UI and states
    public float CurrentCharge { get; private set; }
    public bool AttackHeld { get; private set; }
    public bool IsOvercharged { get; private set; }
    public bool IsInSweetspot => CurrentCharge >= sweetspotMin && CurrentCharge <= sweetspotMax; //expression bodied property isInSweetspot = true if currentcharge is between the min and max, false otherwise 
    
    StateMachine stateMachine; 
    public string CurrentStateName => stateMachine.CurrentState.GetType().Name;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        groundChecker = GetComponent<GroundChecker>();
        playerLower = GetComponent<PlayerControllerLower>();

        //Setup timers
        swingTimer = new CountdownTimer(swingDuration);
        swingCooldownTimer = new CountdownTimer(swingCooldown);
        timers = new List<Timer> {swingTimer, swingCooldownTimer };

        swingTimer.OnTimerStop += () => swingCooldownTimer.Start();

        //State Machine
        stateMachine = new StateMachine();

        //Declare states
        var neutralState = new NeutralState(player: this, animator);
        var chargeState = new ChargeState(player: this, animator);
        var swingState = new SwingState(player: this, animator);

        //Define transitions--------------------------------------------------------------------------------------------------------------
        //neutral
        At(neutralState, chargeState, new FuncPredicate(() => AttackHeld && !swingCooldownTimer.IsRunning));

        //charging
        At(chargeState, swingState, new FuncPredicate(() => !AttackHeld));
        At(chargeState, neutralState, new FuncPredicate(() => IsOvercharged));

        //swinging 
        At(swingState, neutralState, new FuncPredicate(() => swingTimer.IsFinished));

        //-------------------------------------------------------------------------------------------------------------------------------

        //Initial State
        stateMachine.SetState(neutralState);
    }

    void Start() => input.EnablePlayerActions();

    void OnEnable()
    {
        input.Attack += OnAttack;
    }

    void OnDisable()
    {
        input.Attack -= OnAttack;
    }

    void Update()
    {
        stateMachine.Update();
        HandleTimers();
    }

    void FixedUpdate()
    {
        //HandleAiming();
        stateMachine.FixedUpdate();
    }

    void OnAttack (bool performed)
    {
        AttackHeld = performed; // to check if attack button is held down
    }

    public void TickCharge()
    {
        if (IsOvercharged) return;

        CurrentCharge += Time.deltaTime * chargeSpeed;

        //check if we just went past the sweetspot
        if (CurrentCharge > sweetspotMax)
        {
            HandleOvercharge();
        }

        CurrentCharge = Mathf.Clamp01(CurrentCharge);
    }

    private void HandleOvercharge()
    {
        IsOvercharged = true;
        SpawnFloatingText("MISS! OVERCHARGED!");
        
        if (!swingCooldownTimer.IsRunning)
        {
            swingCooldownTimer.Start();
        }
    }

    public void ResetCharge()
    {
        CurrentCharge = 0f;
        IsOvercharged = false;
    }

    public void ShowChargeUI(bool show)
    {
        AttackCanvas.gameObject.SetActive(show);
    }

    public void HandleSwing()//premature release
    {
        if (!IsInSweetspot)
        {
            SpawnFloatingText("MISS! TOO EARLY!");
            return;
        }

        //check for enemies within the sweetspot attack range
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

        float physicalMinRange = sweetspotMin * attackRange;
        float physicalMaxRange = sweetspotMax * attackRange;

        foreach (Collider hit in hits)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, hit.transform.position);
            if (distanceToEnemy >= physicalMinRange && distanceToEnemy <= physicalMaxRange)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                }
            }
        }
    }

    //do we keep here?
    // public void HandleAiming() 
    // {
    //     Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

    //     if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity,groundLayer))
    //     {
    //         mouse = hit.point;
    //     }
    //     else
    //     {
    //         //fallback if a ground not detected to intersect at a plane at the player Y pos. 
    //         Plane groundPlane = new Plane(Vector3.up, transform.position);
    //         if (groundPlane.Raycast(ray, out float distance))
    //         {
    //             mouse = ray.GetPoint(distance);
    //         }
    //     }

    //     directionTarget = new Vector3(mouse.x - transform.position.x, 0f, mouse.z - transform.position.z).normalized;
        
    //     if (directionTarget != Vector3.zero) // guard against zero vector
    //     {
    //         lookRotation = Quaternion.LookRotation(directionTarget);
    //         transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    //     }
    // }

    void HandleTimers(){
        foreach (var timer in timers){
            timer.Tick(Time.deltaTime);
        }
    }

    //Helper methods 
    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState from, IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    private void SpawnFloatingText(string message)
    {
        if (floatingTextPrefab == null) return;

        GameObject textInstance = Instantiate(floatingTextPrefab, transform.position + textOffset, Quaternion.identity);
        FloatingText ft = textInstance.GetComponent<FloatingText>();
        if (ft != null)
        {
            ft.SetText(message);
        }
    }

    void OnDrawGizmos()
    {
        float lineWidth = 0.5f;
        float lineHeight = transform.position.y; // stay at player height

        // yellow line at sweetspot min distance
        Gizmos.color = Color.yellow;
        DrawHorizontalLine(sweetspotMin, lineWidth, lineHeight);

        // red line at sweetspot max distance
        Gizmos.color = Color.red;
        DrawHorizontalLine(sweetspotMax, lineWidth, lineHeight);
    }

    void DrawHorizontalLine(float distance, float width, float height)
    {
        Vector3 center = new Vector3(transform.position.x, height, transform.position.z) 
            + transform.forward * distance * attackRange; // distance along forward

        Vector3 left  = center - transform.right * width;
        Vector3 right = center + transform.right * width;

        Gizmos.DrawLine(left, right);
    }



}
