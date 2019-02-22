using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerController : MyTools {

    public bool Protected;
    public Animator anim;
    public PlayerKit specKit;
    public Slider healthSlider;
    public GameObject cam;
    public Transform camTarget;
    public float speed;
    public float xSpeed;
    public float ySpeed;
    public float minY, maxY;
    public float grav;
    public float jumpForce;
    public PlayerState state;

    private Vector3 moveVector;
    bool sprint;
    private float rotationX, rotationY;
    private float currentX, currentY;
    private float vertSpeed;
    private float sprintValue;
    private CharacterController controller;

    private int _health;

    public int Health
    {
        get { return _health; }
        set
        {
            if(value < 0)
            {
                _health = 0;
            }
            else
            {
                _health = value;
            }
            healthSlider.value = _health;
        }
    }

    void Start() {
        state = PlayerState.active;
        Health = 100;
        currentY = currentX = 0;
        sprint = false;
        sprintValue = 0;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
    }

    void Update() {

        if(state == PlayerState.active)
        {
            PlayerMove();
            Sprint();
            PlayerRotate();
        }
        else if(state == PlayerState.inAir)
        {
            OnAirMove();
            PlayerRotate();
        }
        UseKit();
    }


    #region Базовые
    private void PlayerMove()
    {
        float x = 0, z = 0;
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
            moveVector = cam.transform.right * x + cam.transform.forward * (z + 2*sprintValue);
        }
        anim.SetFloat("Xstate", x);
        anim.SetFloat("Zstate", z + sprintValue);

        if (controller.isGrounded)
        {
            vertSpeed = 0;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                vertSpeed = jumpForce;
            }
        }

        vertSpeed += grav * Time.deltaTime;
        moveVector = new Vector3(moveVector.x * speed * Time.fixedDeltaTime,
            vertSpeed * Time.deltaTime, moveVector.z * speed * Time.fixedDeltaTime);
        if (moveVector != Vector3.zero)
        {
            controller.Move(moveVector);
        }
    }
    private void PlayerRotate()
    {
        float x = 0, y = 0;
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            var h = Input.GetAxis("Mouse X");
            x = Mathf.Sign(h);
            var v = Input.GetAxis("Mouse Y");
            y = Mathf.Sign(v);
            rotationX = transform.localEulerAngles.y + h * xSpeed;
            rotationY += v * ySpeed;
            rotationY = Mathf.Clamp(rotationY, minY, maxY);
            transform.localEulerAngles = new Vector3(0, rotationX, 0);
            RotateToValue(x, y);
            rotationY = currentY;
            if(transform.rotation.y >= minY && transform.rotation.y <= maxY)
            {
                cam.transform.RotateAround(transform.position, transform.right, rotationY);
            }
            //cam.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        }
        anim.SetFloat("RotateX", currentX);
        anim.SetFloat("RotateY", currentY);
    }
    private void Sprint()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0)
        {
            sprint = true;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetAxis("Vertical") <= 0)
        {
            sprint = false;
        }

        if(sprint)
        {
            if (sprintValue + Time.deltaTime <= 1)
            {
                sprintValue += Time.deltaTime;
            }
            else
            {
                sprintValue = 1;
            }
        }
        else
        {
            if (sprintValue - Time.deltaTime >= 0.3f)
            {
                sprintValue -= Time.deltaTime;
            }
            else
            {

                sprintValue = 0;
            }
        }

    }
    private void RotateToValue(float x, float y)
    {
        float X = 0, Y = 0;

        if (currentX != x)
        {
            if (currentX < x)
            {
                X = 1;
            }
            if (currentX > x)
            {
                X = -1;
            }
            float distance, step;
            step = 0.1f;
            distance = Mathf.Abs(x - currentX);
            if (step < distance)
            {
                currentX += X * step;
            }
            else
            {
                currentX = x;
            }
        }

        if (currentY < maxY && currentY > minY)
        {
            if (currentY != x)
            {
                if (currentY < x)
                {
                    Y = 1;
                }
                if (currentX > x)
                {
                    Y = -1;
                }
                float distance, step;
                step = 0.1f;
                distance = Mathf.Abs(x - currentX);
                if (step < distance)
                {
                    currentY += Y * step;
                }
                else
                {
                    currentY = y;
                }
            }
        }
        else
        {
            currentY = 0;
        }
    }
    #endregion

    #region Взаимодействие

    public void GetDamage(int damage)
    {
        if(specKit.Kit != KitType.NoSuit)
        {
            int result = 1;
            if(damage - specKit.armorPoint > 1)
            {
                result = damage - specKit.armorPoint;
            }
            Health -= result;
        }
        else
        {
            Health -= damage;
        }
    }
    public void PutOnSuit(SpecKit kit)
    {
        Protected = false;
        Debug.Log(kit);
        specKit.Initiolize(kit);
    }


    private void OnTriggerEnter(Collider other)
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        SecurityBotsSystem system;
        if(MyGetComponent(other.gameObject, out system))
        {
            //system.SetTarget(transform.position + Vector3.up * 1.5f, this);
            if (Protected)
            {
                system.SetActive(-1);
            }
            else
            {
                system.SetActive(1);
            }
        }

        SpecKit kit;
        if(MyGetComponent(other.gameObject, out kit))
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                anim.SetTrigger("Action");
                PutOnSuit(kit);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        SecurityBotsSystem system;
        if (MyGetComponent(other.gameObject, out system))
        {
            system.SetActive(0);
        }
    }
    #endregion

    #region Способности

    private void UseKit()
    {
        if(Input.GetKeyDown(KeyCode.F) && specKit.Kit != KitType.NoSuit)
        {
            if(specKit.Kit == KitType.Comandor && state == PlayerState.active)
            {
                Protected = !Protected;
            }
            else if(specKit.Kit == KitType.Sniper)
            {
                if(state == PlayerState.active)
                {
                    state = PlayerState.inAir;
                    anim.SetInteger("State", 1);
                }
                else if(state == PlayerState.inAir)
                {
                    state = PlayerState.active;
                    anim.SetInteger("State", 0);
                }
            }
            //specKit.Action();
        }
    }

    private void OnAirMove()
    {
        float x = 0, z = 0, y = 0;
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetAxis("UpDownAxis") != 0)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
            y = Input.GetAxis("UpDownAxis");
            moveVector = cam.transform.right * x + cam.transform.forward * z + cam.transform.up * y;
            anim.SetFloat("Xstate", x);
            anim.SetFloat("Zstate", z);
        }
        moveVector = new Vector3(moveVector.x * Time.fixedDeltaTime, moveVector.y * Time.deltaTime, moveVector.z * Time.fixedDeltaTime);
        if (moveVector != Vector3.zero)
        {
            controller.Move(moveVector);
        }
    }

    #endregion

    #region Камера
    private void SetCamTarget(Transform target)
    {
       
    }
    private void CamMove()
    {
       
    }
    private void CamRotation()
    {
       
    }
    private void GetInterface()
    {
        
    }
    public void Exitinterface()
    {
       
    }
    #endregion

}


