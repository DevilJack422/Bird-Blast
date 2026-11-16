using UnityEngine;

public class TntBox : MonoBehaviour, IDamageable
{
    [Header("-----Health & Points-----")]
    [SerializeField] private float health = 10f;
    [SerializeField] private int pointValue = 500;

    [Header("-----Impact-----")]
    [SerializeField] private float minImpact = 3f;
    [SerializeField] private float impactMultiplier = 1.5f;
    [SerializeField] private float graceTime = 1f;

    [Header("-----Explosive-----")]
    [SerializeField] private float radius = 2.2f;
    [SerializeField] private float explosionDamage = 60f;
    [SerializeField] private float force = 7f;

    [Header("-----System-----")]
    [SerializeField] private GameObject explosiveEffect;
    [SerializeField] private string soundGroup = "Effect";
    [SerializeField] private string explosiveSound = "Explosive";

    private bool isExplosed;
    private float spawnTime;

    void Awake() => spawnTime = Time.time;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isExplosed || Time.time - spawnTime < graceTime) return;

        float impact = collision.relativeVelocity.magnitude;
        if (impact < minImpact) return;

        TakeDamage((impact - minImpact) * impactMultiplier);
    }

    public void TakeDamage(float amount)
    {
        if (isExplosed) return;

        health -= amount;
        if (health <= 0) Explode();
    }

    private void Explode()
    {
        if (isExplosed) return;
        isExplosed = true;

        if (ScoreManager.Instance != null) ScoreManager.Instance.AddCombo(pointValue);
        Explosion.Create(transform.position, radius, explosionDamage, force, gameObject);

        if (explosiveEffect != null)
        {
            Instantiate(explosiveEffect, transform.position, Quaternion.identity);
        }

        if (!string.IsNullOrEmpty(soundGroup) && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(soundGroup, explosiveSound);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
