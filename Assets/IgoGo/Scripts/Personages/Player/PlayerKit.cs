using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKit : MyTools
{
    private GameObject _helmet;
    private GameObject _belt;
    private GameObject _suit;
    private GameObject _pult;
    private Animator anim;

    public Transform helmetPos;
    public Transform beltPos;
    public Transform suitPos;
    public Transform pultPos;
    public int armorPoint;
    public KitType Kit
    {
        get { return _kit; }
        set
        {
            switch(value)
            {
                case KitType.NoSuit:
                    Action = null;
                    break;
                case KitType.Engineer:
                    Action = EngineerAction;
                    rollerModel = Suit.transform.GetChild(0).transform.GetChild(0).gameObject;
                    break;
                case KitType.Comandor:
                    Action = ComandorAction;
                    break;
                case KitType.Sniper:
                    Action = SniperAction;
                    break;
            }
            _kit = value;
        }
    }
    [HideInInspector] public SimpleHandler Action;

    public GameObject Helmet
    {
        get
        {
            return _helmet;
        }
        set
        {
            if (_helmet != null)
            {
                Destroy(_helmet);
            }
            _helmet = Instantiate(value, helmetPos);
            _helmet.transform.localPosition = Vector3.zero;
        }
    }
    public GameObject Belt
    {
        get
        {
            return _belt;
        }
        set
        {
            if (_belt != null)
            {
                Destroy(_belt);
            }
            _belt = Instantiate(value, beltPos);
            _belt.transform.localPosition = Vector3.zero;
        }
    }
    public GameObject Suit
    {
        get
        {
            return _suit;
        }
        set
        {
            if (_suit != null)
            {
                Destroy(_suit);
            }
            _suit = Instantiate(value, suitPos);
            _suit.transform.localPosition = Vector3.zero;
        }
    }
    public GameObject Pult
    {
        get
        {
            return _pult;
        }
        set
        {
            if (_pult != null)
            {
                Destroy(_pult);
            }
            _pult = Instantiate(value, pultPos);
            _pult.transform.localPosition = Vector3.zero;
        }
    }

    private GameObject rollerDronPrefab;

    private KitType _kit;
    private GameObject roller;
    private GameObject rollerModel;

    private void Start()
    {
        Kit = KitType.NoSuit;
        active = false;
    }

    private void Update()
    {
        if (active)
        {
            if (Kit == KitType.Sniper)
            {
                RoketJumpController();
            }
        }
    }
    
    public void Initiolize(SpecKit specKit)
    {
        Helmet = specKit.helmet;
        Belt = specKit.belt;
        Suit = specKit.suit;
        Pult = specKit.pult;

        Kit = specKit.type;
        if(Kit == KitType.Engineer)
        {
            rollerDronPrefab = specKit.rollerDronPrefab;
        }
        if (specKit.animObj)
        {
            anim = Suit.GetComponent<Animator>();
        }
        armorPoint = specKit.ArmorPoints;
    }


    private bool active;

    public void RemoveKit()
    {
        if(Kit != KitType.NoSuit)
        {
            Destroy(helmetPos.GetChild(0).gameObject);
            Destroy(pultPos.GetChild(0).gameObject);
            Destroy(beltPos.GetChild(0).gameObject);
            Destroy(suitPos.GetChild(0).gameObject);
        }

        Kit = KitType.NoSuit;
        anim = null;
        armorPoint = 0;
    }
    private void ComandorAction()
    {
        active = !active;
        anim.SetBool("Active", active);
    }
    private void EngineerAction()
    {
        if(roller != null)
        {
            Destroy(roller);
            rollerModel.SetActive(true);
        }
        else
        {
            roller = Instantiate(rollerDronPrefab, transform);
            roller.transform.localPosition = Vector3.zero + transform.right * 1.5f + transform.up * 0.4f;
            roller.transform.parent = null; 
            rollerModel.SetActive(false);
            roller.transform.GetChild(1).GetComponent<RollerConroller>().Initioze(gameObject.GetComponent<CharacterReactions>());
        }
    }
    private void SniperAction()
    {
        active = !active;
        anim.SetBool("Active", active);
    }

    private void RoketJumpController()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("Jump");
        }
    }

}
