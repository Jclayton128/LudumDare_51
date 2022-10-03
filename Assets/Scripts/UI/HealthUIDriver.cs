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

    [SerializeField] ParticleSystem[] _healthStatusRepairFX = null;
    [SerializeField] Image[] _systemIcons = null;

    [SerializeField] TextMeshProUGUI _instructionTMP = null;
    [SerializeField] TextMeshProUGUI _tooltipTMP = null;
    [SerializeField] Color _tooltipColor = Color.white;

    TimeController _timeController;

    #endregion

    //settings
    [SerializeField] float _deployedUIScale = 2.5f;
    [SerializeField] float _deployTime = 0.7f;
    [SerializeField] Color _iconColor = new Color(1, 1, 1, 0.6f);

    [Tooltip("0: 4/4 hits left, 1: 3/4 hits left, 2: 2/4 hits left, 3: 1/4 hits left, 4: dead")]
    [SerializeField] Color[] _damageLevelsByColor = new Color[6];

    [SerializeField] int _particlesToEmitPerFrameWhileRepairing = 2;

    [SerializeField]
    string[] _systemNames = new string[4]
    {
        "Radar",
        "Weapon",
        "Mobility",
        "Shield"
    };

    //state
    Tween _resizeTween;
    bool _isUIDeployed = false;
    Vector2 _sizeDeltaRetracted;
    Tween[] _iconColorTweens = new Tween[4];
    Tween _tooltipColorTween;
    Tween _instructionTween;

    private void Awake()
    {
        _timeController = FindObjectOfType<TimeController>();
        _timeController.OnNewPhase += HandlePhaseChange;

        GameController gc = _timeController.GetComponent<GameController>();
        gc.OnPlayerStartsRun += HandleOnPlayerStartsRun;
        gc.OnPlayerDies += HandleOnPlayerDies;

        for (int i = 0; i < _systemIcons.Length; i++)
        {
            _iconColorTweens[i].Kill();
            _iconColorTweens[i] =
                _systemIcons[i].DOColor(Color.clear, _deployTime).SetUpdate(false);
        }


        _instructionTMP.color = Color.clear;
        _tooltipTMP.color = Color.clear;

    }

    private void HandleOnPlayerStartsRun(GameObject newPlayer)
    {
        StatsHandler sh = newPlayer.GetComponent<StatsHandler>();
        sh.OnReceiveDamage += HandleDamageStatusChanged;
        _tooltipColorTween.Kill();
        _instructionTween.Kill();
        _tooltipTMP.color = Color.clear;
        _instructionTMP.color = Color.clear;

        ResetHealthStatusToFull();
        _tooltipTMP.text = "";
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
            HandleDamageStatusChanged(i, 0, false);
        }
    }

    [ContextMenu("debug")]
    private void DebugReceiveDamage()
    {
        HandleDamageStatusChanged(1, 2, false);
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
        
        for (int i = 0; i < _systemIcons.Length; i++)
        {
            _iconColorTweens[i].Kill();
            _iconColorTweens[i] =
                _systemIcons[i].DOColor(_iconColor, _deployTime).SetUpdate(false);
        }



        _tooltipColorTween.Kill();
        _tooltipColorTween = _tooltipTMP.DOColor(_tooltipColor, _deployTime);
        _instructionTween.Kill();
        _instructionTween = _instructionTMP.DOColor(_tooltipColor, _deployTime);


    }

    private void RetractHealthPanel()
    {
        _resizeTween.Kill();
        _resizeTween = _healthPanel.DOSizeDelta(_sizeDeltaRetracted, _deployTime);

        for (int i = 0; i < _systemIcons.Length; i++)
        {
            _iconColorTweens[i].Kill();
            _iconColorTweens[i] =
                _systemIcons[i].DOColor(Color.clear, _deployTime).SetUpdate(false);
        }

        _tooltipColorTween.Kill();
        _tooltipColorTween = _tooltipTMP.DOColor(Color.clear, _deployTime);
        _instructionTween.Kill();
        _instructionTween = _instructionTMP.DOColor(Color.clear, _deployTime);
    }

    #endregion

    #region Subsystem Handlers
    private void HandleDamageStatusChanged(int subsystemAffected, float newDamageTier,
        bool isRepairedDamaged)
    {

        _tooltipTMP.text = _systemNames[subsystemAffected];
        _tooltipColorTween.Kill();
        _tooltipTMP.color = _tooltipColor;
        _tooltipColorTween = _tooltipTMP.DOColor(Color.clear, _deployTime * 3f);


        _healthStatusImages[subsystemAffected].color = ConvertDamageTierIntoColor(newDamageTier);

        if (isRepairedDamaged)
        {
            _healthStatusRepairFX[subsystemAffected].
                    Emit(_particlesToEmitPerFrameWhileRepairing);
        }

    }

    private Color ConvertDamageTierIntoColor(float damageTier)
    {
        return _damageLevelsByColor[Mathf.Clamp(((int)Mathf.Ceil(damageTier)) + 1, 0, 5)];
    }

    #endregion
}
