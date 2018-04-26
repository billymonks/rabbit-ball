using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMover : MonoBehaviour {
    public Transform destination;
    private ParticleSystem.Particle[] particles;
    private ParticleSystem ps;
	// Use this for initialization
	void Start () {
        //ps = this.GetComponent<ParticleSystem>();
        //particles = new ParticleSystem.Particle[200];

	}
	
	// Update is called once per frame
	void Update () {
        /*if (!destination)
            return;
        
        ps.GetParticles(particles);
        print(particles[0].position);
        for(int i = 0; i < particles.Length; i++)
        {
            particles[i].
            particles[i].velocity = Vector3.Lerp(particles[i].velocity, (destination.position - particles[i].position), Time.deltaTime * 10f);
            particles[i].position = Vector3.Lerp(particles[i].position, destination.position, Time.deltaTime * 16f);
        }*/
	}
}
