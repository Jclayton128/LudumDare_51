using UnityEngine;

public class Bullet : MonoBehaviour
{
    BulletPoolController _poolController;
    [SerializeField] bool _isPlayerBullet = false;
    public bool IsPlayerBullet { get => _isPlayerBullet; }

    //state
    float _timeToExpire;

    public void Initialize(BulletPoolController poolControllerRef)
    {
        _poolController = poolControllerRef;
    }

    public void SetupForUse(float timeToExpire)
    {
        _timeToExpire = timeToExpire;
    }

    private void Update()
    {
        if (Time.time >= _timeToExpire)
        {
            _poolController.ReturnExpiredBulletToPool(this);
        }
    }

}