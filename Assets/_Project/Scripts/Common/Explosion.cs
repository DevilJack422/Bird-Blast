using System.Collections.Generic;
using UnityEngine;

public static class Explosion
{
    public static void Create(Vector2 pos, float radius, float maxDamage, float maxForce, GameObject self = null)
    {
        var damaged = new HashSet<IDamageable>();

        foreach (var hit in Physics2D.OverlapCircleAll(pos, radius))
        {
            if (self != null && hit.gameObject == self) continue;

            float dist = Vector2.Distance(pos, hit.ClosestPoint(pos));
            float fallof = 1f - Mathf.Clamp01(dist / radius);

            if (hit.TryGetComponent<IDamageable>(out var dmg) && damaged.Add(dmg))
            {
                dmg.TakeDamage(maxDamage * fallof);
            }

            var body = hit.attachedRigidbody;
            if (body != null && body.bodyType == RigidbodyType2D.Dynamic)
            {
                Vector2 dir = ((Vector2)body.worldCenterOfMass - pos).normalized;
                body.AddForce(dir * maxForce * fallof, ForceMode2D.Impulse);
            }
        }
    }
}
