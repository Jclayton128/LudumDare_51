using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class HealthUIDriver : MonoBehaviour
{
    #region Scene References
    [SerializeField] RectTransform _healthPanel = null;

    [Tooltip("0: Up, 1: Down, 2: Left, 3: Right")]
    [SerializeField] Image[] _healthStatusImages = null;

    TimeController _timeController;

    #endregion

    //settings
    [SerializeField] float _deployedUIScale = 2.5f;
    [SerializeField] float _deployTime = 0.7f;
    [SerializeField] ParticleSystem _repairingInProgressParticleFXPrefab = null;

    //state
    Tween _resizeTween;
    bool _isUIDeployed = false;
    Vector2 _sizeDeltaRetracted;
    ParticleSystem.EmissionModule _repairEmissions;
    ParticleSystem _repairingParticleFX;

    private void Awake()
    {
        _timeController = FindObjectOfType<TimeController>();
        _timeController.OnNewPhase += HandlePhaseChange;

        //connect to Player's HealthHandler
        //healthHandler.OnReceiveDamage += HandleReceivedDamage;

        _repairingParticleFX = Instantiate(_repairingInProgressParticleFXPrefab)
            .GetComponent<ParticleSystem>();
        _repairEmissions = _repairingParticleFX.emission;
        _repairEmissions.rateOverTime = 0;
    }

    private void HandlePhaseChange(TimeController.Phase newPhase)
    {
        if (newPhase == TimeController.Phase.C_healing)
        {
            EnlargeHealthPanel();
            _isUIDeployed = true;
        }
        else
        {
            if (!_isUIDeployed)
            {
                //nothing
            }
            else
            {
                RetractHealthPanel();
            }
        }
    }

    private void EnlargeHealthPanel()
    {
        _sizeDeltaRetracted = _healthPanel.sizeDelta;

        _resizeTween.Kill();
        _resizeTween = _healthPanel.DOSizeDelta(_sizeDeltaRetracted * _deployedUIScale, _deployTime);
    }

    private void RetractHealthPanel()
    {
        _resizeTween.Kill();
        _resizeTween = _healthPanel.DOSizeDelta(_sizeDeltaRetracted, _deployTime);
    }

    private void HandleReceivedDamage()
    {
       

    }

/// <summary>
/// Call this to begin displaying a system being repaired.
/// </summary>
/// <param name="subsystemIndex"></param>
    private void HandleRepairBeginning(int subsystemIndex)
    {
        _repairingParticleFX.transform.position = _healthStatusImages[subsystemIndex].rectTransform.position;
        _repairEmissions.rateOverTime = 10f;
    }

    private void HandleRepairEnding()
    {
        _repairEmissions.rateOverTime = 0f;
    }

    [ContextMenu("Debug repair 0")]
    private void DebugTestOnZero()
    {
        HandleRepairBeginning(0);
    }

    [ContextMenu("Debug repair 1")]
    private void DebugTestOnOne()
    {
        HandleRepairBeginning(1);
    }

    [ContextMenu("Debug end ")]
    private void DebugTestOff()
    {
        HandleRepairEnding();
    }
}
