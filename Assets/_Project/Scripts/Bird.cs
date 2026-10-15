using System;
using UnityEditor.Callbacks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour
{
    public event Action<Bird> OnFinished;

    [Header("-----End Turn-----")]
    [SerializeField] private float stopSpeed = 0.3f; // Speed under this number mean stop
    [SerializeField] private float stopTime = 1f; // How long standing still before it ends
    [SerializeField] private float maxFlightTime = 8f; // Safe: Flight too long ends
    [SerializeField] private float killY = -10f; // Fall under this point is end

    private Rigidbody2D rb;
    private float flightTimer, slowTimer;
    private bool isFinishd;

    public bool IsLaunched { get; private set; }
    public Vector2 GravityVector => Physics2D.gravity * rb.gravityScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; //stay still on slingshot
    }

    void Update()
    {
        if (!IsLaunched || isFinishd) return;

        flightTimer = Time.deltaTime;
        slowTimer = rb.linearVelocity.magnitude < stopSpeed ? slowTimer + Time.deltaTime : 0f;

        if (slowTimer >= stopTime || flightTimer >= maxFlightTime || transform.position.y < killY)
        {
            Finish();
        }
    }

    public void Launch(Vector2 velocity)
    {
        IsLaunched = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = velocity;
    }

    private void Finish()
    {
        isFinishd = true;
        OnFinished?.Invoke(this);
        //TODO: Spawn VFX mist
        Destroy(gameObject, 0.2f);
    }
}
