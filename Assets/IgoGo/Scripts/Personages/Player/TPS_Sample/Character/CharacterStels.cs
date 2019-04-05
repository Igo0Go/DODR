using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    right,
    left,
    stay,
    forward,
    back
}

public class CharacterStels : MyTools, IPlayerPart {

    private CharacterStatus characterStatus;
    private CharacterController characterController;
    private CharacterInput characterInput;
    private CameraHandler cameraHandler;
    private Animator anim;

    private Collider currentCollider;
    private Shelter currentShelter;
    private Vector3 dir;
    private Vector3 target;
    private Vector3 bufer;
    private Direction direction;
    [SerializeField] private bool opportunityToStels;
    private float speed;
    private int move;
    private bool leftCam;
    private bool aiming;

    private bool InDistance
    {
        get
        {
            if(Vector3.Distance(transform.position, target) <= 0.1f)
            {
                return true;
            }
            return false;
        }
    }

    public void Initialize(SampleController sampleController)
    {
        cameraHandler = sampleController.cameraHandler;
        characterStatus = sampleController.characterStatus;
        anim = sampleController.anim;
        characterController = sampleController.characterController;
        characterInput = sampleController.characterInput;
        speed = 10;
        direction = Direction.stay;
    }



    private void Update()
    {
        StelsUpdate();
    }

    public void StelsUpdate()
    {
        ToCover();
        MoveToTarget();
        StelsInput();
    }

    private void ToCover()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (characterStatus.isGround && !characterStatus.isAiming &&
                !characterStatus.isFight && !characterStatus.onWall)
            {
                if (opportunityToStels && !characterStatus.isBehindCover)
                {
                    if (MyGetComponent(currentCollider.gameObject, out currentShelter))
                    {
                        dir = currentShelter.direction;
                        anim.SetBool("FullShelter", currentShelter.fullShelter);
                        if (Vector3.Angle(transform.forward, dir) < 100)
                        {
                            characterStatus.isBehindCover = true;
                            anim.SetFloat("Zstate", 0);
                            anim.SetFloat("RotateX", 0);
                            transform.forward = dir;
                            leftCam = cameraHandler.leftPivot;
                            if (currentShelter.shelterType == ShelterType.Standart)
                            {
                                target = CheckStandartTarget(transform.position, currentShelter.direction);
                            }
                            else
                            {
                                target = currentShelter.playerPoint.position;
                            }
                            dir = -currentShelter.direction;
                            move = 1;
                        }
                    }
                }
                else
                {
                    characterStatus.isBehindCover = false;
                    cameraHandler.leftPivot = leftCam;
                    cameraHandler.StaticCam = false;
                }
                anim.SetBool("Stels", characterStatus.isBehindCover);
                anim.applyRootMotion = characterStatus.isBehindCover;
                CapsulToStelsState(characterStatus.isBehindCover);
            }
        }
    }
    private void MoveToTarget()
    {
        if(move > 0)
        {
            if (InDistance)
            {
                transform.position = target;
                if (move == 2)
                {
                    target = bufer;
                    move = 1;
                }
                else if(move == 1)
                {
                    transform.forward = dir;
                    anim.applyRootMotion = false;
                    anim.SetFloat("Xstate", 0);
                    move = 0;
                    if (!characterStatus.isAiming)
                    {
                        aiming = false;
                        float y = currentShelter.playerPoint.rotation.eulerAngles.y;
                        cameraHandler.lookAngel = y;
                        cameraHandler.StaticCam = true;
                    }

                    if (currentShelter.shelterType == ShelterType.Angel)
                    {
                        direction = Direction.stay;
                        if (currentShelter.shelterAngel.rightShelter == null)
                        {
                            cameraHandler.leftPivot = true;
                        }
                        else
                        {
                            cameraHandler.leftPivot = false;
                        }
                    }
                }
            }
            else
            {
                transform.position = Vector3.Slerp(transform.position, target, speed * Time.deltaTime);
            }
        }
    }
    private void StelsInput()
    {
        if(characterStatus.isBehindCover && move == 0 && currentShelter != null)
        {
            if(currentShelter.shelterType == ShelterType.Standart)
            {
                StandartInput();
            }
            else
            {
                AngelIput();
            }
        }
    }

    private void StandartInput()
    {
        float hor = Input.GetAxis("Horizontal");
        if(hor != 0) 
        {
            transform.Translate(currentShelter.transform.right * hor * Time.deltaTime);
            if(hor > 0 )
            {
                direction = Direction.right;
            }
            else
            {
                direction = Direction.left;
            }
        }
        else
        {
            direction = Direction.stay;
        }
        anim.SetFloat("Xstate", hor);
    }
    private void AngelIput()
    {
        float hor = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.D))
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (currentShelter.shelterAngel.rightHelper != null && characterInput.selectedWeapon != 3)
                {
                    Vector3 point = currentShelter.shelterAngel.rightHelper.transform.position;
                    target = new Vector3(point.x, transform.position.y, point.z);
                    CapsulToStelsState(false);
                    dir = currentShelter.direction;
                    transform.forward = dir;
                    anim.SetBool("Aiming", true);
                    anim.SetFloat("Xstate", 1);
                    move = 1;
                    aiming = true;
                    return;
                }
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(currentShelter.shelterAngel.rightShelter != null)
                {
                    bufer = currentShelter.shelterAngel.rightShelter.playerPoint.transform.position;
                    target = currentShelter.shelterAngel.rightHelper.position;
                    dir = -currentShelter.shelterAngel.rightShelter.direction;
                    anim.applyRootMotion = true;
                    anim.SetTrigger("StelsAngel");
                    anim.SetFloat("Xstate", 1);
                    move = 2;
                }
                else
                {
                    target = currentShelter.playerPoint.transform.position;
                    move = 1;
                }
                return;
            }

            if (currentShelter.shelterAngel.rightNeigbor != null && !characterStatus.isAiming)
            {
                transform.Translate(-currentShelter.transform.right * Time.deltaTime);
                anim.SetFloat("Xstate", hor);
                direction = Direction.right;
                return;
            }
            else if(direction != Direction.stay)
            {
                target = currentShelter.playerPoint.transform.position;
                move = 1;
                return;
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (currentShelter.shelterAngel.leftHelper != null && characterInput.selectedWeapon != 3)
                {
                    Vector3 point = currentShelter.shelterAngel.leftHelper.transform.position;
                    target = new Vector3(point.x, transform.position.y, point.z);
                    CapsulToStelsState(false);
                    dir = currentShelter.direction;
                    transform.forward = dir;
                    anim.SetBool("Aiming", true);
                    anim.SetFloat("Xstate", -1);
                    move = 1;
                    aiming = true;
                    return;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (currentShelter.shelterAngel.leftShelter != null)
                {
                    bufer = currentShelter.shelterAngel.leftShelter.playerPoint.transform.position;
                    target = currentShelter.shelterAngel.leftHelper.position;
                    dir = -currentShelter.shelterAngel.leftShelter.direction;
                    anim.SetTrigger("StelsAngel");
                    anim.SetFloat("Xstate", -1);
                    anim.applyRootMotion = true;
                    move = 2;
                }
                else
                {
                    target = currentShelter.playerPoint.position;
                    move = 1;
                }
                return;
            }

            if (currentShelter.shelterAngel.leftNeigbor != null && !characterStatus.isAiming)
            {
                transform.Translate(currentShelter.transform.right * Time.deltaTime);
                anim.SetFloat("Xstate", hor);
                direction = Direction.left;
                return;
            }
            else if (direction != Direction.stay)
            {
                target = currentShelter.playerPoint.position;
                move = 1;
                return;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            target = currentShelter.playerPoint.position;
            characterStatus.isAiming = false;
            CapsulToStelsState(false);
            dir = -currentShelter.direction;
            anim.SetBool("Aiming", false);
            anim.SetFloat("Xstate", 0);
            transform.forward = dir;
            move = 1;
        }

        anim.SetFloat("Xstate", 0);

    }
    public void CapsulToStelsState(bool state)
    {
        if(state)
        {
            characterController.center = new Vector3(0, 1, 0.2f);
        }
        else
        {
            characterController.center = new Vector3(0, 0.9f, 0);
        }
    }
    private Vector3 CheckStandartTarget(Vector3 point, Vector3 direction)
    {
        Vector3 returnPos = point;
        RaycastHit hit;
        if (Physics.Raycast(point + Vector3.up *0.5f, direction, out hit, 1))
        {
            returnPos = hit.point + hit.normal * 0.2f;
            returnPos.y = point.y - 0.1f;
        }
        return returnPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Shelter"))
        {
            if(characterStatus.isBehindCover && !characterStatus.isAiming)
            {
                currentCollider = other;
                currentShelter = currentCollider.gameObject.GetComponent<Shelter>();
                anim.SetBool("FullShelter", currentShelter.fullShelter);
                if (currentShelter.shelterType == ShelterType.Angel)
                {
                    anim.SetFloat("Xstate", 0);
                    target = currentShelter.playerPoint.position;
                    if(move != 2)
                    {
                        move = 1;
                    }
                }
            }
            else
            {
                currentCollider = other;
                opportunityToStels = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Shelter") && other == currentCollider)
        {
            if(!aiming)
            {
                if (characterStatus.isBehindCover && move != 2)
                {
                    if (direction == Direction.right && currentShelter.shelterAngel.rightNeigbor != null)
                    {
                        currentShelter = currentShelter.shelterAngel.rightNeigbor;
                        currentCollider = currentShelter.GetComponent<BoxCollider>();
                    }
                    else if (direction == Direction.left && currentShelter.shelterAngel.leftNeigbor != null)
                    {
                        currentShelter = currentShelter.shelterAngel.leftNeigbor;
                        currentCollider = currentShelter.GetComponent<BoxCollider>();
                    }
                    target = currentShelter.playerPoint.position;
                    move = 1;
                }
                else
                {
                    currentCollider = null;
                    currentShelter = null;
                    opportunityToStels = false;
                }
            }
        }
    }

    private void StartAiming()
    {
        characterStatus.isAiming = true;
    }
    private void StopAiming()
    {
        anim.SetBool("Aiming", false);
    }
}
