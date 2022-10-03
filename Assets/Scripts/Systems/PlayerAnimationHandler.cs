using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAnimationHandler : MonoBehaviour
{
    [Tooltip("0: Top, 1: Left, 2: Right")]
    [SerializeField] Transform[] _playerPods = null;

    [SerializeField] Transform[] _inPositions = null;
    [SerializeField] Transform[] _outPositions = null;

    [SerializeField] float _deployTime = 0.7f;

    Tween[] _positionTweens = new Tween[3];
    Tween[] _rotateTweens = new Tween[3];
    Vector3 _inwardFacing = new Vector3(0, 0, 180f);

    private void Awake()
    {
        TimeController tc = FindObjectOfType<TimeController>();
        tc.OnNewPhase += HandlePhaseChange;

        HandlePhaseChange(TimeController.Phase.A_mobility);
    }

    private void HandlePhaseChange(TimeController.Phase newPhase)
    {
        switch (newPhase)
        {
            case TimeController.Phase.A_mobility:
                ExtendPods(0f);
                RotatePods(false, _deployTime);
                RetractPods(_deployTime*2f);
                break;

            case TimeController.Phase.B_firepower:
                ExtendPods(0f);
                RotatePods(true, _deployTime);

                break;

            case TimeController.Phase.C_healing:
                RetractPods(0f);
                break;





        }
    }

    private void ExtendPods(float delay)
    {
        for (int i = 0; i < _playerPods.Length; i++)
        {
            _positionTweens[i].Kill();
            _positionTweens[i] = _playerPods[i].transform.DOLocalMove(
                _outPositions[i].localPosition, _deployTime)
                .SetEase(Ease.InOutCubic)
                .SetUpdate(false)
                .SetDelay(delay);
        }
    }

    private void RetractPods(float delay)
    {
        for (int i = 0; i < _playerPods.Length; i++)
        {
            _positionTweens[i].Kill();
            _positionTweens[i] = _playerPods[i].transform.DOLocalMove(
                _inPositions[i].localPosition, _deployTime)
                .SetEase(Ease.InOutCubic)
                .SetUpdate(false)
                .SetDelay(delay);
        }
    }

    private void RotatePods(bool shouldPointInward, float delay)
    {
        if (shouldPointInward)
        {
            for (int i = 0; i < _playerPods.Length; i++)
            {
                _rotateTweens[i].Kill();
                _rotateTweens[i] = _playerPods[i].transform.DOLocalRotate(
                    _inwardFacing, _deployTime)
                    .SetEase(Ease.InSine)
                    .SetUpdate(false)
                    .SetDelay(delay); ;
            }
        }
        else
        {
            for (int i = 0; i < _playerPods.Length; i++)
            {
                _rotateTweens[i].Kill();
                _rotateTweens[i] = _playerPods[i].transform.DOLocalRotate(
                    Vector3.zero, _deployTime)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(false)
                    .SetDelay(delay); ;
            }
        }
    }
}
