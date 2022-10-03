using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject _startPanel = null;
    [SerializeField] GameObject _timerPanel = null;
    [SerializeField] GameObject _shieldPanel = null;
    [SerializeField] GameObject _healthPanel = null;
    [SerializeField] GameObject _endgamePanel = null;
    [SerializeField] GameObject _audioSettingsPanel = null;

    [SerializeField] TextMeshProUGUI _killCountTMP = null;
    [SerializeField] TextMeshProUGUI _phaseCountTMP = null;

    [SerializeField] GameObject _statsSubpanel = null;
    [SerializeField] GameObject _creditsSubpanel = null;
    bool _isStatsToggled = true;

    public enum Context {
        Pregame,
        InGame,
        PostGame,
        AudioSettings,
    }

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
                _audioSettingsPanel.SetActive(false);
                break;

            case Context.InGame:
                _startPanel.SetActive(false);
                _timerPanel.SetActive(true);
                _shieldPanel.SetActive(true);
                _healthPanel.SetActive(true);
                _endgamePanel.SetActive(false);
                _audioSettingsPanel.SetActive(false);
                break;

            case Context.PostGame:
                _startPanel.SetActive(false);
                _timerPanel.SetActive(false);
                _shieldPanel.SetActive(false);
                _healthPanel.SetActive(false);
                _endgamePanel.SetActive(true);
                _audioSettingsPanel.SetActive(false);


                int killcount = FindObjectOfType<EnemyController>().GetKillCount();
                _killCountTMP.text = $"Enemies Dispatched: {killcount}";

                int phaseCount = FindObjectOfType<TimeController>().GetPhaseCount();
                _phaseCountTMP.text = $"Phases Survived: {phaseCount}";

                break;

            case Context.AudioSettings:
                _startPanel.SetActive(false);
                _timerPanel.SetActive(false);
                _shieldPanel.SetActive(false);
                _healthPanel.SetActive(false);
                _endgamePanel.SetActive(false);
                _audioSettingsPanel.SetActive(true);
                break;
        }
    }

    public void HandleCreditsButtonClick()
    {
        _isStatsToggled = !_isStatsToggled;
        _creditsSubpanel.SetActive(!_isStatsToggled);
        _statsSubpanel.SetActive(_isStatsToggled);

    }
}
