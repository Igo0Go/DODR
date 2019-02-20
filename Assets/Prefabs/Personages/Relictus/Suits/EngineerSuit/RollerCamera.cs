using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerCamera : MonoBehaviour {

    public Transform target;
    public Vector3 camOffset;
    public LayerMask noTargetMask;
    public LayerMask obstacleMask;
    public float camSpeed;
    public float xSpeed;
    public float ySpeed;
    public float angelLimit;
    public float minDistance;
    public float noTargetDistance;

    private Camera cam;
    private LayerMask camOrigin;
    private Vector3 localPos;
    private float currentRotation;
    public float maxDistance;

    private float Distance
    {
        get
        {
            return Vector3.Distance(Position, target.position);
        }
    }



    private Vector3 Position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }

    // Use this for initialization
    void Start () {
        maxDistance = Distance;
        camOffset = target.transform.position - Position;
        cam = GetComponent<Camera>();
        camOrigin = cam.cullingMask;
	}

    // Update is called once per frame
   
    void Update () {
        CameraRotation();
        ObstacleReaction();
        TargetReaction();
    }

    private void CameraRotation()
    {
        float mx, my;
        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");
        
        if(my != 0 || mx != 0)
        {
            if (my != 0)
            {
                float targetMousePos = Mathf.Clamp(currentRotation - my * ySpeed * Time.deltaTime, -angelLimit, angelLimit);
                if (targetMousePos != currentRotation)
                {
                    float rotation = targetMousePos - currentRotation;
                    transform.RotateAround(target.position, transform.right, rotation);
                    currentRotation = targetMousePos;
                }
            }
            if (mx != 0)
            {
                transform.RotateAround(target.position, Vector3.up, mx * xSpeed * Time.deltaTime);
            }
            camOffset = target.transform.position - Position;
        }
        transform.LookAt(target);
    }
    private void ObstacleReaction()
    {
        RaycastHit hit;
        if (Physics.Raycast(target.position, Position - target.position, out hit, Distance, obstacleMask))
        {
            Position = hit.point;
        }
        else if(Distance <= maxDistance && Physics.Raycast(Position, -transform.forward, 0.1f, obstacleMask))
        {
            Position -= transform.forward * camSpeed * Time.deltaTime;
        }
        else if (Distance > maxDistance)
        {
            Position += transform.forward * camSpeed * Time.deltaTime;
            camOffset = target.transform.position - Position;
        }
    }
    private void TargetReaction()
    {
        if(Distance < noTargetDistance)
        {
            cam.cullingMask = noTargetMask;
        }
        else
        {
            cam.cullingMask = camOrigin;
        }
    }

}
