using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticleHandler : MonoBehaviour
{
    ExplosionController _explosionController;

    internal void Initialize(ExplosionController explosionController)
    {
        _explosionController = explosionController;
    }

    public void OnParticleSystemStopped()
    {
        _explosionController.ReturnExpiredExplosion(this.GetComponent<ParticleSystem>());
    }
}
