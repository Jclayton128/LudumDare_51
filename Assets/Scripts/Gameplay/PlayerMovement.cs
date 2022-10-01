using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    InputController input;

    void Start()
    {
        input = GetComponent<InputController>();
    }

    void Update()
    {
        transform.position += (Vector3)input.Move * moveSpeed * Time.deltaTime;
    }
}
