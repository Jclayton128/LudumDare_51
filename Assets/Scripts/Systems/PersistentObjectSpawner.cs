using UnityEngine;

public class PersistentObjectSpawner : MonoBehaviour {
    [SerializeField] GameObject persistentObjects;

    static bool hasSpawned = false;

    private void Awake() {
        if (hasSpawned) return;
        GameObject spawned = Instantiate(persistentObjects);
        DontDestroyOnLoad(spawned);
        hasSpawned = true;
    }
}
