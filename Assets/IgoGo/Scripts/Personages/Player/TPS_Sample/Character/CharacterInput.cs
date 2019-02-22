using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour, IPlayerPart {

    private CharacterStatus characterStatus;
    private CameraHandler cameraHandler;
    private CharacterMovement characterMovement;
    private CharacterInventory characterInventory;
    private CharacterFight characterFight;
    private CharacterReactions characterReactions;
    [HideInInspector] public ShootWeapon shootWeapon;
    [HideInInspector] public FightWeapon fightWeapon;

    public bool opportunityToAim;
    public bool isAiming;
    public bool debugAim;
    [Range(1,3)] public int selectedWeapon;

    [HideInInspector]
    public bool sniperMode;
    [HideInInspector]
    public float sniperMultiplicator;

    private float Distance
    {
        get
        {
            return Vector3.Distance(transform.position + transform.up * 1.4f, targetLook.position);
        }
    }

    private AudioSource aud;
    private SimpleHandler shootDelegate;
    private Animator anim;
    private Transform targetLook;
    private bool leftPivot;
    private bool opportunityToShoot;
    private float _distance;



    // Update is called once per frame
    public void InputUpdate () {

        InputAiming();
        InputSelectWeapon();

        if (characterInventory.fightWeapon != null)
        {
            ProtectInput();
        }

        if(!characterStatus.isBehindCover)
        {
            if (selectedWeapon != 1)
            {
                FightInput();
            }
            if (!characterStatus.isSprint)
            {
                KitInput();
            }
        }
    }

    private void InputSelectWeapon()
    {
        if(!anim.GetBool("Aiming"))
        {
            if(Input.GetKeyDown(KeyCode.Alpha1) && characterInventory.fightWeapon != null && selectedWeapon != 1)
            {
                selectedWeapon = 1;
                anim.SetTrigger("ChangeWeapon");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && characterInventory.secondWeapon != null && selectedWeapon != 2)
            {
                selectedWeapon = 2;
                anim.SetTrigger("ChangeWeapon");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && characterInventory.firstWeapon != null && selectedWeapon != 3)
            {
                selectedWeapon = 3;
                anim.SetTrigger("ChangeWeapon");
            }
        }
    }
    private void SelectWeapon()
    {
        characterInventory.DestroyWeapon();
        characterInventory.SelectWeaponAction(selectedWeapon);
    }
    private void InputAiming()
    {
        RaycastAiming();
       
        

        if(anim.GetInteger("WeaponType") != 0)
        {
            if (characterStatus.sniper)
            {
                SniperMode();
            }

            if(sniperMode)
            {
                sniperMultiplicator = shootWeapon.sniperCam.fieldOfView / 60;
            }
            else
            {
                sniperMultiplicator = 1;
            }

            if ((Input.GetMouseButton(1) && opportunityToAim) || sniperMode)
            {
                if (!characterStatus.isGround)
                {
                    Time.timeScale = 0.3f;
                    aud.Play();
                }
                else
                {
                    Time.timeScale = 1f;
                    aud.Stop();
                }
                characterStatus.isAiming = true;
                characterStatus.isAimingMove = true;
                fightWeapon.transform.position = shootWeapon.shootPoint.transform.position;
                fightWeapon.transform.rotation = shootWeapon.shootPoint.transform.rotation;
                shootDelegate();
            }
            else if (Input.GetMouseButton(1) && !opportunityToAim)
            {
                Time.timeScale = 1f;
                aud.Stop();
                characterStatus.isAiming = false;
                characterStatus.isAimingMove = true;
                characterFight.weapon.StopProtect();
            }
            else if (shootWeapon.weaponConfig.type == WeaponType.Ray)
            {
                Time.timeScale = 1f;
                aud.Stop();
                shootWeapon.ReturnRay();
            }

        }
        if (!Input.GetMouseButton(1) && !sniperMode)
        {
            Time.timeScale = 1f;
            aud.Stop();
            characterStatus.isAiming = false;
            characterStatus.isAimingMove = false;
            characterInventory.ReturnSwordToPos();
            characterFight.weapon.StopProtect();
        }

        if (debugAim)
        {
            characterStatus.isAiming = isAiming;
            characterStatus.isAimingMove = isAiming;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            leftPivot = !leftPivot;
            cameraHandler.leftPivot = leftPivot;
        }
    }
    private void SniperMode()
    {
        if(!characterStatus.onWall && !characterStatus.isBehindCover)
        {
            if (Input.GetMouseButtonDown(2))
            {
                sniperMode = !sniperMode;

                characterStatus.isAiming = sniperMode;
                characterStatus.isAimingMove = sniperMode;
                characterInventory.sniperCam.gameObject.SetActive(sniperMode);
            }

            if (sniperMode)
            {
                float mw = Input.GetAxis("Mouse ScrollWheel");

                if (characterInventory.sniperCam.fieldOfView - mw * 20 < 60)
                {
                    characterInventory.sniperCam.fieldOfView -= mw * 20;
                }
            }
        }
    }
    private void RaycastAiming()
    {
        if (Distance > 1.5f)
        {
            opportunityToAim = true;
        }
        else
        {
            opportunityToAim = false;
        }
    }
    private void FightInput()
    {
        if(characterMovement.State == PlayerState.active)
        {
            if (Input.GetKeyDown(KeyCode.Q) && characterInventory.fighter && !anim.GetBool("Aiming"))
            {
                characterFight.SwordInput();
            }
        }
    }
    private void ProtectInput()
    {
        if (characterMovement.State == PlayerState.active)
        {
            if (Input.GetKeyDown(KeyCode.R) && characterInventory.fightWeapon != null && characterStatus.isAiming)
            {
                characterFight.weapon.CheckProtect();
            }
        }
       
    }
    private void KitInput()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            characterReactions.UseKit();
        }
    }

    public void Initiolize(SampleController sampleController)
    {
        sniperMultiplicator = 1;
        cameraHandler = sampleController.cameraHandler;
        targetLook = cameraHandler.targetLook;
        leftPivot = cameraHandler.leftPivot;
        anim = sampleController.anim;
        characterStatus = sampleController.characterStatus;
        characterMovement = sampleController.characterMovement;
        characterInventory = sampleController.characterInventory;
        characterFight = sampleController.characterFight;
        characterReactions = sampleController.characterReactions;
        aud = sampleController.aud;
    }

    private void InputOneShoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            shootWeapon.shootDelegate();
        }
    }
    private void InputRayShoot()
    {
        if (Input.GetMouseButton(0))
        {
            shootWeapon.shootDelegate();
        }
        if(Input.GetMouseButtonUp(0))
        {
            shootWeapon.ReturnRay();
        }
    }
    private void InputAutoShoot()
    {
        if (Input.GetMouseButton(0))
        {
            shootWeapon.shootDelegate();
        }
    }
    public void SetShootDelegate(WeaponType type)
    {
        if(type != WeaponType.Sword)
        {
            switch(type)
            {
                case WeaponType.OneShoot:
                    shootDelegate = InputOneShoot;
                    break;
                case WeaponType.Automatic:
                    shootDelegate = InputAutoShoot;
                    break;
                case WeaponType.Ray:
                    shootDelegate = InputRayShoot;
                    break;
            }
        }
    }
    private void ShootOpportunity()
    {
        opportunityToShoot = true;
    }
}
