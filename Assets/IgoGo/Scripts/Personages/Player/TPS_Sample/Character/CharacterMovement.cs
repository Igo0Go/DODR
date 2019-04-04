using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MyTools, IPlayerPart {

    [HideInInspector] public PlayerState State
    {
        get { return _state; }
        set
        {
            if(value != PlayerState.active)
            {
                sprintValue = moveAmounth = vertical = horizontal = 0;
            }
            _state = value;
        }
    }

    private Transform cameraTransform;
    private CharacterStatus characterStatus;

    [HideInInspector] public float sprintValue;
    [HideInInspector] public float moveAmounth;
    [HideInInspector] public float vertical;
    [HideInInspector] public float horizontal;

    private Vector3 rotationDirection;
    private Vector3 moveDirection;
    private Vector3 moveDir;
    private Vector3 targetDir;
    private Quaternion lookDir;
    private Quaternion targerRot;
    private CharacterController characterController;
    private PlayerState _state;

    #region Перемещение
    public bool JetPack
    {
        get { return _jetPack; }
        set
        {
            _jetPack = value;
            if(_jetPack)
            {
                jumpForce = jumpForce * 2;
                grav = 0.7f * characterStatus.standartGravityForce;
            }
            else
            {
                jumpForce = characterStatus.standartJumpForce;
                grav = characterStatus.standartGravityForce;
            }
        }
    }

    private bool _jetPack;
    private Vector3 moveVector;
    [SerializeField] private float speed;
    private float grav;
    private float jumpForce;
    private float vertSpeed;

    #endregion



    public void Initiolize(SampleController sampleController)
    {
        characterController = GetComponent<CharacterController>();
        characterStatus = sampleController.characterStatus;
        cameraTransform = sampleController.cameraHandler.transform;
        JetPack = false;
        characterStatus.onWall = false;
        State = sampleController.characterReactions.State;
    }
    

    public void MoveUpdate()
    {
        if(!characterStatus.onWall && !characterStatus.isBehindCover)
        {
            if (State == PlayerState.active)
            {
                vertical = Input.GetAxis("Vertical");
                horizontal = Input.GetAxis("Horizontal");
                moveAmounth = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
                if (moveAmounth != 0)
                {
                    characterStatus.isMove = true;
                }
                else
                {
                    characterStatus.isMove = false;
                }
                Sprint();

                moveDir = cameraTransform.forward * vertical;
                moveDir += cameraTransform.right * horizontal;
                moveDir.Normalize();
                moveDirection = moveDir;
                rotationDirection = cameraTransform.forward;

                RotationNormal();
                PlayerMove();
                characterStatus.isGround = OnGround();
            }
          
        }
    }

    private void PlayerMove()
    {
        if(!characterStatus.isFight)
        {
            if (!characterStatus.isAiming)
            {
                moveVector = transform.forward.normalized * (moveAmounth + sprintValue * 5 * moveAmounth) * Time.deltaTime * 10 * speed;
            }
            else
            {
                moveVector = transform.forward * vertical + transform.right * horizontal;
            }
           
            characterStatus.isGround = characterController.isGrounded;
            if (characterController.isGrounded)
            {
                characterStatus.isJump = false;
                vertSpeed = 0;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    characterStatus.isJump = true;
                    vertSpeed = jumpForce;
                }
            }
            vertSpeed -= grav * Time.deltaTime;
            moveVector = new Vector3(moveVector.x * speed * Time.fixedDeltaTime, vertSpeed * Time.deltaTime, moveVector.z * speed * Time.fixedDeltaTime);
            if (moveVector != Vector3.zero)
            {
                characterController.Move(moveVector);
            }
        }
    }
    public void ResetValues()
    {
        vertSpeed = 0;
    }
    private void RotationNormal()
    {
        if(characterStatus.isGround)
        {
            if (!characterStatus.isAiming)
            {
                rotationDirection = moveDirection;
            }
            targetDir = rotationDirection;
            targetDir.y = 0;
        }
       
        
        if(targetDir == Vector3.zero)
        {
            targetDir = transform.forward;
        }

        lookDir = Quaternion.LookRotation(targetDir);
        targerRot = Quaternion.Slerp(transform.rotation, lookDir, characterStatus.rotationSpeed);
        transform.rotation = targerRot;
    }
    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0)
        {
            characterStatus.isSprint = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetAxis("Vertical") <= 0)
        {
            characterStatus.isSprint = false;
        }

        if (characterStatus.isSprint)
        {
            sprintValue = SmoothlyChange(0,1,sprintValue,Time.deltaTime);
        }
        else
        {
            sprintValue = SmoothlyChange(0, 1, sprintValue, -Time.deltaTime);
        }
    }
    private bool OnGround()
    {
        if(characterController.isGrounded)
        {
            characterStatus.isJump = false;
            return true;
        }
        return false;
    }
}
