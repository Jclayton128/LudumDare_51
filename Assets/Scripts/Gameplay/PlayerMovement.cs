using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    //[SerializeField] float moveSpeed = 5f;

    StatsHandler statsHandler;
    TimeController timeController;
    InputController input;

    Vector2 previousPosition;
    Vector2 velocity;

    public Vector2 Velocity => velocity;

    void Awake() {
        input = GetComponent<InputController>();
        statsHandler = GetComponent<StatsHandler>();
        timeController = FindObjectOfType<TimeController>();
    }

    void Update() {
        previousPosition = transform.position;
        transform.position += (Vector3)input.Move * statsHandler.MoveSpeed * timeController.PlayerTimeScale * Time.deltaTime;
        velocity = ((Vector2)transform.position - previousPosition) / Time.deltaTime;
    }
}
