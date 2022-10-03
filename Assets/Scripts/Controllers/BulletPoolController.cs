using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolController : MonoBehaviour
{
    TimeController _timeController;

    List<Bullet> _activePlayerBullets = new List<Bullet> ();
    Queue<Bullet> _pooledPlayerBullets = new Queue<Bullet> ();
    [SerializeField] Bullet _playerBulletPrefab = null;

    List<Bullet> _activeEnemyBullets = new List<Bullet>();
    Queue<Bullet> _pooledEnemyBullets = new Queue<Bullet>();
    [SerializeField] Bullet _enemyBulletPrefab = null;

    private void Awake()
    {
        _timeController = GetComponent<TimeController>();
    }

    public void ReturnExpiredBulletToPool(Bullet expiringBullet)
    {
        if (expiringBullet.CheckIfIsPlayerBullet())
        {
            _activePlayerBullets.Remove(expiringBullet);
            _pooledPlayerBullets.Enqueue(expiringBullet);            
        }
        else
        {
            _activeEnemyBullets.Remove(expiringBullet);
            _pooledEnemyBullets.Enqueue(expiringBullet);
        }
        expiringBullet.gameObject.SetActive(false);

    }

    public Bullet RequestBullet(bool isRequestingPlayerBullet, 
        Vector3 desiredLocation, Quaternion desiredRotation)
    {
        Bullet bullet;
        if (isRequestingPlayerBullet)
        {
            if (_pooledPlayerBullets.Count == 0)
            {
                bullet = Instantiate(_playerBulletPrefab, desiredLocation, desiredRotation);
                bullet.Initialize(this, _timeController);
            }
            else
            {
                bullet = _pooledPlayerBullets.Dequeue();
                bullet.gameObject.SetActive(true);
                bullet.transform.position = desiredLocation;
                bullet.transform.rotation = desiredRotation;
            }
        }
        else
        {
            if (_pooledEnemyBullets.Count == 0)
            {
                bullet = Instantiate(_enemyBulletPrefab, desiredLocation, desiredRotation);
                bullet.Initialize(this, _timeController);
            }
            else
            {
                bullet = _pooledEnemyBullets.Dequeue();
                bullet.gameObject.SetActive(true);
                bullet.transform.position = desiredLocation;
                bullet.transform.rotation = desiredRotation;
            }
        }
        return bullet;
    }

}
