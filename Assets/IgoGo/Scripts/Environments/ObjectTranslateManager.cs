using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void SimpleHandler();

public class ObjectTranslateManager : UsingObject {

    public Vector3 offsetPos;
    public float speed;
    [Tooltip("Задержка перед запуском")] public float delay;
    [Tooltip("Задержка между циклами")] public float pauseTime;
    public bool reverce;

    private SimpleHandler moveHandler;
    private Vector3 pos1;
    private Vector3 pos2;
    private Vector3 currentTargetPos;
    private Vector3 moveVector;
    [Space(20)] public bool active;
    private bool pause;


    private bool Conclude
    {
        get
        {
            if (Vector3.Distance(transform.position, currentTargetPos) > speed * Time.deltaTime)
            {
                return false;
            }
            return true;
        }
    }

    // Use this for initialization
    void Start()
    {
        pos1 = transform.position;
        pos2 = pos1 + offsetPos;
        pause = false;
        if (reverce)
        {
            moveHandler = ReverceMove;
        }
        else
        {
            moveHandler = ForwardMove;
        }
        currentTargetPos = pos1;
    }

    // Update is called once per frame
    void Update()
    {
        moveHandler();
    }

    public override void Use()
    {
        Invoke("Action", delay);
    }

    private void ForwardMove()
    {
        if (active && !pause)
        {
            if (Conclude)
            {
                transform.position = currentTargetPos;
                active = false;
            }
            else
            {
                transform.position += moveVector * speed * Time.deltaTime;
            }
        }
    }
    private void ReverceMove()
    {
        if (active && !pause)
        {
            if (Conclude)
            {
                transform.position = currentTargetPos;
                pause = true;
                Invoke("ChangeTarget", pauseTime);
            }
            else
            {
                transform.position += moveVector * speed * Time.deltaTime;
            }
        }
    }

    private void ChangeTarget()
    {
        if (currentTargetPos == pos1)
        {
            currentTargetPos = pos2;
        }
        else
        {
            currentTargetPos = pos1;
        }
        moveVector = currentTargetPos - transform.position;
        moveVector = moveVector.normalized;
        pause = false;
    }
    
    private void Action()
    {
        if (reverce)
        {
            active = !active;
        }
        else
        {
            active = true;
        }
        pause = false;
        ChangeTarget();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position + offsetPos, transform.localScale);
    }
}
