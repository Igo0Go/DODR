using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/CharacterStatus")]
public class CharacterStatus : ScriptableObject {

    public float standartJumpForce;
    public float standartGravityForce;
    public bool isBehindCover;
    public bool isMove;
    public bool isAimingMove;
    public bool isAiming;
    public bool isSprint;
    public bool isGround;
    public bool isJump;
    public bool isFight;
    public bool onWall;
    public bool sniper;

    public float currentXrot;
    public float rotationSpeed;
}
