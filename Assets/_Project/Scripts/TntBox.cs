using UnityEngine;

public class TntBox : MonoBehaviour, IDamageable
{
    [Header("-----Health & Points-----")]
    [SerializeField] private float health = 10f;
    [SerializeField] private int pointValue = 500;

    [Header("-----Explosive-----")]
    [SerializeField] private float radius = 2.2f;
    [SerializeField] private float damage = 60f;
    [SerializeField] private float force = 7f;

    [Header("-----System-----")]
    [SerializeField] private GameObject explosiveEffect;
    [SerializeField] private string soundGroup = "Effect";
    [SerializeField] private string explosiveSound = "Explosive";

    private bool isExplosed;

    public void TakeDamage(float damage)
    {
        if (isExplosed) return;
        health -= damage;
        if (health <= 0)
        {
            Explode();
            return;
        }
    }

    private void Explode()
    {
        Explosion.Create(transform.position, radius, damage, force, gameObject);

        if (explosiveEffect != null)
        {
            Instantiate(explosiveEffect, transform.position, Quaternion.identity);
        }

        if (!string.IsNullOrEmpty(soundGroup) && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(soundGroup, explosiveSound);
        }
        isExplosed = true;
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
