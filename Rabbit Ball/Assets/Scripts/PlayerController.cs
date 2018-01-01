using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
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
	// Use this for initialization
	void Start () {
        transform = body.transform;
		prevPos = transform.position;
		forwardValue = transform.forward;

		startPos = transform.position;

		rb = body.GetComponent<Rigidbody> ();
        c = body.GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
        Ray r = new Ray(transform.position, -transform.up * 2f);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 2f))
        {
            grounded = true;
        } else
        {
            grounded = false;
        }

		if (state == 0) {
            BeginRolling();
		} else if (state == 1) {
			UpdateForwardFromMovement ();

			if (Input.GetKey (KeyCode.W)) {
				if (groundActionReady) {
					BeginGroundAction ();
				}
			}

				if (groundAction)
					DoGroundAction ();
                else
                    transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime * 10f);
        }
	}

	private void UpdateForwardFromMovement() {
        float rotation = 0f;
        if (Input.GetKey(KeyCode.A))
            rotation -= rotationSpeed;
        if (Input.GetKey(KeyCode.D))
            rotation += rotationSpeed;
        rb.velocity = Quaternion.AngleAxis(rotation, transform.up) * rb.velocity;

        if ((transform.position - prevPos).magnitude > 1) {
			forwardValue = Vector3.Lerp(forwardValue, 
				(transform.position - prevPos).normalized, 
				cameraTurnSpeed * Time.deltaTime);

            forwardValue = Quaternion.AngleAxis(rotation * 3f, transform.up) * forwardValue;

            prevPos = transform.position;
		} else
        {
            forwardValue = Quaternion.AngleAxis(rotation * 3f, transform.up) * forwardValue;
        }

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation (forwardValue), Time.deltaTime * 5f);
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
        rotationSpeed = fastRotation;
    }

	private void DoGroundAction() {
		groundActionTimer -= Time.deltaTime;

        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, 0.5f, 1f), Time.deltaTime * 4f);
        hopPower += Time.deltaTime;

        if (grounded)
            rotationSpeed = fastRotation;
        else {
            rotationSpeed = slowRotation;
            rb.AddForce(Vector3.down * Time.deltaTime * 4f, ForceMode.Impulse);
        }
            

        if (!Input.GetKey (KeyCode.W))
			EndGroundAction ();
	}

	private void EndGroundAction() {
		groundActionTimer = 0f;
		groundAction = false;
        groundActionReady = true;
        
        c.material = slippery;
        rotationSpeed = slowRotation;

        if (grounded)
        {
            rb.AddForce((transform.up + transform.forward).normalized * (actionJumpPower * Mathf.Min(hopPower, 1f)), ForceMode.Impulse);
        }
        
	}

	void OnCollisionEnter(Collision c) {
		if (c.collider.gameObject.CompareTag ("Boundary")) {
			transform.position = startPos;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
	}
}
