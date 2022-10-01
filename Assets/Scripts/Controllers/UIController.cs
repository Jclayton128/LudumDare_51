using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject _startPanel = null;
    [SerializeField] GameObject _timerPanel = null;
    [SerializeField] GameObject _shieldPanel = null;
    [SerializeField] GameObject _healthPanel = null;
    [SerializeField] GameObject _endgamePanel = null;


    public enum Context {Pregame, InGame, PostGame }

    public void SetContext(Context newContext)
    {
        switch (newContext)
        {
            case Context.Pregame:
                _startPanel.SetActive(true);
                _timerPanel.SetActive(false);
                _shieldPanel.SetActive(false);
                _healthPanel.SetActive(false);
                _endgamePanel.SetActive(false);
                break;

            case Context.InGame:
                _startPanel.SetActive(false);
                _timerPanel.SetActive(true);
                _shieldPanel.SetActive(true);
                _healthPanel.SetActive(true);
                _endgamePanel.SetActive(false);
                break;

            case Context.PostGame:
                _startPanel.SetActive(false);
                _timerPanel.SetActive(false);
                _shieldPanel.SetActive(false);
                _healthPanel.SetActive(false);
                _endgamePanel.SetActive(true);
                break;



        }
    }
}
