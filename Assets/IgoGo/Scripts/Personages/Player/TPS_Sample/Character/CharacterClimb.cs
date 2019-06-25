using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterClimb : MyTools, IPlayerPart {

    public Vector3 animPosError;
    [Range(0,10)]
    public float speed;
    [Space(20)]
    public bool debug;


    private CharacterStatus characterStatus;
    private CharacterInventory characterInventory;
    private CameraHandler cameraHandler;
    private ClimbPoint currentPoint;
    private Vector3 nextPointPos;
    private Animator anim;

    private float currentSpeed;
    private bool fall;
    private bool key;
    private bool opportunityToClimb;
    private int move;

	
	void Update () {

        if (fall)
        {
            if(characterStatus.isGround)
            {
                fall = false;
                opportunityToClimb = true;
                anim.SetTrigger("ChangeWeapon");
            }
        }
        if (characterStatus.onWall)
        {
            if (debug)
            {
                transform.localPosition = animPosError;
                transform.rotation = currentPoint.transform.rotation;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                fall = true;
                characterStatus.onWall = false;
                currentPoint = null;
                transform.parent = null;
                move = 0;
                anim.SetBool("OnWall", false);
                cameraHandler.StaticCam = false;
            }

            if (move == 1)
            {
                MoveToSimplePoint();
            }
            else if(move == 2)
            {
                MoveToFinalePoint();
            }
            else if(key)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    if (currentPoint.up != null)
                    {
                        currentPoint = currentPoint.up;
                        transform.parent = currentPoint.transform;
                        if (currentPoint.climbType == ClimbType.Final)
                        {
                            anim.SetBool("FinalClimb", true);
                            anim.SetBool("OnWall", false);
                            anim.SetFloat("JumpLeg", -1);
                            fall = true;
                            currentSpeed = speed / 10;
                            move = 2;
                        }
                        else
                        {
                            anim.SetInteger("ClimbJump", 1);
                            currentSpeed = speed * 0.7f;
                            move = 1;
                        }
                    }
                    key = false;
                    Invoke("Return", 0.7f);
                    return;
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    if (currentPoint.down != null)
                    {
                        currentPoint = currentPoint.down;
                        transform.parent = currentPoint.transform;
                        anim.SetInteger("ClimbJump", -1);
                        currentSpeed = speed * 0.7f;
                        move = 1;
                    }
                    key = false;
                    Invoke("Return", 0.7f);
                    return;
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    if (currentPoint.right != null)
                    {
                        anim.SetFloat("Xstate", 1);
                        currentPoint = currentPoint.right;
                        transform.parent = currentPoint.transform;
                        currentSpeed = speed;
                        move = 1;
                    }
                    key = false;
                    Invoke("Return", 0.7f);
                    return;
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    if (currentPoint.left != null)
                    {
                        anim.SetFloat("Xstate", -1);
                        currentPoint = currentPoint.left;
                        transform.parent = currentPoint.transform;
                        currentSpeed = speed;
                        move = 1;
                    }
                    key = false;
                    Invoke("Return", 0.7f);
                    return;
                }
            }
        }
    }

    private void Return()
    {
        key = true;
    }
    private void ReturnOp()
    {
        opportunityToClimb = true;
    }
    private void MoveToSimplePoint()
    {
        float step = currentSpeed * Time.deltaTime;
        if(Vector3.Distance(transform.localPosition, animPosError) > step)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, animPosError, step);
        }
        else
        {
            transform.localPosition = animPosError;
            move = 0;
            anim.SetInteger("ClimbJump", 0);
            anim.SetFloat("Xstate", 0);
            float y = currentPoint.transform.rotation.eulerAngles.y;
            cameraHandler.lookAngel = y;
            cameraHandler.StaticCam = true;
        }
    }
    private void MoveToFinalePoint()
    {
        float step = speed * Time.deltaTime;
        if (Vector3.Distance(transform.localPosition, Vector3.zero) > step)
        {
            if(transform.localPosition.y < animPosError.y - Mathf.Abs(animPosError.y/2))
            {
                transform.Translate(Vector3.up * step);
            }
            else
            {
                fall = false;
                transform.localPosition = Vector3.Slerp(transform.localPosition, Vector3.zero, step * 2);
            }
        }
        else
        {
            transform.localPosition = Vector3.zero;
            fall = false;
            transform.parent = null;
            characterStatus.onWall = false;
            move = 0;
            anim.SetTrigger("ChangeWeapon");
            Invoke("ReturnOp", 1f);
            cameraHandler.StaticCam = false;
        }
    }
    public void ResetAll()
    {
        characterStatus.onWall = false;
        characterStatus.isGround = true;
        fall = false;
        currentPoint = null;
        transform.parent = null;
        move = 0;
        anim.SetBool("OnWall", false);
        anim.SetBool("OnGround", true);
    }

    public void Initialize(SampleController sampleController)
    {
        fall = false;
        opportunityToClimb = true;
        characterInventory = sampleController.characterInventory;
        characterStatus = sampleController.characterStatus;
        characterStatus.onWall = false;
        cameraHandler = sampleController.cameraHandler;
        anim = sampleController.anim;
    }

    private void OnTriggerStay(Collider other)
    {
        if(!characterStatus.onWall)
        {
            if (other.tag.Equals("Helper"))
            {
                if (!characterStatus.isGround && !fall && opportunityToClimb)
                {
                    ClimbStarter starter;
                    if (MyGetComponent(other.gameObject, out starter))
                    {
                        Vector3 dir = starter.point.transform.position - transform.position;
                        dir.y = 0;
                        if (Vector3.Angle(transform.forward, dir) < 100)
                        {
                            key = true;
                            opportunityToClimb = false;
                            characterInventory.DestroyWeapon();
                            characterInventory.SelectWeaponAction(3);
                            currentPoint = starter.point;
                            transform.parent = currentPoint.transform;
                            characterStatus.onWall = true;
                            currentSpeed = speed;
                            move = 1;
                            anim.SetFloat("JumpLeg", 0);
                            anim.SetBool("OnGround", false);
                            anim.SetBool("OnWall", true);
                            transform.rotation = currentPoint.transform.rotation;
                        }
                    }
                }
            }
        }
    }
    
}
