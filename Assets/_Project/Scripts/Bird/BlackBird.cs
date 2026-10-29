using UnityEngine;

public class BlackBird : Bird
{
    [Header("-----Explosive-----")]
    [SerializeField] private float radius = 2.5f;
    [SerializeField] private float damage = 80f;
    [SerializeField] private float force = 8f;
    [SerializeField] private float autoExplodeDelay = 1.5f;
    [SerializeField] private GameObject explosionEffect;

    private bool isHitSomething;
    protected override bool HasAbility => true;

    protected override void UseAbility()
    {
        Explosion.Create(transform.position, radius, damage, force, gameObject);
        SoundManager.Instance.PlaySound("Effect", "Explosion");
        PlayEffect();

        foreach (var sr in GetComponentsInChildren<SpriteRenderer>()) sr.enabled = false;
        foreach (var col in GetComponentsInChildren<Collider2D>()) col.enabled = false;

        Finish();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (!IsLaunched || AbilityUsed || isHitSomething) return;
        isHitSomething = true;
        Invoke(nameof(AutoExplode), autoExplodeDelay);
    }

    private void AutoExplode() => TriggerAbility();

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void PlayEffect()
    {
        if (explosionEffect != null) Instantiate(explosionEffect, transform.position, Quaternion.identity);
    }
}
