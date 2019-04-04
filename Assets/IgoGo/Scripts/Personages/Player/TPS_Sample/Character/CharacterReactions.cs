using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum KitType
{
    NoSuit,
    Comandor,
    Sniper,
    Engineer
}

public enum PlayerState
{
    active,
    disactive,
    dead,
    inAir
}

	//hi

public interface IAlive
{
    float Health { get; set; }

    void GetDamage(int damage);
    void Dead();
    void ShockEffect(float time);
}

public class CharacterReactions : MyTools, IAlive {

    public Slider healthSlider;
    public PlayerKit specKit;
    public bool Protected;

    public PlayerState State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
            sampleController.CheckState(_state);
        }
    }
    public float Health
    {
        get { return healthSlider.value; }
        set
        {
            healthSlider.value = value;
            if(healthSlider.value <= 0)
            {
                Dead();
            }
        }
    }

    [SerializeField]
    private PlayerState _state;
    private SampleController sampleController;
    private CharacterInventory characterInventory;
    private CharacterStatus characterStatus;
    private CharacterMovement characterMovement;
    public CharacterStels characterStels;
    private CharacterInput characterInput;
    private CameraHandler cameraHandler;
    private Crosshair crosshair;
    private Animator anim;
    private Transform target;

    private event SimpleHandler DeadEvent;

    public void Initiolize(SampleController sampleController)
    {
        this.sampleController = sampleController;
        characterInventory = sampleController.characterInventory;
        characterInput = sampleController.characterInput;
        cameraHandler = sampleController.cameraHandler;
        crosshair = sampleController.crosshair;
        characterMovement = sampleController.characterMovement;
        characterStatus = sampleController.characterStatus;
        characterStels = sampleController.characterStels;
        anim = sampleController.anim;
        target = sampleController.target;
    }

    public void Dead()
    {
        anim.applyRootMotion = true;
        State = PlayerState.dead;
        characterStatus.isBehindCover = false;
        anim.SetBool("Stels", characterStatus.isBehindCover);
        characterStels.CapsulToStelsState(characterStatus.isBehindCover);
        anim.SetBool("Dead", true);
        anim.SetTrigger("DeadTrigger");
        Invoke("NoSuit", 3f);
        Invoke("ReturnActive", 3f);
        if(DeadEvent != null)
        {
            DeadEvent.Invoke();
        }
        DeadEvent = null;
    }
    public void GetDamage(int damage)
    {
        if (Health > 0)
        {
            var result = damage - specKit.armorPoint;
            if (result <= 0)
            {
                result = 1;
            }
            Health = Health - result;
        }
    }
    public void ShockEffect(float time)
    {
        characterStatus.shock = true;
        Invoke("ReturnShock", time);
    }

    public void PutOnSuit(SpecKit kit)
    {
        characterInput.selectedWeapon = 3;
        anim.SetTrigger("ChangeWeapon");
        State = PlayerState.active;
        Protected = false;
        specKit.Initiolize(kit);
        characterInventory.firstWeapon = kit.firstWeapon;
        characterInventory.secondWeapon = kit.secondWeapon;
        cameraHandler.SetMask(kit.type);
    }
    public void UseKit()
    {
        if (specKit.Kit != KitType.NoSuit && State != PlayerState.dead)
        {
            switch(specKit.Kit)
            {
                case KitType.Engineer:
                    sampleController.deadPanel.Play();
                    if(State == PlayerState.disactive)
                    {
                        crosshair.Visible = true;
                        State = PlayerState.active;
                    }
                    else if(State == PlayerState.active)
                    {
                        crosshair.Visible = false;  
                        State = PlayerState.disactive;
                    }
                    break;
                case KitType.Comandor:
                    Protected = !Protected;
                    break;
                case KitType.Sniper:
                    characterMovement.JetPack = !characterMovement.JetPack;
                    break;
            }
            specKit.Action();
        }
    }
    private void NoSuit()
    {
        characterStatus.isAiming = false;
        anim.SetInteger("WeaponType", 0);
        characterInput.selectedWeapon = 3;
        anim.SetTrigger("ChangeWeapon");
        Protected = false;
        specKit.RemoveKit();
        characterInventory.firstWeapon = null;
        characterInventory.secondWeapon = null;
        cameraHandler.SetMask(KitType.NoSuit);
        characterMovement.JetPack = false;
    }
    private void ReturnActive()
    {
        anim.applyRootMotion = false;
        anim.SetBool("Dead", false);
        Health = 100;
        State = PlayerState.active;
    }
    private void ReturnShock()
    {
        characterStatus.shock = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        AnimActivator anim;
        if(MyGetComponent(other.gameObject, out anim))
        {
            if(anim.triggerReactor)
            {
                anim.SetActiveForAll(true);
                return;
            }
        }

        UsingObject reactor;
        if (MyGetComponent(other.gameObject, out reactor))
        {
            reactor.Use();
            return;
        }


        //LocationReactor reactor;
        //if (MyGetComponent(other.gameObject, out reactor))
        //{
        //    reactor.Use();
        //    return;
        //}

        //MusicChanger changer;
        //if (MyGetComponent(other.gameObject, out changer))
        //{
        //    changer.Use();
        //    return;
        //}

        TurretScript turret;
        if (MyGetComponent(other.gameObject, out turret))
        {
            turret.target = target;
            DeadEvent += turret.Clear;
            return;
        }

        if (other.tag.Equals("Safe"))
        {
            sampleController.Safe();
            Health = 100;
        }
        if(other.tag.Equals("DeadZone"))
        {
            Dead();
        }
        if (other.tag.Equals("NoSuit"))
        {
            NoSuit();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        UsingObject obj;
        if (MyGetComponent(other.gameObject, out obj))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                obj.Use();
            }
        }

        SecurityBotsSystem system;
        if (MyGetComponent(other.gameObject, out system))
        {
            system.SetTarget(transform.position + Vector3.up * 1.5f, this);
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
        if (MyGetComponent(other.gameObject, out kit))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PutOnSuit(kit);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        AnimActivator anim;
        if (MyGetComponent(other.gameObject, out anim))
        {
            anim.SetActiveForAll(false);
            return;
        }

        SecurityBotsSystem system;
        if (MyGetComponent(other.gameObject, out system))
        {
            system.SetActive(0);
            return;
        }

        TurretScript turret;
        if (MyGetComponent(other.gameObject, out turret))
        {
            turret.target = null;
            DeadEvent -= turret.Clear;
            return;
        }

        LocationReactor reactor;
        if (MyGetComponent(other.gameObject, out reactor))
        {
            if(!reactor.enterOnly)
            {
                reactor.Use();
            }
            return;
        }
    }
}


