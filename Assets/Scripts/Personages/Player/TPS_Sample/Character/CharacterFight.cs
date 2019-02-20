﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFight : MonoBehaviour, IPlayerPart {

    public FightWeapon weapon;
    public CharacterStatus characterStatus;
    private Animator anim;

    private bool delay;
    [Range(0,3)]
    private int impuctNumber;

    public void SwordInput()
    {
        if(!delay)
        {
            weapon.gameObject.SetActive(true);
            impuctNumber++;
            anim.applyRootMotion = true;
            characterStatus.isFight = true;
            characterStatus.isAiming = false;
            characterStatus.isAimingMove = false;
            anim.SetTrigger("Sword");
            anim.SetInteger("SwordCount", impuctNumber);
            weapon.SetValue(impuctNumber);
            weapon.SetActive(true);
            delay = true;
        }
    }
    private void ReturnSword()
    {
        characterStatus.isFight = false;
        anim.applyRootMotion = false;
        impuctNumber = 0;
        anim.SetInteger("SwordCount", impuctNumber);
        weapon.SetActive(false);
    }
    private void OpenSwordInput()
    {
        delay = false;
    }

    public void Initiolize(SampleController sampleController)
    {
        anim = sampleController.anim;
    }
}
