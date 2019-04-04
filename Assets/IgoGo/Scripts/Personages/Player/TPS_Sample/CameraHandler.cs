using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MyTools, IPlayerPart {

    public Transform cameraTransform;
    public Transform pivot;
    public Transform character;
    public Transform mainTransform;
    public Transform targetLook;

    [HideInInspector] public Camera cam;
    public LayerMask noPlayerMask;
    public LayerMask engineerMask;

	//hi

    public CameraConfig cameraConfig;

    public bool leftPivot;
    [HideInInspector] public bool rotateMode;
    public bool noObstacles;
    [HideInInspector]
    public bool smoothyCam;

    public float smoothX;
    public float smoothY;
    
    public float smoothXVelocity;
    public float smoothYVelocity;

    public float lookAngel;
    public float titlAngel;

    [HideInInspector]
    public bool StaticCam
    {
        get
        {
            return _staticCam;
        }
        set
        {
            _staticCam = value;

            if(_staticCam)
            {
                angelBufer = lookAngel;
            }
        }
    }

    private CharacterStatus characterStatus;
    private CharacterInput characterInput;

    private Vector3 CharacterPos
    {
        get { return character.position + character.up * 1.5f; }
    }
    private Vector3 standartCamLocalPos;
    private LayerMask defaultMask;
    private float step;
    private float mouseX;
    private float mouseY;
    private float angelBufer;
    private float fieldBufer;
    private int ignoreLayer;
    private bool obstacle;
    private bool _staticCam;

    #region Служебные переменные
    private Vector3 newCamPosition;
    private Vector3 dir;
    private Vector3 newPivotPosition;
    private Vector3 targetPosition;
    private Quaternion targetRot;
    #endregion

    private void Start()
    {
		cam = cameraTransform.GetComponent<Camera>();
        fieldBufer = cam.fieldOfView;
    }

    private void LateUpdate()
    {
        Tick();
        TargetLook();
        CameraAngel();
    }

    private void TargetLook()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward * 100);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2000, noPlayerMask))
        {
            targetLook.position = Vector3.Lerp(targetLook.position, hit.point, Time.deltaTime * 40);
        }
        else
        {
            targetLook.position = Vector3.Lerp(targetLook.position, targetLook.transform.forward * 200, Time.deltaTime * 5);
        }
    }

    private void Tick()
    {
        HandlePosition();
        HandleRotation();

        targetPosition = Vector3.Lerp(mainTransform.position, character.position, 1);
        mainTransform.position = targetPosition;
    }
    private void HandlePosition()
    {
        float targetX = cameraConfig.normalX;
        float targetY = cameraConfig.normalY;
        float targetZ = cameraConfig.normalZ;

        if (leftPivot)
        {
            targetX = -targetX;
        }


        if (characterStatus.isAiming)
        {
            if(leftPivot)
            {
                targetX = -cameraConfig.aimX;
            }
            else
            {
                targetX = cameraConfig.aimX;
            }
            targetZ = cameraConfig.aimZ;
        }

        

        newPivotPosition = pivot.localPosition;
        newPivotPosition.x = targetX;
        newPivotPosition.y = targetY;



        dir = cameraTransform.position - CharacterPos;
        RaycastHit hit;
        if (Physics.Raycast(CharacterPos, dir, out hit, Vector3.Distance(CharacterPos, cameraTransform.position), noPlayerMask))
        {
            newCamPosition = hit.point;
            obstacle = true;
        }
        else
        {
            cameraTransform.localPosition = new Vector3(0, 0, cameraTransform.localPosition.z);
            newCamPosition = cameraTransform.localPosition;
            newCamPosition.z = targetZ;
            obstacle = false;
        }       
        pivot.localPosition = Vector3.Lerp(pivot.localPosition, newPivotPosition, step);
        if(obstacle)
        {
            cameraTransform.position = newCamPosition;
        }
        else
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newCamPosition, step);
        }
    }
    private void HandleRotation()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        characterStatus.currentXrot = mouseX;

        if(cameraConfig.turnSmooth > 0)
        {
            smoothX = Mathf.SmoothDamp(smoothX, mouseX, ref smoothXVelocity, cameraConfig.turnSmooth);
            smoothY = Mathf.SmoothDamp(smoothY, mouseY, ref smoothYVelocity, cameraConfig.turnSmooth);
        }
        else
        {
            smoothX = mouseX;
            smoothY = mouseY;
        }

        lookAngel += smoothX * characterInput.sniperMultiplicator * cameraConfig.xRotSpeed;

        if (StaticCam)
        {
            lookAngel = Mathf.Clamp(lookAngel, angelBufer + cameraConfig.minXAngel, angelBufer + cameraConfig.maxXAngel);
        }
       
            targetRot = Quaternion.Euler(0, lookAngel, 0);
            mainTransform.rotation = targetRot;
        
        titlAngel -= smoothY * characterInput.sniperMultiplicator * cameraConfig.yRotSpeed;
        titlAngel = Mathf.Clamp(titlAngel, cameraConfig.minYAngel, cameraConfig.maxYAngel);
        pivot.localRotation = Quaternion.Euler(titlAngel, 0, 0);
    }

    public void SetMask(KitType kit)
    {
        switch(kit)
        {
            case KitType.NoSuit:
                cam.cullingMask = defaultMask;
                break;
            case KitType.Engineer:
                cam.cullingMask = engineerMask;
                break;
        }
    }
    public void EnabledCamera(bool value)
    {
        cam.gameObject.SetActive(enabled);
    }
    public void CameraAngel()
    {
        if(smoothyCam)
        {
            cam.fieldOfView = SmoothlyChange(cam.fieldOfView, 40, 100 * Time.deltaTime);
        }
        else
        {
            cam.fieldOfView = SmoothlyChange(cam.fieldOfView, fieldBufer, 100 * Time.deltaTime);
        }
    }

    public void Initiolize(SampleController sampleController)
    {
        rotateMode = true;
        step = Time.fixedDeltaTime * cameraConfig.pivotSpeed;
        defaultMask = cam.cullingMask;
        characterStatus = sampleController.characterStatus;
        characterInput = sampleController.characterInput;
    }
}
