using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour
{
    public event Action<Bird> OnFinished;

    [Header("-----End Turn-----")]
    [SerializeField] private float stopSpeed = 0.3f; // Speed under this number mean stop
    [SerializeField] private float stopTime = 1f; // How long standing still before it end 
    [SerializeField] private float maxFlightTime = 8f; // Safe: Flight too long ends
    [SerializeField] private float killY = -10f; // Fall under this point is end

    [Header("-----Ablity-----")]
    [SerializeField] private float abilityDelay = 0.1f;
    [SerializeField] private string abilitySoundName = "";

    protected Rigidbody2D rb;
    private float flightTimer, slowTimer;
    private bool isFinishd;

    public bool IsLaunched { get; private set; }
    public bool AbilityUsed { get; private set; }
    public Vector2 GravityVector => Physics2D.gravity * rb.gravityScale;

    protected virtual bool HasAbility =>  false;
    protected virtual void UseAbility() { }
    protected virtual bool CanFinish() => true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; //stay still on slingshot
    }

    void Update()
    {
        if (!IsLaunched || isFinishd) return;
       
        flightTimer = Time.deltaTime;
        HandleAbilityInput();

        slowTimer = rb.linearVelocity.magnitude < stopSpeed ? slowTimer + Time.deltaTime : 0f;

        bool shouldFinish = slowTimer >= stopTime
                            || flightTimer >= maxFlightTime
                            || transform.position.y < killY;

        if (shouldFinish && CanFinish()) Finish();
    }

    public void Launch(Vector2 velocity)
    {
        IsLaunched = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = velocity;
    }

    protected void Finish()
    {
        isFinishd = true;
        OnFinished?.Invoke(this);
        //TODO: Spawn VFX mist
        Destroy(gameObject, 0.2f);
    }

    private void HandleAbilityInput()
    {
        if (Pointer.current == null)
        {
            Debug.Log("[Abillity] Pointer.current = null");
            return;
        }

        if (!Pointer.current.press.wasPressedThisFrame) return;
        Debug.Log($"[Ability] click! type={GetType().Name} HasAbility={HasAbility} used={AbilityUsed} t={flightTimer:F2} timeScale={Time.timeScale}");

        if (!HasAbility || AbilityUsed || flightTimer < abilityDelay) return;
        if (Time.timeScale == 0 || Pointer.current == null) return;
        
        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        Debug.Log($"[Ability] overUI={overUI}");
        if (overUI) return;

        TriggerAbility();
    }

    protected void TriggerAbility()
    {
        if (AbilityUsed) return;
        AbilityUsed = true;
        if (!string.IsNullOrEmpty(abilitySoundName) && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound("Bird", abilitySoundName);
        }
        UseAbility();
    }
}
