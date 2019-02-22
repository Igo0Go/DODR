using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void EnergyHandler(bool value);


public class ComputerSistem : MonoBehaviour {

    public Transform dronPoint;
    public float workTime;
    [Header("Активность консоли")]
    public bool energyStatus;
    public ActionObject[] connectingObjcts;
    public AnimActivator anim;

    private EnergyHandler energyEvent;
    private SimpleHandler simpleAction;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < connectingObjcts.Length; i++)
        {
            energyEvent += connectingObjcts[i].OnChangeEnergyHandler;
        }

        ChangeEnergy();
    }

    public void FinalWork(float time)
    {
        Invoke("Action", time);
    }

    public void ChangeEnergy()
    {
        if(energyEvent != null)
        {
            energyEvent.Invoke(energyStatus);
        }
        anim.SetActiveForAll(energyStatus);
        if (energyStatus)
        {
            simpleAction = Break; ;
        }
        else
        {
            simpleAction = Repair;
        }
    }

    private void Action()
    {
        simpleAction();
    }

    public void Repair()
    {
        energyStatus = true;
        ChangeEnergy();
    }
    public void Break()
    {
        energyStatus = false;
        ChangeEnergy();
    }
}
