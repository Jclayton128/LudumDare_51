using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretHandler : MonoBehaviour
{
    TimeController _timeController;
    
    //settings
    [Tooltip("Turret's rotation speed in degrees/sec")]
    [SerializeField] float _baseTurnRate = 50f; 

    [Tooltip("This is multiplied against _baseTurnRate while _isFiring")]
    [SerializeField] float _firingTurnRateModifier = 0.25f;


    //state
    Vector2 _desiredSteeringVector_Debug;

    float _currentTurnRate;
    bool _isFiring = false;
    bool _canRotate = false;
    
    private void Awake()
    {
        _timeController = FindObjectOfType<TimeController>();
        _timeController.OnNewPhase += HandlePhaseChange;
    }

    private void HandlePhaseChange(TimeController.Phase newPhase)
    {
        if (newPhase == TimeController.Phase.B_firepower)
        {
            _canRotate = true;
        }
        else
        {
            _canRotate = false;
        }
    }

    private void Update()
    {
        if (_canRotate)
        {
            //TODO remove shims
            DebugAimingShim(); // Replace with Vector2/3 from Input
            DebugFiringShim(); // Replace with bool from Input

            if (_isFiring)
            {
                _currentTurnRate = _baseTurnRate * _firingTurnRateModifier;
            }
            else
            {
                _currentTurnRate = _baseTurnRate;
            }

            UpdateTurretFacingToMousePos();
        }
        else
        {
            _currentTurnRate = _baseTurnRate;
            UpdateTurretFacingToLocalUp();
        }

    }

    private void DebugAimingShim()
    {
        _desiredSteeringVector_Debug = (Camera.main.ScreenToWorldPoint(Input.mousePosition)- transform.position);
        Debug.DrawLine(transform.position, transform.position + (Vector3)_desiredSteeringVector_Debug, Color.red, 0.1f);
    }

    private void DebugFiringShim()
    {
        _isFiring = Input.GetKey(KeyCode.Mouse0);
    }

    private void UpdateTurretFacingToMousePos()
    {
        float angleToTargetFromNorth = Vector3.SignedAngle(_desiredSteeringVector_Debug, Vector2.up, transform.forward);
        Quaternion angleToPoint = Quaternion.Euler(0, 0, -1 * angleToTargetFromNorth);
        transform.rotation =
            Quaternion.RotateTowards(transform.rotation, angleToPoint,
            _currentTurnRate * Time.deltaTime);
    }


    private void UpdateTurretFacingToLocalUp()
    {        
        Quaternion angleToPoint = Quaternion.Euler(0, 0, 0);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, angleToPoint,
            _currentTurnRate * Time.deltaTime);
    }


}
