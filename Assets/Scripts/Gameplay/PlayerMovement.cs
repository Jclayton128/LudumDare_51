using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] float moveSpeed = 5f;

    TimeController timeController;
    InputController input;

    Vector2 previousPosition;
    Vector2 velocity;

    public Vector2 Velocity => velocity;

    void Awake() {
        input = GetComponent<InputController>();
        timeController = FindObjectOfType<TimeController>();
    }

    void Update() {
        previousPosition = transform.position;
        transform.position += (Vector3)input.Move * moveSpeed * timeController.PlayerTimeScale * Time.deltaTime;
        velocity = ((Vector2)transform.position - previousPosition) / Time.deltaTime;
    }
}
