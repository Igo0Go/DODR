using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightController : MonoBehaviour {

    private int fightIndex;
    private Animator anim;

	// Use this for initialization
	void Start () {
        fightIndex = 0;
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        Fight();
    }


    #region Ближний бой

    private void Fight()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            fightIndex++;
            anim.SetInteger("FightIndex", fightIndex);
            anim.SetTrigger("Fight");
        }
    }

    private void StopFight()
    {
        fightIndex = 0;
        anim.SetFloat("FightIndex", fightIndex);
    }

    #endregion
}
