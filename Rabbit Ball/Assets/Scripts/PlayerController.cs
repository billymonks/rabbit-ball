using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public RectTransform powerMeter;
	private float maxMeterHeight;

	private int state = 0;

	public Vector3 forwardValue;

	private Vector3 prevPos;

	private Rigidbody rb;

	public float cameraTurnSpeed = 5f;

	private float hitPower = 0f, maxHitPower = 100f, chargeSpeed = 1f;

	private float stoppedTimer = 0f, stoppedLength = 1f;

	public float powerMultiplier = 0.2f;

	public float groundActionLength = 3f;
	public float airActionLength = 3f;

	private float groundActionTimer = 0f;
	private float airActionTimer = 0f;
	private bool groundAction = false, airAction = false, groundActionReady = false, airActionReady = false;

	public float actionRunSpeed = 10f;
	public float actionJumpPower = 30f;

	private Vector3 startPos = Vector3.zero;

	// Use this for initialization
	void Start () {
		maxMeterHeight = powerMeter.rect.height;
		prevPos = transform.position;
		forwardValue = transform.forward;

		startPos = transform.position;

		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		powerMeter.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, maxMeterHeight * (hitPower / maxHitPower)); 
		if (state == 0) {
			if (Input.GetKey (KeyCode.W)) {
				hitPower += chargeSpeed;
				hitPower = Mathf.Min (hitPower, maxHitPower);
			} else if (hitPower > 0) {
				hitPower = Mathf.Min (hitPower, maxHitPower);
				BeginRolling ();
			} else {
				float rotation = 0f;
				if (Input.GetKey (KeyCode.A))
					rotation -= 1f;
				if (Input.GetKey (KeyCode.D))
					rotation += 1f;
				transform.Rotate (new Vector3 (0f, rotation, 0f));
				forwardValue = transform.forward;
			}
		} else if (state == 1) {
			UpdateForwardFromMovement ();

			if (Input.GetKey (KeyCode.W)) {
				if (groundActionReady) {
					BeginGroundAction ();
				}
			}

			if (rb.velocity.magnitude < 1 && rb.angularVelocity.magnitude < 1) {
				stoppedTimer -= Time.deltaTime;
				if(stoppedTimer <= 0f)
					EndRolling ();
			} else {
				stoppedTimer = stoppedLength;

				if (groundAction)
					DoGroundAction ();
			}
		}
	}

	private void UpdateForwardFromMovement() {
		if ((transform.position - prevPos).magnitude > 1) {
			forwardValue = Vector3.Lerp(forwardValue, 
				(transform.position - prevPos).normalized, 
				cameraTurnSpeed * Time.deltaTime);



			prevPos = transform.position;
		}

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation (forwardValue), Time.deltaTime * 2f);
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
	}

	private void DoGroundAction() {
		groundActionTimer -= Time.deltaTime;

		rb.AddForce(forwardValue * actionRunSpeed * Time.deltaTime, ForceMode.VelocityChange);

		if (groundActionTimer <= 0f || !Input.GetKey (KeyCode.W))
			EndGroundAction ();
	}

	private void EndGroundAction() {
		groundActionTimer = 0f;
		groundAction = false;
		rb.AddForce (Vector3.up * actionJumpPower, ForceMode.Impulse);
	}

	void OnCollisionEnter(Collision c) {
		if (c.collider.gameObject.CompareTag ("Boundary")) {
			transform.position = startPos;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
	}
}
