using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FightWeapon : Weapon {

    public Animator anim;

    private int result;
    bool protect;

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
        protect = !protect;
        anim.SetBool("Protect", protect);
    }
    public void StopProtect()
    {
        protect = false;
        anim.SetBool("Protect", protect);
    }

    private void Update()
    {
        anim.SetFloat("Position", result, 0.1f, 5 * Time.deltaTime);
    }
}
