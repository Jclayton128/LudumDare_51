using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float moveAccel = 10f;

    TimeController timeController;
    InputController input;
    StatsHandler stats;

    Vector2 desiredHeading;
    Vector2 previousPosition;
    Vector2 velocity;

    public Vector2 Velocity => velocity;

    void Awake() {
        input = GetComponent<InputController>();
        stats = GetComponent<StatsHandler>();
        timeController = FindObjectOfType<TimeController>();
    }

    void Update() {
        desiredHeading = Vector2.MoveTowards(stats.IsAlive ? input.Move : Vector2.zero, desiredHeading, moveAccel * Time.deltaTime);
        previousPosition = transform.position;
        transform.position += (Vector3)desiredHeading * moveSpeed * timeController.PlayerTimeScale * Time.deltaTime;
        velocity = ((Vector2)transform.position - previousPosition) / Time.deltaTime;
    }
}
