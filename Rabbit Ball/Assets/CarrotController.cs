using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotController : MonoBehaviour {
    public ParticleSystem collectionParticle;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ParticleSystem ps = GameObject.Instantiate<ParticleSystem>(collectionParticle);
            ps.transform.position = transform.position;
            GameObject.Destroy(ps, 5f);
            GameObject.Destroy(gameObject);

        }
    }
}
