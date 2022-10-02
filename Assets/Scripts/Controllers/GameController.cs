using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameController : MonoBehaviour
{
    UIController _uiController;

    public Action<GameObject> OnPlayerStartsRun;
    public Action<GameObject> OnPlayerDies;

    //state
    public bool IsGameRunning = false;
    GameObject _currentPlayer;
    public GameObject CurrentPlayer { get => _currentPlayer; }

    private void Awake()
    {
        _uiController = GetComponent<UIController>();
    }

    private void Start()
    {
        _uiController.SetContext(UIController.Context.Pregame);
    }

    public void StartNewGame()
    {
        _currentPlayer = GameObject.FindGameObjectWithTag("Player");
        _currentPlayer.GetComponent<StatsHandler>().OnPlayerDying += HandlePlayerDying;
        
        _uiController.SetContext(UIController.Context.InGame);
        IsGameRunning = true;
        OnPlayerStartsRun?.Invoke(_currentPlayer);
    }

    public void HandlePlayerDying()
    {
        _uiController.SetContext(UIController.Context.PostGame);
        _currentPlayer.GetComponent<StatsHandler>().OnPlayerDying -= HandlePlayerDying;
        IsGameRunning = false;
    }

    public void HandleRestartMetaGameLoop()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
