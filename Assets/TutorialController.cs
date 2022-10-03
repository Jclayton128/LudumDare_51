using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

public class TutorialController : MonoBehaviour
{
    [SerializeField] string[] _tutorialSteps = null;
    [SerializeField] float[] _durationOfSteps = null;
    [SerializeField] TextMeshProUGUI _tutorialTMP = null;
    [SerializeField] Color _textColor = Color.white;

    //settings
    float _fadeTime = 2.0f;


    //state
    int _currentStepIndex = 0;
    [SerializeField] float _countdownForCurrentStep = Mathf.Infinity;
    Tween _colorFadeTween;

    private void Awake()
    {
        GetComponent<GameController>().OnPlayerStartsRun += HandleStartRun;
        _tutorialTMP.color = Color.clear;
    }

    private void HandleStartRun(GameObject go)
    {
        _tutorialTMP.color = Color.white;
        _countdownForCurrentStep = _durationOfSteps[_currentStepIndex];
        _tutorialTMP.text = _tutorialSteps[_currentStepIndex];
    }

    private void Update()
    {
        _countdownForCurrentStep -= Time.unscaledDeltaTime;

        if (_countdownForCurrentStep < _fadeTime)
        {
            _colorFadeTween.Kill();
            _colorFadeTween = _tutorialTMP.DOColor(Color.clear, _fadeTime);
        }

        if (_countdownForCurrentStep <= 0)
        {
            IncrementToNextStep();
        }
    }

    private void IncrementToNextStep()
    {
        _currentStepIndex++;
        if (_currentStepIndex >= _tutorialSteps.Length) return;

        _colorFadeTween.Kill();
        _tutorialTMP.color = _textColor;
        _countdownForCurrentStep = _durationOfSteps[_currentStepIndex];
        _tutorialTMP.text = _tutorialSteps[_currentStepIndex];

    }
}
