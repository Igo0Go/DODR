using UnityEngine;

public class RollerConroller : MyTools {

    [HideInInspector]
    public CharacterReactions characterReactions;
    public float speed;
    public Transform cam;
    public Transform remoteControl;
    [Tooltip("0 - фанарик, 1 - рука, 2 - паяльник, 3 пила")]
    public Animator[] anim;


 
    private RollerCamera rollerCamera;
    private Vector3 moveVector;
    private Rigidbody rb;
    private bool isMoving;

    // Use this for initialization
    void Start () {
        isMoving = true;
        Cursor.lockState = CursorLockMode.Locked;
        moveVector = remoteControl.forward;
        rb = GetComponent<Rigidbody>();
        rollerCamera = cam.gameObject.GetComponent<RollerCamera>();
        ResetArm();
	}
    private void Update()
    {
        if(isMoving)
        {
            CheckLight();
        }
    }
    private void FixedUpdate()
    {
        if(isMoving)
        {
            UpdateMove();
        }
    }

    public void Initioze(CharacterReactions reactions)
    {
        characterReactions = reactions;
    }

    private void UpdateMove()
    {
        float x, z;
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        if(x != 0 || z != 0)
        {
            moveVector = cam.right * x + cam.forward * z;
            moveVector.y = 0;

            rb.AddForce(moveVector * speed * Time.fixedDeltaTime, ForceMode.Impulse);
        }
        remoteControl.forward = moveVector;
        cam.position = transform.position - rollerCamera.camOffset;
    }
    private void CheckLight()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            anim[0].SetBool("Active", !anim[0].GetBool("Active"));
        }
    }
    private void UseDestroyer()
    {
        anim[2].SetBool("Active", false);
        bool active = anim[3].GetBool("Active");
        if(!active)
        {
            bool key = anim[1].GetBool("Active");
            if (!key)
            {
                anim[1].SetBool("Active", !key);
            }
            anim[3].SetBool("Active", !active);
        }
    }
    private void UseCreator()
    {
        anim[3].SetBool("Active", false);
        bool active = anim[2].GetBool("Active");
        if (!active)
        {
            bool key = anim[1].GetBool("Active");
            if (!key)
            {
                anim[1].SetBool("Active", !key);
            }
            anim[2].SetBool("Active", !active);
        }
    }
    private void UseArm()
    {
        bool key = anim[1].GetBool("Active");
        if (!key)
        {
            anim[1].SetBool("Active", !key);
        }
    }
    private void ResetArm()
    {
        for (int i = 1; i < anim.Length; i++)
        {
            anim[i].SetBool("Active", false);
        }
        rb.isKinematic = false;
        isMoving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        AnimActivator anim;
        if (MyGetComponent(other.gameObject, out anim))
        {
            if(anim.triggerReactor)
            {
                anim.SetActiveForAll(true);
            }
        }
        if (other.tag.Equals("DeadZone"))
        {
            characterReactions.UseKit();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        ComputerSistem comp;
        if (MyGetComponent(other.gameObject, out comp))
        {
            if (Input.GetKeyDown(KeyCode.E) && isMoving)
            {
                isMoving = false;
                transform.position = comp.dronPoint.position;
                remoteControl.rotation = comp.dronPoint.rotation;
                moveVector = remoteControl.forward;
                rb.isKinematic = true;
                if(comp.energyStatus)
                {
                    UseDestroyer();
                }
                else
                {
                    UseCreator();
                }
                comp.FinalWork(comp.workTime);
                Invoke("ResetArm", comp.workTime);
            }
        }

        UsingObject obj;
        if (MyGetComponent(other.gameObject, out obj))
        {
            if (Input.GetKeyDown(KeyCode.E) && isMoving)
            {
                UseArm();
                obj.Use();
                Invoke("ResetArm", 1);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        AnimActivator anim;
        if (MyGetComponent(other.gameObject, out anim))
        {
            if(anim.triggerReactor)
            {
                anim.SetActiveForAll(false);
            }
        }
    }
}
