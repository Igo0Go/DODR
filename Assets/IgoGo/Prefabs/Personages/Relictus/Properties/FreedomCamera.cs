using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreedomCamera : MonoBehaviour {

    [Range(0,2)] public float speed;
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
        ChangeSpeed();
	}

    private float SprintInput()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            return sprintSpeedMultiplicator;
        }
        else
        {
            return speed;
        }
    }
    private void CamPosition()
    {
        forwardAxis = Input.GetAxis("Vertical");
        rightAxis = Input.GetAxis("Horizontal");
        upAxis = Input.GetAxis("UpDownAxis");

        Vector3 newPos = (transform.forward * forwardAxis  * SprintInput()) + (transform.right * rightAxis * speed) + (transform.up * upAxis * speed);

        transform.position += newPos;      
    }
    private void CamRotation()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        rotateZ = Input.GetAxis("LeftRightRotation");

        Quaternion rot = Quaternion.Euler(-mouseY*ySpeed, mouseX * xSpeed, rotateZ * -zSpeed);
        transform.rotation = transform.rotation * rot;


        //transform.rotation = Quaternion.Slerp(transform.rotation, roty, ySpeed);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotz, zSpeed);

        //transform.rotation = Quaternion.Slerp(transform.rotation, rot, xSpeed);

        //transform.Rotate(transform.up, mouseX * xSpeed);
        //transform.Rotate(transform.right, -mouseY * ySpeed);
        //transform.Rotate(transform.forward, rotateZ * zSpeed);

    }
    private void ChangeSpeed()
    {
        if(Input.GetKey(KeyCode.X) && speed + Time.deltaTime <= 2)
        {
            speed += Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.Z) && speed - Time.deltaTime >= 0)
        {
            speed -= Time.deltaTime;
        }
    }
}
