using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TimerUIDriver : MonoBehaviour
{
    #region Scene References

    [Tooltip("These must be ordered from 0 to 9. Index 0 should be the central big triangle.")]
    [SerializeField] Image[] _countdownTriangles = null;

    [SerializeField] TextMeshProUGUI _phaseTMP = null;

    #endregion

    TimeController _timeController;

    //settings
    const float _timerEvaporationTime = 0.5f;

    //state
    Color[] _countdownTriangleStartingColors;
    Tween[] _sizeTweens;
    Tween[] _colorTweens;

    private void Awake()
    {
        _sizeTweens = new Tween[_countdownTriangles.Length];
        _colorTweens = new Tween[_countdownTriangles.Length];

        _timeController = FindObjectOfType<TimeController>();
        _timeController.OnTimerAdvancement += UpdateDisplayedTimer;
        _timeController.OnNewPhase += UpdateDisplayedPhaseOnNewPhase;

        GatherCountdownTrianglesStartingColors();

        UpdateDisplayedPhaseOnNewPhase(TimeController.Phase.A_mobility);


    }

    private void GatherCountdownTrianglesStartingColors()
    {
        _countdownTriangleStartingColors = new Color[_countdownTriangles.Length];
        for (int i = 0; i < _countdownTriangles.Length; i++)
        {
            _countdownTriangleStartingColors[i] = _countdownTriangles[i].color;
        }
    }

    private void UpdateDisplayedTimer(int timeRemaining)
    {
        _sizeTweens[timeRemaining].Kill();
        _colorTweens[timeRemaining].Kill();

        //Grow and Dissolve the current triangle
        _sizeTweens[timeRemaining] = _countdownTriangles[timeRemaining].rectTransform.DOScale(1.3f, _timerEvaporationTime/1.3f);
        _colorTweens[timeRemaining] = _countdownTriangles[timeRemaining].DOColor(Color.clear, _timerEvaporationTime);
    }

    private void UpdateDisplayedPhaseOnNewPhase(TimeController.Phase currentPhase)
    {
        ResetTimer();
        switch (currentPhase)
        {
            case TimeController.Phase.A_mobility:
                _phaseTMP.text = "A";
                break;

            case TimeController.Phase.B_firepower:
                _phaseTMP.text = "B";
                break;

            case TimeController.Phase.C_healing:
                _phaseTMP.text = "C";
                break;
        }
    }

    private void ResetTimer()
    {
        for (int i = 0; i < _countdownTriangles.Length; i++)
        {
            _countdownTriangles[i].color = _countdownTriangleStartingColors[i];
            _countdownTriangles[i].rectTransform.localScale = Vector3.one;
        }
    }

}
