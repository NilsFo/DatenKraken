using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleShotgunPlayer : MonoBehaviour
{
    public ParticleSystem myParticleSystem;
    public bool playOnAwake;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
    }

    private void Awake()
    {
        if (playOnAwake)
        {
            PlayParticles();
        }
    }

    public void PlayParticles()
    {
        myParticleSystem.Play();
    }
}