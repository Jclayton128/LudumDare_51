using UnityEngine;

public class EnemyAnimator : MonoBehaviour {
    [Tooltip("Colors to cycle through")]
    [SerializeField] Color[] colors;

    [Tooltip("Scale mods to cycle through")]
    [SerializeField] Vector2[] scaleMods;

    [SerializeField] float cycleTime = 2.5f;

    [Header("Agro")]

    [SerializeField] bool isAgro = false;
    [SerializeField] float agroShakeMod = 0.5f;

    float t = 0f;
    int colorIndex;
    int scaleModIndex;

    Vector2 initialScale;
    Vector2 initialPosition;
    Color currentColor;
    Color nextColor;
    Vector2 currentScaleMod;
    Vector2 nextScaleMod;

    SpriteRenderer spriteRenderer;

    public void SetAgro(bool value) {
        isAgro = value;
    }

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;
        initialPosition = transform.position;
    }

    void Update() {
        CycleColors();
        CycleScaleMods();
        HandleAgroShake();

        t += Time.deltaTime / cycleTime;

        if (t >= 1f) {
            t = 0f;
            colorIndex = GetNextIndex(colorIndex, colors.Length);
            scaleModIndex = GetNextIndex(scaleModIndex, scaleMods.Length);
        }
    }

    void CycleColors() {
        if (colors.Length < 2) return;
        currentColor = colors[colorIndex];
        nextColor = colors[GetNextIndex(colorIndex, colors.Length)];
        spriteRenderer.color = Color.Lerp(currentColor, nextColor, Mathf.Clamp01(t));
    }

    void CycleScaleMods() {
        if (scaleMods.Length < 2) return;
        currentScaleMod = scaleMods[scaleModIndex];
        nextScaleMod = scaleMods[GetNextIndex(scaleModIndex, scaleMods.Length)];
        Vector2 scaleMod = Vector2.Lerp(currentScaleMod, nextScaleMod, t);
        transform.localScale = new Vector3(initialScale.x * scaleMod.x, initialScale.y * scaleMod.y, 1f);
    }

    void HandleAgroShake() {
        if (!isAgro) return;
        // transform.position = transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * agroShakeMod * 0.05f;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetVector("_Offset", UnityEngine.Random.insideUnitCircle * agroShakeMod * 0.05f);
        spriteRenderer.SetPropertyBlock(mpb);
    }

    int GetNextIndex(int index, int collectionLength) {
        if (collectionLength == 0) return 0;
        return (index + 1) % collectionLength;
    }
}
