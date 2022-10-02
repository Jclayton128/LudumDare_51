using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class AnnouncerUIDriver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _announcerTMP = null;

    [SerializeField] float _announcerFontScale = 1.5f;
    [SerializeField] float _announceTime = 1.3f;
    [SerializeField] Color _announcerColor = Color.white;


    [SerializeField]
    string[] _phaseNames = new string[3]
    {
        "Run",
        "Fight",
        "Heal"
    };

    //state
    Tween _colorTween;
    Tween _sizeTween;
    float _initialFontSize;

    private void Awake()
    {
        TimeController tc = FindObjectOfType<TimeController>();
        tc.OnNewPhase += AnnounceNewPhase;
        _initialFontSize = _announcerTMP.fontSize;
    }

    private void AnnounceNewPhase(TimeController.Phase newPhase)
    {

        _announcerTMP.text = _phaseNames[(int)newPhase];
        _announcerTMP.color = _announcerColor;
        _colorTween.Kill();
        _colorTween = _announcerTMP.DOColor(Color.clear, _announceTime);

        _sizeTween.Kill();
        _announcerTMP.fontSize = _initialFontSize;
        _sizeTween = DOTween.To(() =>
       _announcerTMP.fontSize, x => _announcerTMP.fontSize = x,
        _initialFontSize * _announcerFontScale, _announceTime);
    }
}
