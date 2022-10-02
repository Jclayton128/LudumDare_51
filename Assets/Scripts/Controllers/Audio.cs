using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

public class Audio : MonoBehaviour {
    [SerializeField] StudioGlobalParameterTrigger triggerGameStart;
    [SerializeField] StudioGlobalParameterTrigger triggerGameOver;

    StudioEventEmitter emitter;

    void OnEnable() {
        GlobalEvent.Subscribe(OnGlobalEvent);
    }

    void OnDisable() {
        GlobalEvent.Unsubscribe(OnGlobalEvent);
    }

    private void Awake() {
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    void OnGlobalEvent(GlobalEvent.GlobalEventType eventType) {
        switch (eventType) {
            case GlobalEvent.GlobalEventType.GameStart:
                // TODO: SET FMOD PARAM HERE
                Debug.Log("GLOBAL EVENT --> GAME START");
                triggerGameStart.Value = 1f;
                triggerGameOver.Value = 0f;
                TriggerParameters();
                break;
            case GlobalEvent.GlobalEventType.GameOver:
                // TODO: SET FMOD PARAM HERE
                Debug.Log("GLOBAL EVENT --> GAME OVER");
                triggerGameOver.Value = 1f;
                TriggerParameters();
                break;
            case GlobalEvent.GlobalEventType.GameReset:
                // TODO: SET FMOD PARAM HERE
                Debug.Log("GLOBAL EVENT --> GAME RESET");
                triggerGameStart.Value = 0f;
                triggerGameOver.Value = 0f;
                TriggerParameters();
                break;
        }
    }

    void TriggerParameters() {
        triggerGameStart.TriggerParameters();
        triggerGameOver.TriggerParameters();
    }
}
