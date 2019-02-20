using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecKit : MonoBehaviour {

    public int ArmorPoints;
    public KitType type;

    public GameObject helmet;
    public GameObject belt;
    public GameObject suit;
    public GameObject pult;
    public WeaponConfig firstWeapon;
    public WeaponConfig secondWeapon;

    [Space(20)]
    public bool animObj;
    [Header("Только для инженера")]
    public GameObject rollerDronPrefab;
}
