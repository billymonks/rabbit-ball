using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public float force = 10f;
    public int damage = 10;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnCollisionEnter(Collision collision)
	{
        if(collision.gameObject == PlayerController.player){
            PlayerController.player.SendMessage("TakeDamage",
                                                new EnemyHitInfos(
                                                    this.gameObject, collision.contacts[0].point,
                                                    collision.contacts[0].normal, force, damage));
        }
	}

    void Hit() 
    {
        GameObject.Destroy(this.gameObject);
        //kill this
    }
}

public class EnemyHitInfos {
    public GameObject attacker;
    public Vector3 hitPoint, hitDirection;
    public float force;
    public int carrotLoss;
    public EnemyHitInfos(GameObject attacker, Vector3 hitPoint, Vector3 hitDirection, float force, int carrotLoss) {
        this.attacker = attacker;
        this.hitPoint = hitPoint;
        this.hitDirection = hitDirection;
        this.force = force;
        this.carrotLoss = carrotLoss;
    }
}