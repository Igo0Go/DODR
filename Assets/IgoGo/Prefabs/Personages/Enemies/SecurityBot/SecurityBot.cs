using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityBot : MonoBehaviour {

    public int damage;
    public CharacterReactions player;
    public Vector3 target;
    public float speed;
    public int move;

    private int myMove;
    private float distance;
    private Vector3 startPos;
    private Vector3 moveVector;
    private Animator anim;

	// Use this for initialization
	void Start () {
        myMove = 0;
        startPos = transform.position;
        move = 0;
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        if (myMove == 1)
        {
            anim.SetBool("Active", true);
            MoveToTarget();
        }
        else if(myMove == -1)
        {
            anim.SetBool("Active", false);
            MoveOutTarget();
        }
        else
        {
            anim.SetBool("Active", false);
            MoveToStartPosition();
        }
	}

    private void MoveToTarget()
    {
        float distance = Vector3.Distance(target,transform.position);
        float step = speed * Time.deltaTime;
        moveVector = target - transform.position;

        if(distance > 0.5f)
        {
            transform.position += moveVector.normalized * step;
        }
        else
        {
            player.GetDamage(damage);
            myMove = -1;
        }
    }

    private void MoveOutTarget()
    {
        float distance = Vector3.Distance(target, transform.position);
        float step = speed * Time.deltaTime;
        moveVector = transform.position - target;

        if (distance < 2)
        {
            transform.position += moveVector.normalized * step;
        }
        else
        {
            myMove = move;
        }
    }

    private void MoveToStartPosition()
    {
        float distance = Vector3.Distance(startPos, transform.position);
        float step = speed * Time.deltaTime;
        moveVector = startPos - transform.position;

        if (step < distance)
        {
            transform.position += moveVector.normalized * step;
        }
        else
        {
            transform.position = startPos;
            myMove = move;
        }
    }
}
