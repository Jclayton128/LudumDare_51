using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

public class Audio : MonoBehaviour {
    [SerializeField] StudioGlobalParameterTrigger triggerGameStart;
    [SerializeField] StudioGlobalParameterTrigger triggerGameOver;

    StudioEventEmitter emitter;
    FMOD.Studio.EventInstance eventInstance;
    int timelinePositionMs = 0;
    float timelinePosition;

    void OnEnable() {
        GlobalEvent.Subscribe(OnGlobalEvent);
    }

    void OnDisable() {
        GlobalEvent.Unsubscribe(OnGlobalEvent);
    }

    void Awake() {
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        emitter.Preload = true;
    }

    void Start() {
        timelinePositionMs = 0;
        eventInstance = emitter.EventInstance;
    }

    void Update() {
        emitter.EventInstance.getTimelinePosition(out timelinePositionMs);
        timelinePosition = ((float)timelinePositionMs) * 0.001f;
        AudioStats.currentPlaybackTime = timelinePosition;
    }

    void OnGlobalEvent(GlobalEvent.GlobalEventType eventType) {
        switch (eventType) {
            case GlobalEvent.GlobalEventType.GameStart:
                triggerGameStart.Value = 1f;
                triggerGameOver.Value = 0f;
                TriggerParameters();
                break;
            case GlobalEvent.GlobalEventType.GameOver:
                triggerGameOver.Value = 1f;
                TriggerParameters();
                break;
            case GlobalEvent.GlobalEventType.GameReset:
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
