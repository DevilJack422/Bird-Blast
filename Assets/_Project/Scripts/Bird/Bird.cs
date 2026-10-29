using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour
{
    public event Action<Bird> OnFinished;
    public event Action<Bird> OnFirstHit;

    [Header("-----End Turn-----")]
    [SerializeField] private float lifeAfterHit = 3f; // After the first hit, how long the bird live
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float maxFlightTime = 8f; // Safe: Flight too long ends
    [SerializeField] private float killY = -10f; // Fall under this point is end
    [SerializeField] private GameObject smokeEffect;

    [Header("-----Ablity-----")]
    [SerializeField] private float abilityDelay = 0;
    [SerializeField] private string abilitySoundName = "";

    protected Rigidbody2D rb;
    private float flightTimer, hitTimer;
    private bool isFinishd, hasHit;

    public bool IsLaunched { get; private set; }
    public bool AbilityUsed { get; private set; }
    public Vector2 GravityVector => Physics2D.gravity * rb.gravityScale;

    protected virtual bool HasAbility => false;
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

        flightTimer += Time.deltaTime;
        if (hasHit) hitTimer += Time.deltaTime;

        HandleAbilityInput();

        bool shouldFinish = (hasHit && hitTimer >= lifeAfterHit)
                            || flightTimer >= maxFlightTime
                            || transform.position.y < killY;

        if (shouldFinish && CanFinish()) Finish();
    }

    protected virtual void OnCollisionEnter2D(Collision2D other) 
    {
        if (!IsLaunched || hasHit || isFinishd || flightTimer < 0.1f) return;
        hasHit = true;
        OnFirstHit?.Invoke(this);    
    }

    public void Launch(Vector2 velocity)
    {
        IsLaunched = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = velocity;
    }

    protected void Finish()
    {
        if (isFinishd) return;
        isFinishd = true;
        OnFinished?.Invoke(this);
        if (smokeEffect != null) Instantiate(smokeEffect, transform.position, Quaternion.identity);
        StartCoroutine(FadeAndDestroy());
    }

    private void HandleAbilityInput()
    {
        if (!Pointer.current.press.wasPressedThisFrame) return;
        if (!HasAbility || AbilityUsed || flightTimer < abilityDelay) return;
        if (Time.timeScale == 0 || Pointer.current == null) return;

        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
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

    private IEnumerator FadeAndDestroy()
    {
        foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = false;
        rb.simulated = false;

        var renderers = GetComponentsInChildren<SpriteRenderer>();
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float a = 1f - t / fadeDuration;
            foreach (var sr in renderers)
            {
                Color c = sr.color; c.a = a; sr.color = c;
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
