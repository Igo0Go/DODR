﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour {

    public Transform xAxis;
    public Transform yAxis;
    public AutoWeapon weapon;
    public Transform target;

    [Space(10)]
    public float xSpeed;
    public float ySpeed;
    public float maxY;
    public float minY;
    [Range(-1, 1)] public int errorMultiplicator;
    [Space(10)]
    public bool patrol;

    private Quaternion start;
    private Quaternion end;
    private bool xReady;
    private bool yReady;

    

	// Use this for initialization
	void Start () {
        start = Quaternion.identity;
        end = Quaternion.Euler(0f, 90f, 0f);
    }
	
	// Update is called once per frame
	void Update () {

        if(target != null)
        {
            xRotate();
            yRotate();

            if (xReady && yReady)
            {
                weapon.active = true;
            }
            else
            {
                weapon.active = false;
            }
        }
        else
        {
            weapon.active = false;
            Patrol();
        }
       
	}

    public void Clear()
    {
        target = null;
    }

    private void Patrol()
    {
        xAxis.rotation = Quaternion.Lerp(start, end, Mathf.PingPong(xSpeed*Time.deltaTime, 1f));
    }

    private void xRotate()
    {
        if(target != null)
        {
            Vector3 direction =  target.position - xAxis.position;
            direction.y = 0;
            if (Vector3.Angle(xAxis.forward, direction) > 1f)
            {
                xReady = false;
                Quaternion xRot = Quaternion.LookRotation(direction, xAxis.up);
                xAxis.rotation = Quaternion.Slerp(xAxis.rotation, xRot, Time.deltaTime * xSpeed);
            }
            else
            {
                xAxis.forward = direction;
                xReady = true;
            }
           

        }
    }
    private void yRotate()
    {
        if (target != null)
        {
            Vector3 direction = target.position - yAxis.position;

            if(Vector3.Angle(yAxis.forward, direction) < 40)
            {
                yReady = true;
            }
            else
            {
                yReady = false;
            }
        }
    }
}
