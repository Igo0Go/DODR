using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyReactor : MonoBehaviour {

    public FightWeapon fightWeapon;

    public void GetDamage(int damage)
    {
        fightWeapon.Energy = fightWeapon.Energy - damage;
    }
}
