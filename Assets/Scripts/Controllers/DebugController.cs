using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{

    TimeController _timeController;

    private void Awake()
    {
        Debug.LogError("Don't forget to disable these hard-coded debug tools in final build!");
        _timeController = GetComponent<TimeController>();
    }

    private void Update()
    {
        //ListenForInstaPhaseSkip();
        //ListenForInstaDamage();
    }

    private void ListenForInstaDamage()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            FindObjectOfType<StatsHandler>().ReceivedTargetedBulletImpact(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            FindObjectOfType<StatsHandler>().ReceivedTargetedBulletImpact(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FindObjectOfType<StatsHandler>().ReceivedTargetedBulletImpact(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FindObjectOfType<StatsHandler>().ReceivedTargetedBulletImpact(3);
        }
    }

    private void ListenForInstaPhaseSkip()
    {
        //DEBUG remove this in final build!
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    _timeController.DebugInstantPhaseChangeAndTimerReset();
        //}
    }
}
