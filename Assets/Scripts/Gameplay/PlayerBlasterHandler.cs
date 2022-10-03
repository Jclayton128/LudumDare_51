using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Cinemachine;

public class PlayerBlasterHandler : MonoBehaviour {
    StatsHandler _statsHandlers;
    TimeController _timeController;
    BulletPoolController _bulletPoolController;

    InputController input;
    PlayerMovement movement;

    [SerializeField] Transform _muzzleTransform = null;
    [SerializeField] float _bulletVelocity = 4f;
    [SerializeField] float _bulletLifetime = 2f;
    [SerializeField] float _gunSpinUpTime = 1f;
    [SerializeField] float _recoilScreenShakeMod = 0.1f;
    [SerializeField] AnimationCurve _gunSpinUpRate = AnimationCurve.Linear(0f, 2f, 1f, 1f);
    [SerializeField] StudioEventEmitter gunSound;
    [SerializeField] StudioEventEmitter machineGunSound;
    [SerializeField] CinemachineImpulseSource screenShakeOnFire;

    //state
    [SerializeField] bool _isFiring = false;
    float _timeForNextShot = 0f;
    float _timeSinceStartedShooting = Mathf.Infinity;
    bool _canFire = false;

    private void OnEnable() {
        _timeController.OnNewPhase += HandleOnPhaseChange;
    }

    private void OnDisable() {
        _timeController.OnNewPhase -= HandleOnPhaseChange;
    }

    private void Awake() {
        _statsHandlers = GetComponent<StatsHandler>();
        _bulletPoolController = FindObjectOfType<BulletPoolController>();
        _timeController = _bulletPoolController.GetComponent<TimeController>();
        input = GetComponent<InputController>();
        movement = GetComponent<PlayerMovement>();
        _canFire = true;
    }

    private void HandleOnPhaseChange(TimeController.Phase newPhase) {
        if (newPhase == TimeController.Phase.A_mobility || newPhase == TimeController.Phase.B_firepower) {
            _canFire = _statsHandlers.IsAlive;
            _timeSinceStartedShooting = 0f;
        } else {
            _canFire = false;
        }
    }

    private void Update() {
        if (input.IsFirePressed) HandleOnStartFiring();
        if (!input.IsFirePressed) HandleOnStopFiring();

        if (_canFire && _isFiring && Time.time >= _timeForNextShot) {
            Fire();
            PlayGunSound();
            _timeForNextShot = GetNextShotTime();
        }

        if (!_canFire) {
            gunSound.Stop();
            machineGunSound.Stop();
        }

        _timeSinceStartedShooting += Time.deltaTime;
        if (!_statsHandlers.IsAlive) _canFire = false;
    }

    private float GetNextShotTime() {
        if (!_timeController.IsPhaseB) return Time.time + _statsHandlers.FireRate;
        return Time.time + _statsHandlers.FireRate * _gunSpinUpRate.Evaluate(Mathf.Clamp01(_timeSinceStartedShooting / _gunSpinUpTime));
    }

    private void PlayGunSound() {
        if (_timeController.IsPhaseB) machineGunSound.Play();
        else gunSound.Play();
    }

    private void HandleOnStartFiring() {
        if (_isFiring) return;
        _isFiring = true;
        _timeSinceStartedShooting = 0f;
    }

    private void HandleOnStopFiring() {
        _isFiring = false;
    }

    private void Fire() {
        Bullet bullet = _bulletPoolController.RequestBullet(true,
            _muzzleTransform.position, _muzzleTransform.rotation);

        Vector2 vel = _muzzleTransform.up * _bulletVelocity;
        // apply extra velocity to the bullet if the direction matches the player's current velocity
        vel = vel + movement.Velocity * Mathf.Clamp01(Vector2.Dot(vel, movement.Velocity));

        screenShakeOnFire.GenerateImpulse(-vel.normalized * _recoilScreenShakeMod);

        bullet.SetupForUse(_bulletLifetime, vel);

    }
}
