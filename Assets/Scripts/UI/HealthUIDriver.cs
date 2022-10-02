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

    [Tooltip("0: 4/4 hits left, 1: 3/4 hits left, 2: 2/4 hits left, 3: 1/4 hits left, 4: dead")]
    [SerializeField] Color[] _damageLevelsByColor = new Color[5];

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

        GameController gc = _timeController.GetComponent<GameController>();
        gc.OnPlayerStartsRun += HandleOnPlayerStartsRun;
        gc.OnPlayerDies += HandleOnPlayerDies;

        _repairingParticleFX = Instantiate(_repairingInProgressParticleFXPrefab)
            .GetComponent<ParticleSystem>();
        _repairEmissions = _repairingParticleFX.emission;
        _repairEmissions.rateOverTime = 0;
    }

    private void HandleOnPlayerStartsRun(GameObject newPlayer)
    {
        StatsHandler sh = newPlayer.GetComponent<StatsHandler>();
        sh.OnReceiveDamage += HandleDamageStatusChanged;
        ResetHealthStatusToFull();
    }

    private void HandleOnPlayerDies(GameObject despawningPlayer)
    {
        StatsHandler sh = despawningPlayer.GetComponent<StatsHandler>();
        sh.OnReceiveDamage -= HandleDamageStatusChanged;
    }

    private void Start()
    {
        ResetHealthStatusToFull();
    }

    private void ResetHealthStatusToFull()
    {
        for (int i = 0; i < _healthStatusImages.Length; i++)
        {
            HandleDamageStatusChanged(i, 0);
        }
    }

    [ContextMenu("debug")]
    private void DebugReceiveDamage()
    {
        HandleDamageStatusChanged(1, 2);
    }

    #region Phase Change
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

    #endregion

    #region Subsystem Handlers
    private void HandleDamageStatusChanged(int subsystemAffected, float newDamageTier)
    {
        _healthStatusImages[subsystemAffected].color = ConvertDamageTierIntoColor(newDamageTier);
    }

    private Color ConvertDamageTierIntoColor(float damageTier)
    {
        return _damageLevelsByColor[Mathf.Clamp((int)Mathf.Ceil(damageTier), 0, 4)];
    }

    #endregion

    #region Particle FX for repairs in progress

    /// <summary>
    /// Call this to begin displaying a system being repaired.
    /// </summary>
    /// <param name="subsystemIndex"></param>
    private void HandleRepairBeginning(int subsystemIndex)
    {
        _repairingParticleFX.transform.position = _healthStatusImages[subsystemIndex].rectTransform.position;
        _repairEmissions.rateOverTime = 10f;
    }


    /// <summary>
    /// Call this to end displaying a system being repaired.
    /// </summary>
    private void HandleRepairEnding()
    {
        _repairEmissions.rateOverTime = 0f;
    }

    //[ContextMenu("Debug repair 0")]
    //private void DebugTestOnZero()
    //{
    //    HandleRepairBeginning(0);
    //}

    //[ContextMenu("Debug repair 1")]
    //private void DebugTestOnOne()
    //{
    //    HandleRepairBeginning(1);
    //}

    //[ContextMenu("Debug end ")]
    //private void DebugTestOff()
    //{
    //    HandleRepairEnding();
    //}

    #endregion
}
