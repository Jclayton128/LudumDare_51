using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class TimeController : MonoBehaviour {
    #region Settings/Parameters
    public enum Phase { A_mobility, B_firepower, C_healing, Wraparound }
    public Action<int> OnTimerAdvancement;
    public Action<Phase> OnNewPhase;

    const float _timeBetweenPhases = 10f; // Connection to the LD theme right here!
    const float _timeBetweenTimerAdvancements = 1f;
    const float _timePerSongMeasure = 2.5f;

    [Tooltip("These correspond to Phase A, Phase B, and Phase C, in order")]
    [SerializeField]
    private float[] _enemyTimeScales = new float[3]
    {
        0.5f, 1.0f, 0.1f
    };

    [Tooltip("These correspond to Phase A, Phase B, and Phase C, in order")]
    [SerializeField]
    private float[] _playerTimeScales = new float[3]
    {
            1.0f, 1.0f, 0.1f
    };
    float _timeToLerptoNewTimescale = 1f;

    bool isTimerActive = false;
    float timeToStart = Mathf.Infinity;

    #endregion

    #region State
    [SerializeField] private float _currentEnemyTimeScale;
    public float EnemyTimeScale {
        get => _currentEnemyTimeScale;
    }

    [SerializeField] private float _currentPlayerTimeScale = 1;
    public float PlayerTimeScale {
        get => _currentPlayerTimeScale;
    }

    // float _timeToRotatePhases;
    float _timeForTimerAdvancement;
    int _timerAdvancementsRemainingInPhase;
    public Phase CurrentPhase { get; private set; } = TimeController.Phase.A_mobility;

    public bool IsPhaseA => CurrentPhase == TimeController.Phase.A_mobility;
    public bool IsPhaseB => CurrentPhase == TimeController.Phase.B_firepower;
    public bool IsPhaseC => CurrentPhase == TimeController.Phase.C_healing;

    #endregion

    public float StartTimer() {
        // When triggered, FMOD will advance to the GAME_START sound upon completion of the current measure. The GAME_START sound is exactly 1 measure in duration.
        float timeLeftInCurrentMeasure = _timePerSongMeasure - Time.time % _timePerSongMeasure;
        float timeToWaitForSongSync = timeLeftInCurrentMeasure + _timePerSongMeasure;
        timeToStart = Time.time + timeToWaitForSongSync;

        // _timeToRotatePhases = Time.time + _timeBetweenPhases;
        _timeForTimerAdvancement = Time.time + timeToWaitForSongSync + _timeBetweenTimerAdvancements;
        _timerAdvancementsRemainingInPhase = Mathf.RoundToInt(_timeBetweenPhases);

        return timeToWaitForSongSync;
    }

    public void StopTimer() {
        isTimerActive = false;
    }

    private void Start() {
        // _timeToRotatePhases = Time.time + _timeBetweenPhases;
        _timeForTimerAdvancement = Time.time + _timeBetweenTimerAdvancements;
        _timerAdvancementsRemainingInPhase = Mathf.RoundToInt(_timeBetweenPhases);
        isTimerActive = false;
        timeToStart = Mathf.Infinity;
    }

    private void Update() {
        TryStartTimer();
        if (!isTimerActive) return;
        CheckForTimerAdvancement();
    }

    private void TryStartTimer() {
        if (timeToStart > Time.time) return;
        isTimerActive = true;
    }

    private void CheckForTimerAdvancement() {
        if (Time.time >= _timeForTimerAdvancement) {
            _timerAdvancementsRemainingInPhase--;
            _timeForTimerAdvancement = Time.time + _timeBetweenTimerAdvancements;
            OnTimerAdvancement?.Invoke(_timerAdvancementsRemainingInPhase);
            //Debug.Log($"Tick: {_timerAdvancementsRemainingInPhase} second before rotation.");

            if (_timerAdvancementsRemainingInPhase <= 0) {
                RotatePhase();
                _timerAdvancementsRemainingInPhase = Mathf.RoundToInt(_timeBetweenPhases);
            }

        }
    }
    private void RotatePhase() {
        //Increment Phase
        CurrentPhase++;
        if (CurrentPhase == Phase.Wraparound) {
            CurrentPhase = Phase.A_mobility;
        }

        //"Lerp" into new enemy timescale
        int currentPhaseAsInt = (int)CurrentPhase;
        DOTween.To(() => _currentEnemyTimeScale, x => _currentEnemyTimeScale = x,
            _enemyTimeScales[currentPhaseAsInt], _timeToLerptoNewTimescale);


        //"Lerp" into new enemy timescale
        DOTween.To(() => _currentPlayerTimeScale, x => _currentPlayerTimeScale = x,
            _playerTimeScales[currentPhaseAsInt], _timeToLerptoNewTimescale);

        //Tell everything about the new phase
        OnNewPhase?.Invoke(CurrentPhase);

    }

    public void DebugInstantPhaseChangeAndTimerReset() {
        Debug.Log("Debug: skipping a phase");
        _timeForTimerAdvancement = Time.time;
        _timerAdvancementsRemainingInPhase = 1;
        CheckForTimerAdvancement();
    }


}
