using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class TimeController : MonoBehaviour
{
    #region Settings/Parameters
    public enum Phase {A_mobility, B_firepower, C_healing, Wraparound }
    public Action<int> OnTimerAdvancement;
    public Action<Phase> OnNewPhase;

    const float _timeBetweenPhases = 10f; // Connection to the LD theme right here!
    const float _timeBetweenTimerAdvancements = 1f;

    [Tooltip("These correspond to Phase A, Phase B, and Phase C, in order")]
    [SerializeField]
    private float[] _enemyTimeScales = new float[3]
    {
        0.5f, 1.0f, 0.1f
    };
    float _timeToLerptoNewTimescale = 1f;


    #endregion

    #region State
    private float _currentEnemyTimeScale;
    public float EnemyTimeScale {
        get => _currentEnemyTimeScale;
    }

    private float _currentPlayerTimeScale = 1;
    public float PlayerTimeScale
    {
        get => _currentPlayerTimeScale;
    }

    float _timeToRotatePhases;
    float _timeForTimerAdvancement;
    int _timerAdvancementsRemainingInPhase;
    public Phase CurrentPhase { get; private set; } = TimeController.Phase.A_mobility;

    #endregion

    private void Start()
    {
        _timeToRotatePhases = Time.time + _timeBetweenPhases;
        _timeForTimerAdvancement = Time.time + _timeBetweenTimerAdvancements;
        _timerAdvancementsRemainingInPhase = Mathf.RoundToInt(_timeBetweenPhases);
    }

    private void Update()
    {
        CheckForTimerAdvancement();
    }

    private void CheckForTimerAdvancement()
    {
        if (Time.time >= _timeForTimerAdvancement)
        {
            _timerAdvancementsRemainingInPhase--;
            _timeForTimerAdvancement = Time.time + _timeBetweenTimerAdvancements;
            OnTimerAdvancement?.Invoke(_timerAdvancementsRemainingInPhase);
            //Debug.Log($"Tick: {_timerAdvancementsRemainingInPhase} second before rotation.");
            
            if (_timerAdvancementsRemainingInPhase <= 0)
            {
                RotatePhase();
                _timerAdvancementsRemainingInPhase = Mathf.RoundToInt(_timeBetweenPhases);
            }

        }
    }
    private void RotatePhase()
    {
        //Increment Phase
        CurrentPhase++;
        if (CurrentPhase == Phase.Wraparound)
        {
            CurrentPhase = Phase.A_mobility;
        }

        //"Lerp" into new enemy timescale
        int currentPhaseAsInt = (int)CurrentPhase;
       DOTween.To(() => _currentEnemyTimeScale, x => _currentEnemyTimeScale = x,
           _enemyTimeScales[currentPhaseAsInt], _timeToLerptoNewTimescale);

        //Tell everything about the new phase
        OnNewPhase?.Invoke(CurrentPhase);
        //Debug.Log($"Gong: Changing Phase to {CurrentPhase}");

        //The UI driver for the timer will listen to OnNewPhase to update UI elements.
    }

    public void DebugInstantPhaseChangeAndTimerReset()
    {
        Debug.Log("Debug: skipping a phase");
        _timeForTimerAdvancement = Time.time;
        _timerAdvancementsRemainingInPhase = 1;
        CheckForTimerAdvancement();
    }


}
