using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    TimeController _timeController;
    BulletPoolController _bulletPoolController;
    /// 1 - find player
    /// 2 - translate towards player.  Stop at standoff range
    /// 3 - shoot at player.

    [Tooltip("Average Range at which these enemies stop trying to close the distance. Should be" +
        "less than max weapon range. Actual enemies use a random variation on this.")]
    [SerializeField] float _standoffRange_Nominal = 3f;

    [Tooltip("Distance a enemy bullet from this enemy will travel.")]
    [SerializeField] float _maxWeaponRange = 15f;

    [Tooltip("Speed that an enemy bullet from this enemy will travel.")]
    [SerializeField] float _weaponSpeed = 5f;

    [SerializeField] float _timeBetweenShots = 1.0f;

    [SerializeField] float _maxMoveSpeed = 3f;
    [SerializeField] bool _isLeadingPlayerWithShots = false;

    [Tooltip("How long it takes to get from 0 to 60, or vice versa." +
        "Higher should feel heavier, with more inertia.")]
    [SerializeField] float _accelDecelTime = 1.0f;

    GameObject _player;
    PlayerMovement _pm;
    Vector3 _moveDirection;
    float _actualMoveSpeed;
    float _range;
    float _countdownUntilNextShot = 0;
    float _standoffRange_Actual;
    float _accelDecelRate;


    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _pm = _player.GetComponent<PlayerMovement>();
        _timeController = FindObjectOfType<TimeController>();
        _bulletPoolController = _timeController.GetComponent<BulletPoolController>();
        _standoffRange_Actual = UnityEngine.Random.Range(.8f, 1.2f) * _standoffRange_Nominal;
        _accelDecelRate = _maxMoveSpeed / _accelDecelTime;
    }

    private void Update()
    {
        if (!_player) return;
        _moveDirection = (_player.transform.position - transform.position);
        _range = _moveDirection.magnitude;

        if (_range > _standoffRange_Actual)
        {
            _actualMoveSpeed = Mathf.MoveTowards
                (_actualMoveSpeed, _maxMoveSpeed,
                _accelDecelRate * Time.deltaTime * _timeController.EnemyTimeScale);
        }
        else
        {
            _actualMoveSpeed = Mathf.MoveTowards
                   (_actualMoveSpeed, 0,
                   _accelDecelRate * Time.deltaTime * _timeController.EnemyTimeScale);
        }

        transform.position += _moveDirection.normalized * _actualMoveSpeed *
                _timeController.EnemyTimeScale * Time.deltaTime;

        _countdownUntilNextShot -= Time.deltaTime * _timeController.EnemyTimeScale;
        if (_countdownUntilNextShot <= 0 && _range < _maxWeaponRange )
        {
            Fire();
            _countdownUntilNextShot = _timeBetweenShots;
        }

        
    }

    private void Fire()
    {
        float lifetime = (_range / _weaponSpeed) * 1.5f;
        Vector2 velocity = _moveDirection.normalized * _weaponSpeed;
        Bullet bullet = _bulletPoolController.
            RequestBullet(false, transform.position, Quaternion.identity);
        bullet.SetupForUse(lifetime, velocity);
    }
}
