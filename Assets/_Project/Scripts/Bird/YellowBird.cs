using UnityEngine;

public class YellowBird : Bird
{
    [SerializeField] private float boost = 2f;
    [SerializeField] private float minSpeed = 14f;
    [SerializeField] private float maxSpeed = 30f;

    protected override bool HasAbility => true;

    protected override void UseAbility()
    {
        Vector2 v = rb.linearVelocity;
        if (v.sqrMagnitude < 0.01f) return;
        float speed = Mathf.Clamp(v.magnitude * boost, minSpeed, maxSpeed);
        rb.linearVelocity = v.normalized * speed;
    }
}
