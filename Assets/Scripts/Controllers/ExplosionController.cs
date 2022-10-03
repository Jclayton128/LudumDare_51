using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    List<ParticleSystem> _activeExplosions = new List<ParticleSystem> ();
    Queue<ParticleSystem> _pooledExplosions = new Queue<ParticleSystem> ();

    [SerializeField] ParticleSystem _explosionPrefab = null;

    [Tooltip("This multiplies the number of particle emitted from an explosion")]
    [SerializeField] float _gloryFactor = 1f;

    internal void RequestExplosion(float amount, Vector3 position, Color color)
    {
        //TODO play explosion audio clip here.

        ParticleSystem explosion;
        if (_pooledExplosions.Count == 0)
        {
            explosion = Instantiate(_explosionPrefab);
            explosion.GetComponent<ExplosionParticleHandler>().
                Initialize(this);
        }
        else
        {
            explosion = _pooledExplosions.Dequeue();
            explosion.gameObject.SetActive(true);
        }
        ParticleSystem.MainModule main = explosion.main;
        main.startColor = color;
        int emissionCount = Mathf.RoundToInt((amount + 0.5f) * _gloryFactor);
        explosion.Emit(emissionCount);
        explosion.transform.position = position;
    }

    public void ReturnExpiredExplosion(ParticleSystem oldExplosion)
    {

        _activeExplosions.Remove(oldExplosion);
        _pooledExplosions.Enqueue(oldExplosion);
        oldExplosion.gameObject.SetActive(false);

    }
}
