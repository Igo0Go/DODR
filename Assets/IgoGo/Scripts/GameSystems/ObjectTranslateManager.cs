using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void SimpleHandler();

public class ObjectTranslateManager : UsingObject {

    [Tooltip("Смещения. Суммируются от начальной позиции. При старте объект сместиться от начального состояния на вектор из элемента 0," +
        "потом от полученной позции ещё на вектор из элемента 1 и т.д.")] public List<Vector3> offsetPos;
    [Tooltip("скорость движения")] public float speed;
    [Tooltip("Задержка перед запуском")] public float delay;
    [Tooltip("Задержка между циклами")] public float pauseTime;
    public bool reverce;

    private SimpleHandler moveHandler;
    private Vector3 currentTargetPos;
    private Vector3 moveVector;
    [Space(20)] public bool active;
    private bool pause;
    private int currentOffsetItem;
    private int forwardWay;


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
        pause = false;
        if (reverce)
        {
            moveHandler = ReverceMove;
            currentOffsetItem = 0;
            forwardWay = 1;
            currentTargetPos = transform.position + (forwardWay * offsetPos[currentOffsetItem]);
            moveVector = currentTargetPos - transform.position;
            moveVector = moveVector.normalized;
        }
        else
        {
            moveHandler = ForwardMove;
        }
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
                if(currentOffsetItem == offsetPos.Count-1)
                {
                    active = false;
                }
                else
                {
                    pause = true;
                    Invoke("ChangeTarget", pauseTime);
                }
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
        if(forwardWay == 1)
        {
            currentOffsetItem++;
            if (currentOffsetItem == offsetPos.Count)
            {
                currentOffsetItem--;
                forwardWay = -1;
            }
        }
        else
        {
            currentOffsetItem--;
            if (currentOffsetItem < 0)
            {
                currentOffsetItem++;
                forwardWay = 1;
            }
        }

        currentTargetPos = transform.position + (forwardWay *  offsetPos[currentOffsetItem]);
        
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
            ChangeTarget();
            active = true;
        }
        pause = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 bufer = transform.position;
        for (int i = 0; i < offsetPos.Count; i++)
        {
            Gizmos.DrawLine(bufer, bufer + offsetPos[i]);
            bufer += offsetPos[i];
            if(i == offsetPos.Count-1)
            {
                Gizmos.DrawCube(bufer, transform.lossyScale);
            }
            else
            {
                Gizmos.DrawSphere(bufer, 0.4f);
            }
        }
    }
}
