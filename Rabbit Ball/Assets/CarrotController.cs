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
            //ParticleMover mover = ps.GetComponent<ParticleMover>();
            //mover.destination = other.transform;
            ps.transform.position = transform.position;
            //ScoreKeeper.carrotCount++;
            //ScoreKeeper.UpdateCarrotCount(1);
            ScoreKeeper.keeper.UpdateCarrotCount(1);
            GameObject.Destroy(ps.gameObject, 5f);
            GameObject.Destroy(gameObject);

        }
    }
}
