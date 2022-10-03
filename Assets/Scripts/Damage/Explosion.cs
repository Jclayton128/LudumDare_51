using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using FMODUnity;
using UnityEngine;


//
// EXPLOSION
// This is meant to be a standalone explosion class
//
public class Explosion : MonoBehaviour {
    [SerializeField] float timeActive = 0.7f;
    [SerializeField] float explosiveForce = 5f;
    [SerializeField] ParticleSystem particleFX;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask bulletLayer;
    [SerializeField] StudioEventEmitter explosionSound;
    [SerializeField] CinemachineImpulseSource screenShake;

    BulletPoolController bulletPool;

    CircleCollider2D circle;

    RaycastHit2D[] results = new RaycastHit2D[5];

    List<Entity> entities = new List<Entity>();

    float t = 0f;
    bool hitPlayer = false;

    void Awake() {
        particleFX = GetComponent<ParticleSystem>();
        circle = GetComponent<CircleCollider2D>();
        gameObject.SetActive(true);
        particleFX.Play();
        bulletPool = FindObjectOfType<BulletPoolController>();
        AppIntegrity.Assert(explosionSound != null);
    }

    void Start() {
        explosionSound.Play();
        StartCoroutine(ScreenShakeOnExplode());
    }

    void Update() {
        if (t >= timeActive) {
            //circle.enabled = false;
            gameObject.SetActive(false);
            return;
        }

        int numHits = circle.Cast(Vector2.zero, results);
        for (int i = 0; i < numHits; i++) {
            OnHit(results[i].collider);
        }

        t += Time.unscaledDeltaTime; // was += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (t >= timeActive) return;
        OnHit(other);
    }

    void OnHit(Collider2D collider) {
        if (collider == null) return;

        if (collider.CompareTag("Player")) {
            OnHitPlayer(collider);
            return;
        }

        if ((enemyLayer & (1 << collider.gameObject.layer)) > 0) {
            OnHitEnemy(collider);
            return;
        }

        if (((bulletLayer) & (1 << collider.gameObject.layer)) > 0) {
            OnHitBullet(collider);
            return;
        }
    }

    void OnHitPlayer(Collider2D collider) {
        if (hitPlayer) return;
        StatsHandler stats = collider.GetComponent<StatsHandler>();
        if (stats == null) return;
        stats.ReceiveExplosiveImpact(5f);
        PushBackPlayer(stats);
        Physics2D.IgnoreCollision(collider, circle);
        hitPlayer = true;
    }

    void OnHitEnemy(Collider2D collider) {
        Entity entity = collider.GetComponent<Entity>();
        if (entity == null) return;
        entity.TakeDamage(10f);
    }

    void OnHitBullet(Collider2D collider) {
        Bullet bullet = collider.GetComponent<Bullet>();
        if (bullet == null) return;
        if (bulletPool == null) return;
        bulletPool.ReturnExpiredBulletToPool(bullet);
    }

    void PushBackPlayer(StatsHandler stats) {
        if (stats == null) return;
        PlayerMovement movement = stats.GetComponent<PlayerMovement>();
        if (movement == null) return;
        movement.AddForce((movement.transform.position - transform.position).normalized * explosiveForce);
    }

    IEnumerator ScreenShakeOnExplode() {
        float amount = UnityEngine.Random.Range(.2f, .4f);
        screenShake.GenerateImpulse(Vector3.right * amount);
        yield return new WaitForSecondsRealtime(0.1f);
        // amount = UnityEngine.Random.Range(.2f, .4f);
        // screenShake.GenerateImpulse(Vector3.up * amount);
    }
}