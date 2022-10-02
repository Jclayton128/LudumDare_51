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

    [Tooltip("Range at which the enemy stops trying to close the distance. Should be" +
        "less than max weapon range.")]
    [SerializeField] float _standoffRange = 3f;

    [Tooltip("Distance a enemy bullet from this enemy will travel.")]
    [SerializeField] float _maxWeaponRange = 15f;

    [Tooltip("Speed that an enemy bullet from this enemy will travel.")]
    [SerializeField] float _weaponSpeed = 5f;

    [SerializeField] float _timeBetweenShots = 1.0f;

    [SerializeField] float _moveSpeed = 3f;
    [SerializeField] bool _isLeadingPlayerWithShots = false;

    GameObject _player;
    PlayerMovement _pm;
    Vector3 _moveDirection;
    float _range;
    float _countdownUntilNextShot = 0;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _pm = _player.GetComponent<PlayerMovement>();
        _timeController = FindObjectOfType<TimeController>();
        _bulletPoolController = _timeController.GetComponent<BulletPoolController>();
    }

    private void Update()
    {
        _moveDirection = (_player.transform.position - transform.position);
        _range = _moveDirection.magnitude;

        if (_range < _standoffRange) return;
        else
        {
            transform.position += _moveDirection.normalized * _moveSpeed *
                _timeController.EnemyTimeScale * Time.deltaTime;
        }


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
        Quaternion rot = Quaternion.LookRotation(_moveDirection, Vector3.up;
        _bulletPoolController.RequestBullet(false, lifetime, transform.position, rot);
    }
}
