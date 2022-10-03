using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ShieldUIDriver : MonoBehaviour
{
    [Tooltip("0: inner layer, 1: mid layer, 2: outer layer")]
    [SerializeField] Color[] _shieldLayerColors = new Color[3];
    [SerializeField] Image[] _shieldLayers = null;
    [SerializeField] Image _shieldChargeLevel = null;

    
    private void Awake()
    {
        GameController gc = FindObjectOfType<GameController>();
        gc.OnPlayerStartsRun += HandleOnPlayerStartsRun;
        gc.OnPlayerDies += HandleOnPlayerDespawn;

    }
    private void HandleOnPlayerStartsRun(GameObject newPlayer)
    {
        StatsHandler sh = newPlayer.GetComponent<StatsHandler>();
        sh.OnChangeShieldLayerCount += HandleUpdatedShieldLayerCount;
        sh.OnShieldChargeLevelChange += HandleShieldLevelChange;
    }

    private void HandleShieldLevelChange(float newLevel)
    {
        _shieldChargeLevel.fillAmount = newLevel;
    }

    private void HandleOnPlayerDespawn(GameObject despawningPlayer)
    {
        StatsHandler sh = despawningPlayer.GetComponent<StatsHandler>();
        sh.OnChangeShieldLayerCount -= HandleUpdatedShieldLayerCount;
    }

    //Janky implementation. No judgment.
    private void HandleUpdatedShieldLayerCount(int layersRemaining)
    {
        switch (layersRemaining)
        {
            case 3:
                _shieldLayers[0].color = _shieldLayerColors[0];
                _shieldLayers[1].color = _shieldLayerColors[1];
                _shieldLayers[2].color = _shieldLayerColors[2];
                break;

            case 2:
                _shieldLayers[0].color = _shieldLayerColors[0];
                _shieldLayers[1].color = _shieldLayerColors[1];
                _shieldLayers[2].color = Color.clear;
                break;

            case 1:
                _shieldLayers[0].color = _shieldLayerColors[0];
                _shieldLayers[1].color = Color.clear;
                _shieldLayers[2].color = Color.clear;
                break;

            case 0:
                _shieldLayers[0].color = Color.clear;
                _shieldLayers[1].color = Color.clear;
                _shieldLayers[2].color = Color.clear;
                break;


        }
    }

}
