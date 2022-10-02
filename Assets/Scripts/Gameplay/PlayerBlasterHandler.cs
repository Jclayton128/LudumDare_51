using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlasterHandler : MonoBehaviour
{
    StatsHandler _statsHandlers;
    TimeController _timeController;
    BulletPoolController _bulletPoolController;

    [SerializeField] Transform _muzzleTransform = null;
    [SerializeField] float _bulletVelocity = 4f;
    [SerializeField] float _bulletLifetime = 2f;

    //state
    [SerializeField] bool _isFiring = false;
    float _timeForNextShot = Mathf.Infinity;
    bool _canFire = false;


    private void Awake()
    {
        _statsHandlers = GetComponent<StatsHandler>();
        _bulletPoolController = FindObjectOfType<BulletPoolController>();
        _timeController = _bulletPoolController.GetComponent<TimeController>();
        _timeController.OnNewPhase += HandleOnPhaseChange;
    }

    private void HandleOnPhaseChange(TimeController.Phase newPhase)
    {
        if (newPhase == TimeController.Phase.B_firepower)
        {
            _canFire = true;
        }
        else
        {
            _canFire = false;
        }
    }

    private void Update()
    {
        if (_canFire && _isFiring && Time.time >= _timeForNextShot)
        {
            Fire();
            _timeForNextShot = Time.time + _statsHandlers.FireRate;
        }

        //SHIM
        if (Input.GetKeyDown(KeyCode.Mouse0)) HandleOnStartFiring();
        if (Input.GetKeyUp(KeyCode.Mouse0)) HandleOnStopFiring();
    }

    private void HandleOnStartFiring()
    {
        _isFiring = true;
        
        //This is where the spool up time penalty kicks in...
        //TODO audio for blaster spool up?
        _timeForNextShot = Time.time + (3*_statsHandlers.FireRate);
    }

    private void HandleOnStopFiring()
    {
        _isFiring = false;
    }

    private void Fire()
    {
        //TODO play firing audio clip
        Bullet bullet = _bulletPoolController.RequestBullet(true, _bulletLifetime,
            _muzzleTransform.position, _muzzleTransform.rotation);

        //TODO hook this into the player ship's velocity so a
        //fast player doesn't outpace own bullets
        bullet.GetComponent<Rigidbody2D>().velocity =
            _muzzleTransform.up * _bulletVelocity;            
    }



}
