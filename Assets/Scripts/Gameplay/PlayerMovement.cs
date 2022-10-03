using UnityEngine;

public class PlayerMovement : MonoBehaviour, IForceReceiver {
    [SerializeField] bool debug = false;
    [SerializeField] float moveAccel = 10f;

    [Tooltip("How fast outside forces decay - larger value = faster decay")]
    [SerializeField] float forceDecayMod = 10f;

    TimeController timeController;
    InputController input;
    StatsHandler stats;

    Vector2 desiredHeading;
    Vector2 previousPosition;
    Vector2 velocity;

    Vector2 otherForces;

    public Vector2 Velocity => velocity;

    public void AddForce(Vector2 force) {
        otherForces += force;
    }

    void Awake() {
        input = GetComponent<InputController>();
        stats = GetComponent<StatsHandler>();
        timeController = FindObjectOfType<TimeController>();
    }

    void Update() {
        Move();
        DecayOutsideForces();
    }

    void Move() {
        desiredHeading = Vector2.MoveTowards(stats.IsAlive ? input.Move : Vector2.zero, desiredHeading, moveAccel * Time.deltaTime);
        previousPosition = transform.position;
        transform.position += (Vector3)desiredHeading * stats.MoveSpeed * timeController.PlayerTimeScale * Time.deltaTime;
        transform.position += (Vector3)otherForces * timeController.PlayerTimeScale * Time.deltaTime;
        velocity = ((Vector2)transform.position - previousPosition) / Time.deltaTime;
        if (debug) Debug.Log($"move={input.Move} alive={stats.IsAlive} desiredHeading={desiredHeading}");
        Debug.DrawLine(transform.position, transform.position + (Vector3)velocity, Color.blue, 0.1f);
    }

    void DecayOutsideForces() {
        otherForces = Vector2.MoveTowards(otherForces, Vector2.zero, Time.deltaTime * forceDecayMod);
    }
}
