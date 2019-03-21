using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MyTools, IPlayerPart {

    public Animator anim;
    public CharacterStatus characterStatus;
    public CharacterMovement characterMovement;

    [Range(0,1)][SerializeField] private float m_RunCycleLegOffset;
    private float jumpTime;
    private float jumpLeg;

    public void UpdateAnimation () {

        anim.SetBool("Sprint", characterStatus.isSprint);
        anim.SetBool("Aiming", characterStatus.isAiming);
        anim.SetBool("AimingMove", characterStatus.isAimingMove);

        if (!characterStatus.isAiming)
        {
            AnimationNormal();
        }
        else
        {
            AnimationAiming();
        }

        JumpAnim();
        
    }
    private void AnimationNormal()
    {
        anim.SetFloat("Zstate", characterMovement.moveAmounth + characterMovement.sprintValue, 0.15f, Time.deltaTime);

        if (!characterStatus.isBehindCover)
        {
            if (anim.GetFloat("Zstate") < 0.5f)
            {
                anim.SetFloat("RotateX", 0);
            }
            else
            {
                anim.SetFloat("RotateX", characterStatus.currentXrot, 0.15f, Time.deltaTime);
            }
        }
    }
    private void AnimationAiming()
    {
        float v = characterMovement.vertical;
        float h = characterMovement.horizontal;

        anim.SetFloat("Zstate", v, 0.15f, Time.deltaTime);
        anim.SetFloat("Xstate", h, 0.15f, Time.deltaTime);
    }
    private void JumpAnim()
    {
        if(characterStatus.isMove)
        {
            jumpLeg += jumpLeg + m_RunCycleLegOffset;
            jumpLeg = (jumpLeg < 0.5f ? 1 : -1);
        }
        else if(jumpLeg != 0)
        {
            jumpLeg = SmoothlyChange(jumpLeg, 0, m_RunCycleLegOffset);
        }
        anim.SetBool("OnGround", characterStatus.isGround);
        if (characterStatus.isGround)
        {
            if(jumpTime > 0)
            {
                jumpTime = 0;
            }
            else
            {
                anim.SetFloat("JumpLeg", jumpLeg);
            }
            jumpTime = SmoothlyChange(jumpTime, 0, Time.deltaTime * 10);
            anim.SetFloat("Jump", jumpTime);
            return;
        }
        else
        {
            jumpTime += Time.deltaTime * 15;
            anim.SetFloat("Jump", jumpTime);
        }
    }

    public void Initiolize(SampleController sampleController)
    {
        anim = sampleController.anim;
        characterStatus = sampleController.characterStatus;
        characterMovement = sampleController.characterMovement;
    }
    public void ResetValues()
    {
        characterStatus.isAimingMove = characterStatus.isAiming = characterStatus.isSprint = false;
        anim.SetBool("Sprint", characterStatus.isSprint);
        anim.SetBool("Aiming", characterStatus.isAiming);
        anim.SetBool("AimingMove", characterStatus.isAimingMove);

        anim.SetFloat("Zstate", 0);
        anim.SetFloat("Xstate", 0);
        anim.SetFloat("RotateX", 0);
    }
}
