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

public interface IAlive
{
    float Health { get; set; }

    void GetDamage(int damage);
    void Dead();
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
            float _health = value;
            if(_health < 0)
            {
                _health = 0;
                Dead();
            }
            healthSlider.value = _health;
        }
    }

    [SerializeField]
    private PlayerState _state;
    private SampleController sampleController;
    private CharacterInventory characterInventory;
    private CharacterInput characterInput;
    private CameraHandler cameraHandler;
    private Crosshair crosshair;
    private CharacterStatus characterStatus;
    private Animator anim;

    public void Initiolize(SampleController sampleController)
    {
        this.sampleController = sampleController;
        characterInventory = sampleController.characterInventory;
        characterInput = sampleController.characterInput;
        cameraHandler = sampleController.cameraHandler;
        characterStatus = sampleController.characterStatus;
        crosshair = sampleController.crosshair;
        anim = sampleController.anim;
    }

    public void Dead()
    {
        anim.applyRootMotion = true;
        State = PlayerState.dead;
        anim.SetBool("Dead", true);
        anim.SetTrigger("DeadTrigger");
        Invoke("NoSuit", 3f);
        Invoke("ReturnActive", 3f);
    }
    public void GetDamage(int damage)
    {
        if(Health > 0)
        {
            var result = damage - specKit.armorPoint;
            if (result <= 0)
            {
                result = 1;
            }
            Health = -result;
        }
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
            }
            specKit.Action();
        }
    }
    private void NoSuit()
    {
        characterInput.selectedWeapon = 3;
        anim.SetTrigger("ChangeWeapon");
        Protected = false;
        specKit.RemoveKit();
        characterInventory.firstWeapon = null;
        characterInventory.secondWeapon = null;
        cameraHandler.SetMask(KitType.NoSuit);
    }
    private void ReturnActive()
    {
        anim.applyRootMotion = false;
        anim.SetBool("Dead", false);
        Health = 100;
        State = PlayerState.active;
    }

    private void OnTriggerEnter(Collider other)
    {
        AnimActivator anim;
        if(MyGetComponent(other.gameObject, out anim))
        {
            if(anim.triggerReactor)
            {
                anim.SetActiveForAll(true);
            }
        }
        if(other.tag.Equals("Safe"))
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
                anim.SetTrigger("Action");
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
        }
        SecurityBotsSystem system;
        if (MyGetComponent(other.gameObject, out system))
        {
            system.SetActive(0);
        }
    }
}

public abstract class MyTools : MonoBehaviour
{
    public static bool MyGetComponent<T>(GameObject obj, out T component)
    {
        component = obj.GetComponent<T>();
        if (component == null)
        {
            return false;
        }
        return true;
    }

    public static bool ContainsPhisicsMaterial(Collider obj, out PhysicMaterial component)
    {
        component = obj.sharedMaterial;
        if (component == null)
        {
            return false;
        }
        return true;
    }

    public static float SmoothlyChange(float min, float max, float value, float step)
    {
        if(max <= min)
        {
            Debug.LogError("В методе SmootlyChange неправильно указаны границы.");
        }

        float result = value;

        result += step;
        if(result > max || result < min)
        {
            if(result > max)
            {
                return max;
            }
            if(result < min)
            {
                return min;
            }
        }
        return result;
    }

    public static float SmoothlyChange(float value, float target, float step)
    {
        if(value == target)
        {
            return target;
        }

        int multiply = (target >= value) ? 1 : -1;

        float dir = multiply * step;

        if(Mathf.Abs(target - value) > dir)
        {
            return value + dir;
        }
        else
        {
            return target;
        }
    }
}
