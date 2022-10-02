using UnityEngine;

public class Bullet : MonoBehaviour {
    BulletPoolController _poolController;
    TimeController _timeController;
    [SerializeField] bool _isPlayerBullet = false;
    public bool IsPlayerBullet { get => _isPlayerBullet; }

    //state
    float _scaledLifetimeRemaining;
    Vector3 _velocity;

    public void Initialize(BulletPoolController poolControllerRef, TimeController timeControllerRef) {
        _poolController = poolControllerRef;
        _timeController = timeControllerRef;
    }

    public void SetupForUse(float lifetime, Vector3 velocity) {
        _scaledLifetimeRemaining = lifetime;
        _velocity = velocity;
    }

    private void Update() {
        if (_isPlayerBullet) {
            _scaledLifetimeRemaining -= Time.deltaTime;
        } else {
            _scaledLifetimeRemaining -= Time.deltaTime * _timeController.EnemyTimeScale;
        }

        if (_scaledLifetimeRemaining <= 0) {
            _poolController.ReturnExpiredBulletToPool(this);
        }


        //Moved this here for smoother movement in Phase C.
        if (_isPlayerBullet) {
            transform.position +=
                _velocity * Time.deltaTime;
        } else {
            transform.position +=
                _velocity * Time.deltaTime * _timeController.EnemyTimeScale;
        }
    }

    private void FixedUpdate() {
        //Bullet movement was here, but moved to Update for smoother movement in Phase C

    }

}