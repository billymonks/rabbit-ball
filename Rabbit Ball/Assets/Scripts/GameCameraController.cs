using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraController : MonoBehaviour {
	private bool lookAtTarget = true, followTarget = true;

	public PlayerController target;
	public float targetDistance = 10f;
	public float followSpeed = 10f;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		if (target) {
			transform.LookAt (target.transform);
			transform.position = Vector3.Lerp(transform.position, target.transform.position - target.forwardValue * targetDistance + Vector3.up * 4f, Time.deltaTime * followSpeed);

			RaycastHit hit;
			Ray r = new Ray (target.transform.position, transform.position - target.transform.position);
			if (Physics.Raycast (r, out hit, (transform.position - target.transform.position).magnitude)) {
				transform.position = hit.point;
			}
		}
	}
}
