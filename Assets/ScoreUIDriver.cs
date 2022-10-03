using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ScoreUIDriver : MonoBehaviour
{
    [SerializeField] Image[] _counterImages = null;

    [SerializeField] Color[] _counterColorMenu = new Color[10]
    {
        Color.white,
        Color.white,
        Color.white,
        Color.white,
        Color.white,
        Color.white,
        Color.white,
        Color.white,
        Color.white,
        Color.white
    };

    //state
    int tens = 0;


    private void Awake()
    {
        FindObjectOfType<EnemyController>().OnEnemyDead += HandleEnemyDead;
        for (int i = 0; i < _counterImages.Length; i++)
        {
            _counterImages[i].color = Color.clear;
        }
    }


    private void HandleEnemyDead(int deadEnemies)
    {
        int index = deadEnemies - 1;
        if (index > 0 && (index % 10 == 0))
        {
            tens++;
            if (tens > 10) tens = 0;
        }

        int ones = index % 10;
        _counterImages[ones].color = _counterColorMenu[tens];
    }
}
