using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraController : MonoBehaviour {
    Camera _camera;
    CinemachineVirtualCamera _cvc;
    CinemachineCameraOffset _cco;
    StatsHandler _sh;
    TimeController _tc;

    [SerializeField] float zoomModHealPhase = 2f;
    [SerializeField] float cameraTransitionRate = 0.5f;

    float initialZoom = 6f; // Warning - this number ought to match the parameter in StatsHandler
    Tween currentZoomTween;

    private void OnEnable() {
        _sh.OnReceiveDamage += HandleOnDamageReceived;
        _sh.OnPlayerDying += HandleOnDying;
        _tc.OnNewPhase += HandleNewPhase;
    }

    private void OnDisable() {
        _sh.OnReceiveDamage -= HandleOnDamageReceived;
        _sh.OnPlayerDying -= HandleOnDying;
        _tc.OnNewPhase -= HandleNewPhase;
    }

    private void Awake() {
        _camera = Camera.main;
        _cvc = _camera.GetComponentInChildren<CinemachineVirtualCamera>();
        _cco = _camera.GetComponentInChildren<CinemachineCameraOffset>();
        _sh = FindObjectOfType<StatsHandler>();
        _tc = FindObjectOfType<TimeController>();

        initialZoom = _sh.CameraZoom;
        _cvc.m_Lens.OrthographicSize = _sh.CameraZoom;
        _cco.m_Offset.x = 0f;
    }

    private void HandleNewPhase(TimeController.Phase phase) {
        if (phase == TimeController.Phase.C_healing) {
            DOTween.To(
                () => _cco.m_Offset.x,
                x => _cco.m_Offset.x = x,
                3f,
                cameraTransitionRate);
            currentZoomTween.Kill();
            currentZoomTween = DOTween.To(
                () => _cvc.m_Lens.OrthographicSize,
                x => _cvc.m_Lens.OrthographicSize = x,
                _sh.CameraZoom * zoomModHealPhase,
                cameraTransitionRate);
        } else {
            DOTween.To(
                () => _cco.m_Offset.x,
                x => _cco.m_Offset.x = x,
                0f,
                cameraTransitionRate);
            currentZoomTween.Kill();
            currentZoomTween = DOTween.To(
                () => _cvc.m_Lens.OrthographicSize,
                x => _cvc.m_Lens.OrthographicSize = x,
                _sh.CameraZoom,
                cameraTransitionRate);
        }
    }

    private void HandleOnDying() {
        currentZoomTween.Kill();
        DOTween.To(() => _cvc.m_Lens.OrthographicSize, x => _cvc.m_Lens.OrthographicSize = x, _sh.CameraZoom, cameraTransitionRate);
    }

    private void HandleOnDamageReceived(int system, float f, bool b) {
        if (system != 0) return;
        if (b) return;
        Invoke(nameof(ChangeZoomAPotatoLater), 0.1f);
    }

    private void ChangeZoomAPotatoLater() {
        _cvc.m_Lens.OrthographicSize = _sh.CameraZoom;
    }

}
