using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    UIController _uiController;

    public Action<GameObject> OnPlayerSpawned;
    public Action<GameObject> OnPlayerDespawned;

    //settings
    [SerializeField] GameObject _playerPrefab = null;

    //state
    public bool IsGameRunning = false;
    GameObject _currentPlayer;
    public GameObject CurrentPlayer { get => _currentPlayer; }
    StatsHandler _currentPlayerStatsHandler;

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
        _currentPlayer = Instantiate(_playerPrefab, Vector2.zero, Quaternion.identity);
        _currentPlayer.GetComponent<StatsHandler>().OnPlayerDying += HandlePlayerDying;
        
        _uiController.SetContext(UIController.Context.InGame);
        IsGameRunning = true;
        OnPlayerSpawned?.Invoke(_currentPlayer);
    }

    public void HandlePlayerDying()
    {
        _uiController.SetContext(UIController.Context.PostGame);
        _currentPlayer.GetComponent<StatsHandler>().OnPlayerDying -= HandlePlayerDying;
        IsGameRunning = false;
    }
}
