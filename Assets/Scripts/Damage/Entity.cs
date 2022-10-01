using UnityEngine;

// An "Entity" is a game character that has health and can be killed.

public class Entity : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float startingHealth = 100f;
    [SerializeField] float timeInvincibleAfterHit = 0f;
    [Space]
    [Space]
    [Header("Death")]
    [SerializeField] bool hideOnDeath = false;
    [SerializeField] ParticleSystem deathFX;

    float hp = 100f;
    bool isAlive = true;

    float timeSinceTookDamage = Mathf.Infinity;

    public bool TakeDamage(float amount)
    {
        if (timeSinceTookDamage < timeInvincibleAfterHit) return false;

        hp -= amount;

        if (hp <= 0) Die();

        return true;
    }

    void Die()
    {
        if (!isAlive) return;
        isAlive = false;
        hp = Mathf.Min(hp, 0f);
        if (hideOnDeath) HideSprite();
        if (deathFX != null) deathFX.Play();
    }

    void HideSprite()
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) return;
        spriteRenderer.enabled = false;
    }
}
