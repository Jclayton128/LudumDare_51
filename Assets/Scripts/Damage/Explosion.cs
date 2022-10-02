using UnityEngine;

public class Explosion : MonoBehaviour {

    [SerializeField] ParticleSystem particleFX;
    // [Ser]

    public void Activate() {
        gameObject.SetActive(true);
        particleFX.Play();
    }

    private void Awake() {
        particleFX = GetComponent<ParticleSystem>();
    }
}