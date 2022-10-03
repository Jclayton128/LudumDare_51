using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundUIDriver : MonoBehaviour
{
    TimeController timeController;
    [SerializeField] Color[] _phaseColors = new Color[3];

    [SerializeField] ParticleSystem _upSystem = null;
    [SerializeField] ParticleSystem _downSystem = null;
    ParticleSystem.MainModule _upMain;
    ParticleSystem.MainModule _downMain;
    [SerializeField] int _count = 1000;

    private void Awake()
    {
        timeController = FindObjectOfType<TimeController>();
        timeController.OnTimerAdvancement += HandleOnTimerAdvanced;
        timeController.OnNewPhase += HandlePhaseChange;

        _upMain = _upSystem.main;
        _downMain = _downSystem.main;
    }

    private void HandleOnTimerAdvanced(int count)
    {
        if (count % 2 == 0)
        {
            _upSystem.Emit(_count);
            
        }
        else
        {
            _downSystem.Emit(_count);
        }
    }

    private void HandlePhaseChange(TimeController.Phase newPhase)
    {
        Debug.Log($"new phase color: {_phaseColors[(int)newPhase]}");
        _upMain.startColor = _phaseColors[ (int)newPhase];
        _downMain.startColor = _phaseColors[(int)newPhase];
    }
}
