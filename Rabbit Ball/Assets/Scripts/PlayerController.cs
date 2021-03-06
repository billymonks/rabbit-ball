﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static GameObject player;
	private int state = 0;

	public Vector3 forwardValue;

	private Vector3 prevPos;

	private Rigidbody rb;

    private Collider c;

	public float cameraTurnSpeed = 5f;

	private float hitPower = 0f, maxHitPower = 100f, chargeSpeed = 1f;

	private float stoppedTimer = 0f, stoppedLength = 1f;

	public float powerMultiplier = 0.2f;

	public float groundActionLength = 3f;
	public float airActionLength = 3f;
    private float hopPower = 0f;
	private float groundActionTimer = 0f;
	private float airActionTimer = 0f;
	private bool groundAction = false, airAction = false, groundActionReady = false, airActionReady = false;

	public float actionRunSpeed = 10f;
	public float actionJumpPower = 30f;

	private Vector3 startPos = Vector3.zero;

    public PhysicMaterial slippery, frictiony;

    public GameObject body;

    public float slowRotation = 0.5f, fastRotation = 3f;

    private Transform transform;
    private bool grounded = false;
    private float rotationSpeed = 1f;

    public Transform cameraRigTransform;
    private float cameraRotationX, cameraRotationY;

	public float mouseSpeed = 1f;

    private List<ContactPoint> contacts = new List<ContactPoint>();

    private float jumpTimer;
    private float jumpValidTime = 0.4f;

    public ParticleSystem jumpParticles, rollParticles;

    private float xRotation = 0f;

    private Transform particleHolder;

    private ParticleSystem.EmissionModule rollEmitter;

    private float hurtTimer = 0f;
    public float hurtTime = 2f;

    private bool rolling = false;
	// Use this for initialization
	void Start () {
        transform = body.transform;
		prevPos = transform.position;
		forwardValue = transform.forward;
        particleHolder = jumpParticles.transform.parent;

		startPos = transform.position;

		rb = body.GetComponent<Rigidbody> ();
        c = body.GetComponent<Collider>();

        cameraRotationX = cameraRigTransform.eulerAngles.x;
        cameraRotationY = cameraRigTransform.eulerAngles.y;

        jumpTimer = jumpValidTime;

        rollEmitter = rollParticles.emission;

        player = gameObject;
    }
	
	// Update is called once per frame
	void Update () {
		float turnAmount = 0;

        particleHolder.position = transform.position;
        particleHolder.rotation = Quaternion.LookRotation(forwardValue, Vector3.up);

        turnAmount = Input.GetAxis ("Mouse X") * mouseSpeed + Input.GetAxis("Horizontal") * 2f;
		cameraRotationY += turnAmount;
		cameraRigTransform.RotateAround(body.transform.position, Vector3.up, turnAmount);

        //Debug.DrawRay(transform.position + Vector3.up * 2f, forwardValue, Color.red);

        Ray r = new Ray(transform.position, -transform.up * 1f);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 1f))
        {
            grounded = true;
            rotationSpeed = fastRotation;
        } else
        {
            grounded = false;
            rotationSpeed = slowRotation;

            Ray down = new Ray(transform.position, -Vector3.up * 1f);

            if (Physics.Raycast(down, out hit, 1f))
            {
                grounded = true;
                rotationSpeed = fastRotation;
                transform.eulerAngles = new Vector3( Mathf.Lerp(transform.eulerAngles.x, 0, Time.deltaTime * 4f), transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }

        if (state == 0) {
            BeginRolling();
		} else if (state == 1) {
			UpdateForwardFromMovement ();

			if (Input.GetKey (KeyCode.Space)) {
				if (groundActionReady) {
					BeginGroundAction ();
				}
			}

				if (groundAction)
					DoGroundAction ();
                else
                    transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * 4f);
        } else if (state == 2) { //hurt
            hurtTimer -= Time.deltaTime;
            if (hurtTimer <= 0f){
                hurtTimer = 0f;
                Recover();
            }
        }

        if (grounded)
        {
            rollEmitter.enabled = true;
        }
        else
        {
            rollEmitter.enabled = false;
        }
	}

    private void Recover() {
        rb.freezeRotation = true;
        state = 0;
    }

	private void UpdateForwardFromMovement() {
        float yRotation = transform.eulerAngles.y;

        float rotation = Mathf.Clamp(Vector3.SignedAngle(Quaternion.Euler(0f, yRotation, 0f) * Vector3.forward, Quaternion.Euler(0f, cameraRotationY, 0f) * Vector3.forward, Vector3.up), -rotationSpeed, rotationSpeed);

        rb.velocity = Quaternion.AngleAxis(rotation, transform.up) * rb.velocity;

        if ((transform.position - prevPos).magnitude > 1) {
			forwardValue = Vector3.Lerp(forwardValue, 
				(transform.position - prevPos).normalized, 
                                        cameraTurnSpeed * Time.deltaTime).normalized;

            forwardValue = (Quaternion.AngleAxis(rotation * 3f, transform.up) * forwardValue).normalized;

            prevPos = transform.position;
		} else
        {
            xRotation = 0f;
            forwardValue = (Quaternion.AngleAxis(rotation * 3f, transform.up) * forwardValue).normalized;

        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation (forwardValue) * Quaternion.Euler(xRotation, 0f, 0f), Time.deltaTime * 5f);
	}

    private void SetRotationInstantly() {
        transform.rotation = Quaternion.LookRotation(forwardValue) * Quaternion.Euler(xRotation, 0f, 0f);
    }

	private void BeginRolling() {
		state = 1;
		stoppedTimer = stoppedLength;
		groundActionReady = true;
		airActionReady = true;
		rb.AddForce (forwardValue * hitPower * powerMultiplier, ForceMode.Impulse);
	}

	private void EndRolling() {
		state = 0;
		hitPower = 0;
		stoppedTimer = 0f;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		forwardValue = Vector3.ProjectOnPlane (forwardValue, Vector3.up).normalized;
		transform.rotation = Quaternion.LookRotation (forwardValue, Vector3.up);
		groundAction = false;
		airAction = false;
	}

	private void BeginGroundAction() {
		groundAction = true;
		groundActionReady = false;
		groundActionTimer = groundActionLength;
        hopPower = 0f;
        c.material = frictiony;
        jumpTimer = jumpValidTime;
        //rotationSpeed = fastRotation;
    }

	private void DoGroundAction() {
        xRotation += Time.deltaTime * rb.velocity.magnitude * 10f;
		groundActionTimer -= Time.deltaTime;
        jumpTimer -= Time.deltaTime;

        if (rb.velocity.magnitude > 1)
            rolling = true;
        else
            rolling = false;

        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, 0.5f, 1f), Time.deltaTime * 4f);

        hopPower += Time.deltaTime;

        if (grounded)
        {
            
        }
        else
        {
            
            rb.AddForce(Vector3.down * Time.deltaTime * 4f, ForceMode.Impulse);
        }
            

        if (!Input.GetKey (KeyCode.Space))
			EndGroundAction ();
	}

	private void EndGroundAction() {
		groundActionTimer = 0f;
		groundAction = false;
        groundActionReady = true;

        xRotation = 0f;
        SetRotationInstantly();
        
        c.material = slippery;

        transform.localScale = new Vector3(1f, 2f, 1f);
        //rotationSpeed = slowRotation;

        rolling = false;

        if (grounded && jumpTimer <= 0f)
        {
            rb.AddForce((transform.up + transform.forward).normalized * (actionJumpPower * Mathf.Min(hopPower, 1f)), ForceMode.Impulse);
            jumpParticles.Play();
        }
        
	}

	void OnCollisionEnter(Collision c) {
		if (c.collider.gameObject.CompareTag ("Boundary")) {
			transform.position = startPos;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}

        if (c.collider.gameObject.CompareTag("Environment"))
        {
            //foreach(ContactPoint cp in c.contacts)
            //contacts.Add(cp);
            if (!grounded)
            {
                //Vector3.
                //c.contacts[0].normal
                //Quaternion.
                //transform.rotation = transform.rotation * Quaternion.FromToRotation(transform.up, c.contacts[0].normal);
            }
        }


    }

    void OnCollisionExit(Collision c)
    {
        //if (c.collider.gameObject.CompareTag("Environment"))
        //{
            //foreach (ContactPoint cp in c.contacts)
                //contacts.Remove(cp);
        //}
    }

    void TakeDamage(EnemyHitInfos hit) {
        if(rolling) {
            hit.attacker.SendMessage("Hit");
        } else {
            rb.freezeRotation = false;
            transform.localScale = Vector3.one;
            rb.AddForceAtPosition(hit.hitDirection * hit.force, hit.hitPoint, ForceMode.VelocityChange);
            ScoreKeeper.keeper.UpdateCarrotCount(-hit.carrotLoss);
            state = 2;
            hurtTimer = hurtTime;
            //Todo: Spawn dropped carrot pickups in random directions
        }


    }
}
