using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class InputController : MonoBehaviour {
    [SerializeField][Range(0f, 1f)] float dpadDeadZone = 0.2f;
    [SerializeField] Texture2D mouseReticleTexture;

    #region State
    Vector2 move;
    Vector2 look; // note - look only applies to gamepad R stick
    Vector2 mousePositionScreen;
    Vector2 mousePositionWorld; // mouse position in world relative to this GameObject's transform position
    Vector2 mousePositionNormalized; // (0,0) lower LH corner -> (1,1) upper RH corner
    bool isFirePressed;
    bool isHealing;
    int subsystemIndex;
    #endregion

    #region PublicAccessors
    public Vector2 Move => move;
    public Vector2 Look => look;
    public Vector2 MousePositionScreen => mousePositionScreen;
    public Vector2 MousePositionWorld => mousePositionWorld;
    public Vector2 MousePositionNormalized => mousePositionNormalized;
    public bool IsFirePressed => isFirePressed;
    public bool IsInputKeyboardAndMouse => playerInput.currentControlScheme == "Keyboard&Mouse";
    #endregion

    #region Cached
    new Camera camera;
    StatsHandler stats;
    PlayerInput playerInput;
    InputSystemUIInputModule uiModule;
    string prevControlScheme;
    #endregion

    public void ResetInputSystemBecauseUnityIsDumb() {
        playerInput.SwitchCurrentActionMap("Player");
        StartCoroutine(HaveYouTriedTurningItOffAndOnAgain());
    }

    void Awake() {
        if (camera == null) camera = Camera.main;
        playerInput = GetComponent<PlayerInput>();
        playerInput.camera = camera;
    }

    void Update() {
        if (camera == null) camera = Camera.main;
        mousePositionScreen = Mouse.current.position.ReadValue();
        mousePositionWorld = camera.ScreenToWorldPoint(mousePositionScreen) - transform.position;
        mousePositionNormalized = camera.ScreenToViewportPoint(mousePositionScreen);
        SetMouseStyle();
        // Debug.Log($"move={move}, look={look}, mouseNorm={mousePositionNormalized}, mouseWorld={mousePositionWorld}");
    }

    void OnMove(InputValue value) {
        move = value.Get<Vector2>();
    }

    void OnLook(InputValue value) {
        look = value.Get<Vector2>();
    }

    void OnFire(InputValue value) {
        isFirePressed = value.isPressed;
    }

    void OnHeal(InputValue value) {
        if (!value.isPressed) return;
        if (stats == null) stats = GetComponent<StatsHandler>();
        int subsystemIndex = GetSubsystemRepairIndex(value.Get<Vector2>());
        stats.RepairDamage(subsystemIndex);
    }

    void OnHealUp(InputValue value) { }
    void OnHealDown(InputValue value) { }
    void OnHealLeft(InputValue value) { }
    void OnHealRight(InputValue value) { }

    int GetSubsystemRepairIndex(Vector2 dpad) {
        if (dpad.y > dpadDeadZone) return 0;
        if (dpad.y < -dpadDeadZone) return 1;
        if (dpad.x > dpadDeadZone) return 2;
        return 3;
    }

    void SetMouseStyle() {
        if (mouseReticleTexture == null) return;
        if (playerInput.currentControlScheme == prevControlScheme) return;
        if (!IsInputKeyboardAndMouse) {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return;
        }
        Cursor.SetCursor(mouseReticleTexture, Vector2.zero, CursorMode.Auto);
        prevControlScheme = playerInput.currentControlScheme;
    }

    // UNITY WHYYYYYYY??
    // This fixes a bug where keyboard events were not registered.
    IEnumerator HaveYouTriedTurningItOffAndOnAgain() {
        uiModule = FindObjectOfType<InputSystemUIInputModule>();
        uiModule.enabled = false;
        yield return new WaitForFixedUpdate();
        uiModule.enabled = true;
    }
}
