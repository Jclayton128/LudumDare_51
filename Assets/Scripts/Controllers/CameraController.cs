using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCamera _cvc;
    StatsHandler _sh;

    private void Awake()
    {
        _cvc = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
        _sh = FindObjectOfType<StatsHandler>();
        _sh.OnReceiveDamage += HandleOnDamageReceived;

    }

    private void HandleOnDamageReceived(int system, float f, bool b)
    {
        if (system != 0) return;
        Debug.Log("changing zoom?");
        _cvc.m_Lens.OrthographicSize = _sh.CameraZoom;
    }
   


}
