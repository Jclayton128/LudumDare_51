using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

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
        _uiController.SetContext(UIController.Context.PostGame);
        _currentPlayer.GetComponent<StatsHandler>().OnPlayerDying -= HandlePlayerDying;
        IsGameRunning = false;
        _timeController.StopTimer();
        GlobalEvent.Invoke(GlobalEvent.GlobalEventType.GameOver);
    }

    public void HandleRestartMetaGameLoop() {
        GlobalEvent.Invoke(GlobalEvent.GlobalEventType.GameReset);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
