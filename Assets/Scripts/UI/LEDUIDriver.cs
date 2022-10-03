using UnityEngine;
using UnityEngine.UI;

public class LEDUIDriver : MonoBehaviour {
    [SerializeField] Color colorBeat;
    [SerializeField] Color colorActive;

    [SerializeField] Image ledA;
    [SerializeField] Image ledB;
    [SerializeField] Image ledC;
    [SerializeField] Image ledD;

    const float timeInMeasure = 2.5f;

    Color initialColor;
    bool isGameStarted = false;
    int currentBeat = 0; // 0 through 3

    void OnEnable() {
        GlobalEvent.Subscribe(OnGlobalEvent);
    }

    void OnDisable() {
        GlobalEvent.Unsubscribe(OnGlobalEvent);
    }

    void OnGlobalEvent(GlobalEvent.GlobalEventType eventType) {
        switch (eventType) {
            case GlobalEvent.GlobalEventType.GameStart:
                isGameStarted = true;
                break;
            case GlobalEvent.GlobalEventType.GameOver:
            case GlobalEvent.GlobalEventType.GameReset:
                isGameStarted = false;
                break;
        }
    }

    void Start() {
        initialColor = ledA.color;
    }

    void Update() {
        currentBeat = Mathf.FloorToInt((AudioStats.currentPlaybackTime / (timeInMeasure * 0.25f)) % 4);
        if (isGameStarted) {
            ledA.color = currentBeat >= 0 ? colorActive : initialColor;
            ledB.color = currentBeat >= 1 ? colorActive : initialColor;
            ledC.color = currentBeat >= 2 ? colorActive : initialColor;
            ledD.color = currentBeat >= 3 ? colorActive : initialColor;
        } else {
            ledA.color = currentBeat == 0 ? colorBeat : initialColor;
            ledB.color = currentBeat == 1 ? colorBeat : initialColor;
            ledC.color = currentBeat == 2 ? colorBeat : initialColor;
            ledD.color = currentBeat == 3 ? colorBeat : initialColor;
        }
    }
}