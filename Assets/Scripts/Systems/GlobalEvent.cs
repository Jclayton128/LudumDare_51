using System;
using UnityEngine;

public static class GlobalEvent {
    public enum GlobalEventType {
        GameStart,
        GameOver,
        GameReset,
    }

    static Action<GlobalEventType> OnGlobalEvent;

    public static void Invoke(GlobalEventType eventType) {
        OnGlobalEvent?.Invoke(eventType);
    }

    public static void Subscribe(Action<GlobalEventType> action) {
        OnGlobalEvent += action;
    }

    public static void Unsubscribe(Action<GlobalEventType> action) {
        OnGlobalEvent -= action;
    }
}
