using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPulsing : MonoBehaviour
{
    TimeController _tc;
    [SerializeField] float _growthScale = 1.2f;
    [SerializeField] float _tempo = 1f;

    Vector3 _startingScale;
    float _ticker = 0;
    bool _isEnlarging = true;
    private void Awake()
    {
        _tc = FindObjectOfType<TimeController>();
        _startingScale = transform.localScale;
    }

    private void Update()
    {
        if (_ticker > 1)
        {
            _isEnlarging = false;
        }
        if (_ticker < 0)
        {
            _isEnlarging = true;
        }

        if (_isEnlarging)
        {
            _ticker += _tempo * Time.deltaTime * _tc.EnemyTimeScale;
        }
        else
        {
            _ticker -= _tempo * Time.deltaTime * _tc.EnemyTimeScale;
        }
        transform.localScale =
                Vector3.Lerp(_startingScale, _startingScale * _growthScale, _ticker);


    }

}
