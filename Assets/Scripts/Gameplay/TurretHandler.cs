using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretHandler : MonoBehaviour {
    TimeController _timeController;

    //settings
    [Tooltip("Turret's rotation speed in degrees/sec")]
    [SerializeField] float _baseTurnRate = 50f;

    [Tooltip("Turn rate delta - amount of change per second")]
    [SerializeField] float _turnRateDelta = 50f;

    [Tooltip("This is multiplied against _baseTurnRate while _isFiring")]
    [SerializeField] float _firingTurnRateModifier = 0.25f;

    [SerializeField] InputController input;

    //state
    Vector2 _desiredSteeringVector;

    float _currentTurnRate;
    bool _isFiring = false;
    bool _canRotate = false;

    private void Awake() {
        _timeController = FindObjectOfType<TimeController>();
        _timeController.OnNewPhase += HandlePhaseChange;
        _canRotate = true;
        AppIntegrity.Assert(input != null, "InputController must be provided to TurretHandler!");
    }

    private void HandlePhaseChange(TimeController.Phase newPhase) {
        if (newPhase == TimeController.Phase.A_mobility || newPhase == TimeController.Phase.B_firepower) {
            _canRotate = true;
        } else {
            _canRotate = false;
        }
    }

    private void Update() {
        if (!_canRotate) return;

        HandleAim(); // Replace with Vector2/3 from Input
        HandleIsFiring(); // Replace with bool from Input

        if (_timeController.IsPhaseB) {
            _currentTurnRate = MoveTowards(
                _currentTurnRate,
                _baseTurnRate * (_isFiring ? _firingTurnRateModifier : 1f),
                _turnRateDelta
            );
        } else {
            _currentTurnRate = _baseTurnRate;
        }

        UpdateTurretFacingToMousePos();
    }

    private void HandleAim() {
        // _desiredSteeringVector = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        _desiredSteeringVector = input.IsInputKeyboardAndMouse ? input.MousePositionWorld : input.Look;
        Debug.DrawLine(transform.position, transform.position + (Vector3)_desiredSteeringVector, Color.red, 0.1f);
    }

    private void HandleIsFiring() {
        _isFiring = input.IsFirePressed;
    }

    private void UpdateTurretFacingToMousePos() {
        float angleToTargetFromNorth = Vector3.SignedAngle(_desiredSteeringVector, Vector2.up, transform.forward);
        Quaternion angleToPoint = Quaternion.Euler(0, 0, -1 * angleToTargetFromNorth);
        transform.rotation =
            Quaternion.RotateTowards(transform.rotation, angleToPoint,
            _currentTurnRate * Time.deltaTime);
    }

    private void UpdateTurretFacingToLocalUp() {
        Quaternion angleToPoint = Quaternion.Euler(0, 0, 0);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, angleToPoint,
            _currentTurnRate * Time.deltaTime);
    }

    private float MoveTowards(float current, float target, float delta) {
        if (target > current) return current + Time.deltaTime * delta;
        if (target < current) return current - Time.deltaTime * delta;
        return 0f;
    }
}
