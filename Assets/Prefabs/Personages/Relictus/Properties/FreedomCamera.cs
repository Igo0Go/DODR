using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreedomCamera : MonoBehaviour {

    [Range(0,10)] public float speed;
    [Range(1, 10)] public float xSpeed;
    [Range(1, 10)] public float ySpeed;
    [Range(1, 10)] public float zSpeed;
    [Range(1, 5)] public float sprintSpeedMultiplicator;

    private float forwardAxis;
    private float rightAxis;
    private float upAxis;
    private float speedMultiplicator;
    private float mouseX;
    private float mouseY;
    private float rotateZ;

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
        CamPosition();
        CamRotation();
	}

    private float SprintInput()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            return sprintSpeedMultiplicator;
        }
        return 1;
    }

    private void CamPosition()
    {
        forwardAxis = Input.GetAxis("Vertical");
        rightAxis = Input.GetAxis("Horizontal");
        upAxis = Input.GetAxis("UpDownAxis");

        Vector3 newPos = (transform.forward * forwardAxis) + (transform.right * rightAxis) + (transform.up * upAxis);

        transform.position += Vector3.Slerp(transform.position, newPos, SprintInput());      
    }
    private void CamRotation()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        rotateZ = Input.GetAxis("LeftRightRotation");

        Vector3 newRotation = (transform.forward * zSpeed) + (transform.right * ySpeed) + (transform.up * upAxis);

        Quaternion rot = new Quaternion();
        rot.eulerAngles = newRotation;

        Quaternion rotx = transform.rotation * Quaternion.AngleAxis(mouseX, transform.up);
        Quaternion roty = transform.rotation * Quaternion.AngleAxis(-mouseY, transform.right);
        Quaternion rotz = transform.rotation * Quaternion.AngleAxis(rotateZ, transform.forward);

        rot = rotx * roty;

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, xSpeed);
        //transform.rotation = Quaternion.Slerp(transform.rotation, roty, ySpeed);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotz, zSpeed);

        //transform.rotation = Quaternion.Slerp(transform.rotation, rot, xSpeed);

        //transform.Rotate(transform.up, mouseX * xSpeed);
        //transform.Rotate(transform.right, -mouseY * ySpeed);
        //transform.Rotate(transform.forward, rotateZ * zSpeed);

    }
}
