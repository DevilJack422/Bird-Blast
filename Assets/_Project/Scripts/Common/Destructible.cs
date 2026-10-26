using System;
using UnityEngine;

public class Destructible : MonoBehaviour, IDamageable
{
    public static event Action<Destructible> Destroyed;

    [Header("-----Health & Point-----")]
    [SerializeField] private float maxHealth = 30f;
    [SerializeField] private int pointValue = 100;

    [Header("-----Impact Damage-----")]
    [SerializeField] private float minImpact = 2f; // Impact under this, skip
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private float birdMultiplier = 2f; // Bird cause damage 2 times
    [SerializeField] private float graceTime = 1f; // Ignore damage when scene start
    [SerializeField] private float killY = -10f; // Dead when go out of map

    [Header("-----UI-----")]
    [SerializeField] private Sprite[] damageSprite; // Hold damage sprite from normal to destroy
    [SerializeField] private GameObject breakEffect; // Prefab effect

    [Header("-----Sound-----")]
    [SerializeField] private string soundGroup = "Wood";
    [SerializeField] private string HitSound = "Wood Hit";
    [SerializeField] private string BreakSound = "Wood Break";

    private float health;
    private float spawnTime;
    private float lastSfxTime;
    private bool isDead;
    private SpriteRenderer spriteRenderer;

    public int PointValue => pointValue;

    protected virtual void Awake() 
    {
        health = maxHealth; 
        spawnTime = Time.time;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (transform.position.y < killY) Die();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (isDead || Time.time - spawnTime < graceTime) return;

        float impact = col.relativeVelocity.magnitude;
        if (impact < minImpact) return;

        float damage = (impact - minImpact) * damageMultiplier;
        if (col.collider.GetComponent<Bird>() != null) damage *= birdMultiplier;

        PlayHitSound();
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        health -= damage;
        if (health <= 0)
        {
            Die();
            return;
        }
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null || damageSprite == null || damageSprite.Length == 0) return;

        float lost = 1f - health / maxHealth;
        int i = Mathf.Min((int)(lost * damageSprite.Length), damageSprite.Length - 1);
        spriteRenderer.sprite = damageSprite[i];
    }

    private void PlayHitSound()
    {
        // Limit the frequency, avoid a five-time overlapping hit
        if (Time.time - lastSfxTime < 0.15f || SoundManager.Instance == null) return;
        lastSfxTime = Time.time;
        Play(HitSound);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Destroyed?.Invoke(this);
        if (breakEffect != null) Instantiate(breakEffect, transform.position, Quaternion.identity);
        Play(BreakSound);
        Destroy(gameObject);
    }

    private void Play(string soundName)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(soundGroup, soundName);
        }
    }
}
