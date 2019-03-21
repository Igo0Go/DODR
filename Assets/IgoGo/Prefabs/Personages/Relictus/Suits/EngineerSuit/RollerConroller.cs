using UnityEngine;

public class RollerConroller : MyTools {

    #region Настройки дрона
    [Header("Настройки дрона")]
    [HideInInspector]
    public CharacterReactions characterReactions;
    public float speed;
    public Transform remoteControl;
    [Tooltip("0 - фанарик, 1 - рука, 2 - паяльник, 3 пила")]
    public Animator[] anim;
    #endregion

    #region Настройки камеры
    [Space(20)]
    [Header("Настройки камеры")]
    public Transform camTransform;
    public Transform camRotTransform;
    public Transform camPosTransform;
    public float noTargetDistance;
    public float camSpeed;
    public LayerMask noTargetMask;
    public LayerMask obstacleMask;
    #endregion

    #region Вспомогательные
    private Quaternion camRotation;
    private Vector3 moveVector;
    private Rigidbody rb;
    private Camera cam;
    private LayerMask camOrigin;
    private bool isMoving;
    #endregion

    private float DistanceToCenter
    {
        get
        {
            return Vector3.Distance(camTransform.position, transform.position);
        }
    }
    private float DistanceToCamPoint
    {
        get
        {
            return Vector3.Distance(camTransform.position, camPosTransform.position);
        }
    }

    #region События
    void Start () {
        isMoving = true;
        Cursor.lockState = CursorLockMode.Locked;
        cam = camTransform.GetComponent<Camera>();
        camOrigin = cam.cullingMask;
        moveVector = remoteControl.forward;
        rb = GetComponent<Rigidbody>();
        camRotation = camRotTransform.rotation;
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
    private void LateUpdate()
    {
        ObstacleReaction();
        TargetReaction();
        CamRotation();
    }
    #endregion

    public void Initioze(CharacterReactions reactions)
    {
        characterReactions = reactions;
    }

    #region Основные методы
    private void UpdateMove()
    {
        float x, z;
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        if(x != 0 || z != 0)
        {
            moveVector = camTransform.right * x + camTransform.forward * z;
            moveVector.y = 0;

            rb.AddForce(moveVector * speed * Time.fixedDeltaTime, ForceMode.Impulse);
        }
        remoteControl.forward = moveVector;
    }
    private void CamRotation()
    {
        camRotTransform.rotation = camRotation;
        float mx;
        mx = Input.GetAxis("Mouse X");

        if (mx != 0)
        {
            camRotation.eulerAngles = new Vector3(0, camRotation.eulerAngles.y + mx * camSpeed * Time.deltaTime, 0);
            camRotTransform.rotation = camRotation;
        }
        camTransform.LookAt(transform.position);
    }
    private void ObstacleReaction()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, camTransform.position - transform.position, out hit, DistanceToCenter, obstacleMask))
        {
            camTransform.position = Vector3.Slerp(camTransform.position, hit.point, 15 * Time.deltaTime);
        }
        else if (camTransform.position != camPosTransform.position)
        {
           // Physics.Raycast(camTransform.position, -camTransform.forward, 0.3f, obstacleMask)
            if (DistanceToCamPoint > 0.02)
            {
                camTransform.position = Vector3.Slerp(camTransform.position, camPosTransform.position, 10 * Time.deltaTime);
            }
            else
            {
                camTransform.position = camPosTransform.position;
            }
        }
    }
    private void TargetReaction()
    {
        if (DistanceToCenter < noTargetDistance)
        {
            cam.cullingMask = noTargetMask;
        }
        else
        {
            cam.cullingMask = camOrigin;
        }
    }
    #endregion

    #region Функци дрона
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
    #endregion

    #region Триггеры
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
    #endregion
}
