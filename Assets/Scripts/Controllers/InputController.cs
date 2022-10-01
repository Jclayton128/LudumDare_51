using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    #region State
    Vector2 move;
    Vector2 look; // note - look only applies to gamepad R stick
    Vector2 mousePositionScreen;
    Vector2 mousePositionWorld; // mouse position in world relative to this GameObject's transform position
    Vector2 mousePositionNormalized; // (0,0) lower LH corner -> (1,1) upper RH corner
    #endregion

    #region PublicAccessors
    public Vector2 Move => move;
    public Vector2 Look => look;
    public Vector2 MousePositionNormalized => mousePositionNormalized;
    #endregion

    #region Cached
    new Camera camera;
    #endregion

    void Update()
    {
        if (camera == null) camera = Camera.main;
        mousePositionScreen = Mouse.current.position.ReadValue();
        mousePositionWorld = camera.ScreenToWorldPoint(mousePositionScreen) - transform.position;
        mousePositionNormalized = camera.ScreenToViewportPoint(mousePositionScreen);

        // Debug.Log($"move={move}, look={look}, mouseNorm={mousePositionNormalized}, mouseWorld={mousePositionWorld}");
    }

    void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }
}
