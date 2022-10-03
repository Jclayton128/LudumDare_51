using UnityEngine;
using System;
using FMODUnity;

// An "Entity" is a game character that has health and can be killed.

public class Entity : MonoBehaviour {
    ExplosionController _explosionController;
    EnemyController _enemyController;

    /// <summary>
    /// Float argument is the damage amount
    /// </summary>
    public Action<float> OnReceiveDamage;

    public Action OnDie;

    [SerializeField] StudioEventEmitter soundEnemyDeath;
    [SerializeField] StudioEventEmitter soundEnemyDamage;

    [Header("Health")]
    [SerializeField] float startingHealth = 100f;
    [SerializeField] float timeInvincibleAfterHit = 0f;
    [Space]
    [Space]
    [Header("Death")]
    [SerializeField] bool hideOnDeath = false;
    [SerializeField] bool explodeOnDeath = false;
    [SerializeField] Explosion explosionPrefab;

    float currentHealth;
    public bool IsAlive { get; private set; } = true;

    float _timeInvulnerabilityEnds = 0;

    private void Awake() {
        currentHealth = startingHealth;
        AppIntegrity.Assert(soundEnemyDamage != null);
        AppIntegrity.Assert(soundEnemyDeath != null);
    }

    public void Initialize(EnemyController ecRef, ExplosionController exconRef) {
        _enemyController = ecRef;
        _explosionController = exconRef;
    }

    public void SetUpForUse() {
        _timeInvulnerabilityEnds = 0;
        currentHealth = startingHealth;
        IsAlive = true;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!IsAlive) return;
        if (other.collider == null) return;
        if (explodeOnDeath && other.collider.CompareTag("Player")) {
            TakeDamage(10f);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!IsAlive) return;
        //TODO reference a variable damage amount. Not all bullets do exactly 1 damage?
        TakeDamage(1);
    }

    public bool TakeDamage(float amount) {
        if (!IsAlive) return false;
        if (Time.time < _timeInvulnerabilityEnds) return false;

        if (_explosionController != null) _explosionController.RequestExplosion(amount, transform.position, Color.red);
        OnReceiveDamage?.Invoke(amount);
        currentHealth -= amount;

        if (timeInvincibleAfterHit > 0) {
            _timeInvulnerabilityEnds = Time.time + timeInvincibleAfterHit;
        }

        if (currentHealth <= 0) {
            soundEnemyDeath.Play();
            Die();
        } else {
            soundEnemyDamage.Play();
        }

        return true;
    }

    void Die() {
        if (!IsAlive) return;
        IsAlive = false;

        currentHealth = Mathf.Min(currentHealth, 0f);

        if (_explosionController != null) _explosionController.RequestExplosion(startingHealth, transform.position, Color.red);
        OnDie?.Invoke();

        if (explodeOnDeath) Detonate();

        if (_enemyController != null) {
            _enemyController.ReturnDeadEnemy(this.gameObject);
        } else {
            if (hideOnDeath) HideSprite();
        }

        //if (deathFX != null) deathFX.Play();
    }

    void Detonate() {
        if (explosionPrefab == null) return;
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }

    void HideSprite() {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) return;
        spriteRenderer.enabled = false;
    }
}
