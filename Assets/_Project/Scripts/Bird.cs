using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour
{
    Rigidbody2D rb;
    public bool IsLaunched { get; private set; }

    public Vector2 GravityVector => Physics2D.gravity * rb.gravityScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; //stay still on slingshot
    }

    public void Launch(Vector2 velocity)
    {
        IsLaunched = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = velocity;
    }
}
