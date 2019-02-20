using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Sword,
    OneShoot,
    Automatic,
    Ray
}

[CreateAssetMenu(menuName = "Config/Weapon")]
public class WeaponConfig : ScriptableObject {

    public int damage;
    public WeaponType type;
    public GameObject itemPrefab;

    [Space(10)]
    public Vector3 rHandPos;
    public Vector3 rHandRot;
    public GameObject weaponPrefab;
    public Vector3 weaponPos;
    public Vector3 weaponRot;
}
