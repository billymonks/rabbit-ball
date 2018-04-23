using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitCameraController : MonoBehaviour {
    public Transform target;
    public PlayerController player;
    public Transform cameraPosNear, cameraPosFar;
    private Transform cameraRig;

    private Vector3 prevPos, targetMovementForward;

    private float lookAheadDistance = 1f;

    public float nearSpeed, farSpeed;
    private float posLerpAmt = 0f;

    private float cameraRigXRotation = 0f;
	// Use this for initialization
	void Start () {
        cameraRig = cameraPosNear.parent.transform;
        prevPos = target.transform.position;
        targetMovementForward = target.forward;

        cameraRigXRotation = cameraRig.eulerAngles.x;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 posDiff = (target.position - prevPos);

        float changedCameraRigXRotation; // = Mathf.Lerp(cameraRigXRotation, Mathf.Clamp(cameraRigXRotation - posDiff.y * 1f, -20f, 30f), Time.deltaTime * 16f);
        float cameraRigXRotationChange; // = changedCameraRigXRotation - cameraRigXRotation;
        //cameraRigXRotation = changedCameraRigXRotation;
        //print(cameraRigXRotation);

        posLerpAmt = Mathf.Lerp(posLerpAmt, Mathf.Clamp(posDiff.magnitude, nearSpeed, farSpeed), Time.deltaTime);

        cameraRig.transform.position = target.position;

        Vector3 cameraPos = Vector3.Lerp(cameraPosNear.position, cameraPosFar.position, (posLerpAmt / (farSpeed-nearSpeed)));
        transform.position = Vector3.Lerp(transform.position, cameraPos, Time.deltaTime * 4f);

        if(posDiff.magnitude > 0.1f) {
            targetMovementForward = Vector3.Lerp(targetMovementForward, posDiff, Time.deltaTime);
            lookAheadDistance = Mathf.Clamp(Mathf.Lerp(lookAheadDistance, posDiff.magnitude * 8f, Time.deltaTime), 4f, 64f);

        } else {
            targetMovementForward = Vector3.Lerp(targetMovementForward, target.forward, Time.deltaTime);
            lookAheadDistance = lookAheadDistance = Mathf.Lerp(lookAheadDistance, 4f, Time.deltaTime);
        }

        //print(Mathf.Abs(posDiff.y));
        if (Mathf.Abs(posDiff.y) > 1.2f) {
            changedCameraRigXRotation = Mathf.Lerp(cameraRigXRotation, Mathf.Clamp(cameraRigXRotation - posDiff.y * 1f, -40f, 70f), Time.deltaTime * 4f);
            cameraRigXRotationChange = changedCameraRigXRotation - cameraRigXRotation;
        } else {
            changedCameraRigXRotation = Mathf.Lerp(cameraRigXRotation, 0f, Time.deltaTime);
            cameraRigXRotationChange = changedCameraRigXRotation - cameraRigXRotation;
        }

        cameraRigXRotation = changedCameraRigXRotation;

        //print(cameraRigXRotation);
        //cameraRig.eulerAngles.Set(cameraRigXRotation, cameraRig.eulerAngles.y, cameraRig.eulerAngles.z);
        cameraRig.Rotate(Vector3.right, cameraRigXRotationChange);
        //print(cameraRig.eulerAngles);
        //print(player.forwardValue.normalized);
        //transform.LookAt(cameraRig.transform.position + player.forwardValue.normalized * lookAheadDistance);
        transform.LookAt(cameraRig.transform.position + cameraRig.forward * lookAheadDistance);

        prevPos = target.position;
	}
}
