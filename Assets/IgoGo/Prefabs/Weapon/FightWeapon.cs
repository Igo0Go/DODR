using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FightWeapon : Weapon {

    public Animator anim;

    [Space(10)]
    [Range(1,100)]
    public int maxEnergy;
    [Range(1,10)]
    public float recoilTime;
    public int Energy
    {
        get { return _energy; }
        set
        {
            _energy = value;
            if(_energy <= 0)
            {
                delay = true;
                anim.SetBool("NoEnergy", true);
                anim.SetBool("Protect", false);
                Invoke("ReturnDelay", recoilTime);
            }
        }
    }

    public bool delay;

    private int result;
    bool protect;

    private int _energy;

    public void SetActive(bool value)
    {
        anim.SetBool("Active", value);
    }
    public void SetValue(int value)
    {
        switch(value)
        {
            case 1:
                result = -1;
                break;
            case 2:
                result = 1;
                break;
            case 3:
                result = 0;
                break;
        }
    }

    public void CheckProtect()
    {
        if(!delay)
        {
            protect = !protect;
            anim.SetBool("Protect", protect);
        }
    }
    public void StopProtect()
    {
        protect = false;
        anim.SetBool("Protect", protect);
    }


    private void Start()
    {
        Energy = maxEnergy;
    }
    private void Update()
    {
        anim.SetFloat("Position", result, 0.1f, 5 * Time.deltaTime);
    }
    private void ReturnDelay()
    {
        anim.SetBool("NoEnergy", false);
        delay = false;
        Energy = maxEnergy;
    }
}
