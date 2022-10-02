using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;

public class GameController : MonoBehaviour {
    UIController _uiController;

    public Action<GameObject> OnPlayerStartsRun;
    public Action<GameObject> OnPlayerDies;

    //state
    public bool IsGameRunning = false;
    GameObject _currentPlayer;
    TimeController _timeController;
    public GameObject CurrentPlayer { get => _currentPlayer; }

    private void Awake() {
        _uiController = GetComponent<UIController>();
        _timeController = GetComponent<TimeController>();
    }

    private void Start() {
        _uiController.SetContext(UIController.Context.Pregame);
        if (_currentPlayer == null) _currentPlayer = GameObject.FindGameObjectWithTag("Player");
        _currentPlayer.gameObject.SetActive(false);
        _timeController.StopTimer();
    }

    public void StartNewGame() {
        GlobalEvent.Invoke(GlobalEvent.GlobalEventType.GameStart);
        float timeToWaitForSongSync = _timeController.StartTimer();
        Invoke("OnStartGameElements", timeToWaitForSongSync);
    }

    void OnStartGameElements() {
        if (_currentPlayer == null) _currentPlayer = GameObject.FindGameObjectWithTag("Player");
        _currentPlayer.GetComponent<StatsHandler>().OnPlayerDying += HandlePlayerDying;
        _currentPlayer.gameObject.SetActive(true);
        _uiController.SetContext(UIController.Context.InGame);
        IsGameRunning = true;
        OnPlayerStartsRun?.Invoke(_currentPlayer);
        GlobalEvent.Invoke(GlobalEvent.GlobalEventType.GameStart);
    }

    public void HandlePlayerDying() {
        _timeController.StopTimer();
        GlobalEvent.Invoke(GlobalEvent.GlobalEventType.GameOver);
        _currentPlayer.GetComponent<StatsHandler>().OnPlayerDying -= HandlePlayerDying;
        IsGameRunning = false;
        StartCoroutine(PlayerDeathTimeline());
    }

    public void HandleRestartMetaGameLoop() {
        GlobalEvent.Invoke(GlobalEvent.GlobalEventType.GameReset);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator PlayerDeathTimeline() {
        Tween timescaleTween;

        timescaleTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.15f, 0.3f);
        yield return timescaleTween.WaitForCompletion();

        yield return new WaitForSecondsRealtime(0.1f);

        timescaleTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, 0.1f);
        yield return timescaleTween.WaitForCompletion();

        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(1f);

        OnGameOverState();
    }

    void OnGameOverState() {
        _uiController.SetContext(UIController.Context.PostGame);
    }
}
