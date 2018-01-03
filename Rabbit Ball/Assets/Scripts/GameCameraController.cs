using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraController : MonoBehaviour {
	private bool lookAtTarget = true, followTarget = true;

	public GameObject target;
    private Rigidbody rb;
	public float targetDistance = 10f;
	public float followSpeed = 10f;
	// Use this for initialization
	void Start () {
        rb = target.GetComponentInChildren<Rigidbody>();
	}

	// Update is called once per frame
	void Update () {
		if (target) {
            Vector3 lookAtVector = (target.transform.position - transform.position).normalized;
			//transform.LookAt (target.transform);
			transform.position = Vector3.Lerp(transform.position, target.transform.position - ((target.transform.forward + transform.forward) /2f) * targetDistance + Vector3.up * 4f, Time.deltaTime * followSpeed);
            Plane xz = new Plane(transform.right, transform.position);
            Vector3 targetPoint = target.transform.position + target.transform.forward * (rb.velocity.magnitude / 4f) + Vector3.up * 2f;
            float distance = xz.GetDistanceToPoint(targetPoint);
            transform.position += transform.right * distance;
            transform.LookAt(targetPoint);
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetPoint - transform.position, Vector3.up), Time.deltaTime * 8f);
            //transform.position = Vector3.Lerp(transform.position, target.transform.position - transform.forward * targetDistance + Vector3.up * 4f, Time.deltaTime * followSpeed);

            RaycastHit hit;
			Ray r = new Ray (target.transform.position, transform.position - target.transform.position);
			if (Physics.Raycast (r, out hit, (transform.position - target.transform.position).magnitude)) {
				transform.position = hit.point;
			}
		}
	}
}
