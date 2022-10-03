
using UnityEngine;
using FMODUnity;

public class Audio : MonoBehaviour {
    [SerializeField] StudioGlobalParameterTrigger triggerGameStart;
    [SerializeField] StudioGlobalParameterTrigger triggerGameOver;

    const string TRIGGER_GAME_START = "IsGameStarted";
    const string TRIGGER_GAME_OVER = "IsGameOver";

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
                TriggerGameStart();
                break;
            case GlobalEvent.GlobalEventType.GameOver:
                TriggerGameOver();
                break;
            case GlobalEvent.GlobalEventType.GameReset:
                TriggerGameReset();
                break;
        }
    }

    void TriggerGameStart() {
        emitter.SetParameter(TRIGGER_GAME_OVER, 0f);
        emitter.SetParameter(TRIGGER_GAME_START, 1f);
    }

    void TriggerGameOver() {
        emitter.SetParameter(TRIGGER_GAME_OVER, 1f);
    }

    void TriggerGameReset() {
        emitter.SetParameter(TRIGGER_GAME_START, 0f);
    }
}
