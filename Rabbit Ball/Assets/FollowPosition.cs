using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour {
    public Rigidbody followBody;
    public Transform follow, cameraTransform;
    private Vector3 initialCamPos;
    private float yPosOffset = 0;
	// Use this for initialization
	void Start () {
        initialCamPos = cameraTransform.localPosition;
    }
	
	// Update is called once per frame
	void Update () {
        //yPosOffset = Mathf.Lerp(yPosOffset, Mathf.Clamp(followBody.velocity.y / 16f, -1f, 1f), Time.deltaTime * 8f);
        this.transform.position = follow.position;
        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, cameraTransform.localPosition.y - yPosOffset, Mathf.Lerp(cameraTransform.localPosition.z, initialCamPos.z - followBody.velocity.magnitude, Time.deltaTime));
        
    }
}
