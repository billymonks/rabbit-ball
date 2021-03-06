﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraController : MonoBehaviour {
	private bool lookAtTarget = true, followTarget = true;

	public PlayerController target;
    private Rigidbody rb;
	public float targetDistance = 10f;
	public float followSpeed = 10f;
    public float speedScaler = 10f;

    public float mouseSpeed = 1f;

    private float yHeight = 4f;
    private float minYHeight = 0f;
    private float maxYHeight = 20f;
	// Use this for initialization
	void Start () {
        rb = target.GetComponentInChildren<Rigidbody>();
	}

	// Update is called once per frame
	void Update () {
		if (target) {
            float verticalMouseChange = Input.GetAxis("Mouse Y") * mouseSpeed * 0.5f;
            yHeight = Mathf.Clamp(yHeight - verticalMouseChange, minYHeight, maxYHeight);


            Vector3 lookAtVector = (target.transform.position - transform.position).normalized;
            //transform.LookAt (target.transform);
            Vector3 destinationPos = target.transform.position - ((target.forwardValue)) * targetDistance + Vector3.up * yHeight;
            //Debug.DrawLine(destinationPos, destinationPos + Vector3.up, Color.blue);
            float distanceFromTarget = Vector3.Distance(transform.position, destinationPos);
            float cameraMovementSpeed = (distanceFromTarget / speedScaler) * followSpeed;

            transform.position = Vector3.Lerp(transform.position, destinationPos, Time.deltaTime * cameraMovementSpeed);
            //Plane xz = new Plane(transform.right, transform.position);
            //Vector3 targetPoint = target.transform.position + target.forwardValue * (rb.velocity.magnitude / 4f) + target.transform.up * 2f;
            //float distance = xz.GetDistanceToPoint(targetPoint);
            //transform.position += transform.right * distance;

            //Debug.DrawLine(transform.position, targetPoint, Color.magenta);

            //transform.LookAt(targetPoint);
            transform.LookAt(target.transform);

            RaycastHit hit;
			Ray r = new Ray (target.transform.position, transform.position - target.transform.position);
			if (Physics.Raycast (r, out hit, (transform.position - target.transform.position).magnitude)) {
				transform.position = hit.point;
			}
		}
	}
}
