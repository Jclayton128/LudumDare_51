using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    enum ShotType {
        Single,
        Lead,
        NovaShot,
        Nothing,
        Lunge,
    }

    TimeController _timeController;
    BulletPoolController _bulletPoolController;
    /// 1 - find player
    /// 2 - translate towards player.  Stop at standoff range
    /// 3 - shoot at player.

    [Header("Move Parameters")]

    [Tooltip("Average Range at which these enemies stop trying to close the distance. Should be" +
        "less than max weapon range. Actual enemies use a random variation on this.")]
    [SerializeField] float _standoffRange_Nominal = 3f;
    [SerializeField] float _maxMoveSpeed = 3f;

    [Tooltip("How long it takes to get from 0 to 60, or vice versa." +
        "Higher should feel heavier, with more inertia.")]
    [SerializeField] float _accelDecelTime = 1.0f;

    [Header("Attack Parameters")]

    [Tooltip("Distance a enemy bullet from this enemy will travel.")]
    [SerializeField] float _weaponRange = 15f;

    [Tooltip("Speed that an enemy bullet from this enemy will travel.")]
    [SerializeField] float _weaponSpeed = 5f;

    [SerializeField] float _timeBetweenShots = 1.0f;
    [SerializeField] ShotType _shotType = EnemyBehavior.ShotType.Single;

    [Header("Lunge Behavior")]

    [SerializeField] float timeTelegraphLunge = 0.4f;
    [SerializeField] float lungeDuration = 1f;
    [SerializeField] float lungeSpeed = 1f;
    [SerializeField] AnimationCurve lungeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0.2f);

    GameObject _player;
    PlayerMovement _pm;
    Vector3 _moveDirection;
    float _actualMoveSpeed;
    float _rangeToPlayer;
    float _countdownUntilNextShot = 0;
    float _standoffRange_Actual;
    float _accelDecelRate;

    EnemyAnimator _enemyAnimator;
    Coroutine _lunging;
    bool isLunging = false;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _pm = _player.GetComponent<PlayerMovement>();
        _timeController = FindObjectOfType<TimeController>();
        _enemyAnimator = GetComponent<EnemyAnimator>();
        _bulletPoolController = _timeController.GetComponent<BulletPoolController>();
        _standoffRange_Actual = UnityEngine.Random.Range(.8f, 1.2f) * _standoffRange_Nominal;
        _accelDecelRate = _maxMoveSpeed / _accelDecelTime;
    }

    private void Update()
    {
        if (!_player) return;

        Move();

        _countdownUntilNextShot -= Time.deltaTime * _timeController.EnemyTimeScale;
        if (_countdownUntilNextShot <= 0 && _rangeToPlayer < _weaponRange)
        {
            Fire();
            _countdownUntilNextShot = _timeBetweenShots;
        }


    }

    private void Move()
    {
        if (isLunging) return;
        _moveDirection = (_player.transform.position - transform.position);
        _rangeToPlayer = _moveDirection.magnitude;

        if (_rangeToPlayer > _standoffRange_Actual)
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
    }

    private void Fire()
    {
        switch (_shotType)
        {
            case ShotType.Single:
                FireSingleShot();
                break;

            case ShotType.Lead:
                FireSingleShotWithLead();
                break;

            case ShotType.NovaShot:
                FireNovaShot();
                break;

            case ShotType.Nothing:
                break;

            case ShotType.Lunge:
                LungeTowardsPlayer();
                break;
        }
    }

    private void FireSingleShot()
    {
        float lifetime = (_rangeToPlayer / _weaponSpeed) * 1.5f;
        Vector2 velocity = _moveDirection.normalized * _weaponSpeed;
        Bullet bullet = _bulletPoolController.
            RequestBullet(false, transform.position, Quaternion.identity);
        bullet.SetupForUse(lifetime, velocity);
    }

    private void FireSingleShotWithLead()
    {
        float lifetime =
            (_player.transform.position + (Vector3)_pm.Velocity).magnitude / _weaponSpeed;
        Vector3 futurePlayerPosition = _player.transform.position + (Vector3)(_pm.Velocity * lifetime);
        Vector3 direction = futurePlayerPosition - transform.position;
        Vector2 velocity = direction.normalized * _weaponSpeed;
        Bullet bullet = _bulletPoolController.
            RequestBullet(false, transform.position, Quaternion.identity);
        bullet.SetupForUse(lifetime*1.3f, velocity);
    }

    private void FireNovaShot()
    {
        float degreesSpreadOfEntireBurst = 360f;
        int projectilesInBurst = 10;

        float spreadSubdivided = degreesSpreadOfEntireBurst / projectilesInBurst;
        for (int i = 0; i < projectilesInBurst; i++)
        {
            Quaternion sector = Quaternion.Euler(0, 0, (i * spreadSubdivided) - (degreesSpreadOfEntireBurst / 2) + transform.eulerAngles.z);
            Bullet bullet = _bulletPoolController.RequestBullet(false, transform.position, sector);
            Vector2 velocity = bullet.transform.up * _weaponSpeed;
            bullet.SetupForUse(_weaponRange / _weaponSpeed, velocity);
        }
    }

    private void LungeTowardsPlayer() {
        if (_lunging != null) StopCoroutine(_lunging);
        _lunging = StartCoroutine(Lunging());
    }

    IEnumerator Lunging() {
        isLunging = true;
        if (_enemyAnimator != null) _enemyAnimator.SetAgro(true);
        yield return new WaitForSeconds(timeTelegraphLunge);
        if (_enemyAnimator != null) _enemyAnimator.SetAgro(false);
        float t = 0f;
        while (t < lungeDuration) {
            Vector2 heading = (_player.transform.position - transform.position).normalized;
            transform.position += (Vector3)heading * lungeSpeed * lungeCurve.Evaluate(t / lungeDuration) * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }
        isLunging = false;
        _lunging = null;
    }
}
