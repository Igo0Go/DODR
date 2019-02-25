﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public interface IPlayerPart
{
    void Initiolize(SampleController sampleController);
}

public class SampleController : MonoBehaviour {

    

    public CharacterStatus characterStatus;
    public CameraHandler cameraHandler;
    [Space(10)]
    public CharacterMovement characterMovement;
    public CharacterAnimation characterAnimation;
    public CharacterReactions characterReactions;
    public CharacterFight characterFight;
    public CharacterIK characterIK;
    public CharacterInventory characterInventory;
    public CharacterInput characterInput;
    public CharacterClimb characterClimb;
    public CharacterStels characterStels;

    [Space(20)]
    public PlayableDirector deadPanel;
    public ParticleSystem respawn;
    public Crosshair crosshair;
    public Animator anim;
    public Slider ammoSlider;
    public Transform target;

    [HideInInspector] public AudioSource aud;
    [HideInInspector] public CharacterController characterController;


    private Vector3 safePoint;
    private Quaternion safeRot;

    private bool _enabled;

    private bool Enabled
    {
        get
        {
            return _enabled;
        }
        set
        {
            if (value != _enabled)
            {
                _enabled = value;
                characterAnimation.ResetValues();
                characterAnimation.enabled = _enabled;
                characterInput.enabled = _enabled;
                cameraHandler.cam.gameObject.SetActive(_enabled);
                cameraHandler.enabled = _enabled;
            }
        }
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        aud = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;

        cameraHandler.Initiolize(this);
        characterMovement.Initiolize(this);
        characterAnimation.Initiolize(this);
        characterReactions.Initiolize(this);
        characterFight.Initiolize(this);
        characterIK.Initiolize(this);
        characterInventory.Initiolize(this);
        characterInput.Initiolize(this);
        characterClimb.Initiolize(this);
        characterStels.Initiolize(this);
        ammoSlider.value = 0;

        StandartStats();
    }

    private void Update()
    {
        characterMovement.MoveUpdate();
        if (!characterStatus.onWall)
        {
            characterInventory.InventoryUpdate();
            characterInput.InputUpdate();
            if (!characterStatus.isBehindCover)
            {
                characterAnimation.UpdateAnimation();
            }
        }
    }

    private void StandartStats()
    {
        characterStatus.isAiming = false;
        characterStatus.isAimingMove = false;
        characterStatus.isBehindCover = false;
        characterStatus.isFight = false;
        characterStatus.isSprint = false;
        characterStatus.onWall = false;
        characterStatus.sniper = false;
    }

    public void CheckState(PlayerState state)
    {
        switch(state)
        {
            case PlayerState.dead:
                NewLife();
                Invoke("DarkMonitor",0.5f);
                break;
            case PlayerState.disactive:
                Enabled = false;
                break;
            case PlayerState.active:
                Enabled = true;
                break;
        }
        characterMovement.State = state;
    }

    public void Safe()
    {
        safePoint = transform.position;
        safeRot = transform.rotation;
    }

    private void NewLife()
    {
        characterMovement.ResetValues();
        characterClimb.ResetAll();
        respawn.Play();
        characterReactions.Health = 100;
        characterInventory.RemoveAllShootWeapon();
    }
    private void DarkMonitor()
    {
        deadPanel.Play();
        transform.position = safePoint;
        transform.rotation = safeRot;
    }
}
