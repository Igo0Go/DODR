using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UsingObject: MyTools
{
    public abstract void Use();
}

public abstract class UsingOrigin : UsingObject
{
    public UsingObject[] actionObjects;
    public bool debug;

    private void OnDrawGizmos()
    {
        if(debug)
        {
            foreach (var c in actionObjects)
            {
                if (c is ObjectTranslateManager)
                {
                    Gizmos.color = Color.blue;
                }
                else if (c is AnimActivator)
                {
                    Gizmos.color = Color.green;
                }
                else if (c is ActionObject)
                {
                    Gizmos.color = Color.yellow;
                }
                Gizmos.DrawLine(transform.position, c.transform.position);
            }
        }
    }
}

public class ActionObject : UsingOrigin {

    [Space(20)]
    public Text tip;
    [SerializeField]
    [Tooltip("Не обязательный. Показывает значение энергии. У аниматора должен быть параметр bool ConnectEnergy")]
    public Animator energyMarker;
    public bool ConnectEnergy
    {
        get { return _connectEnergy; }
        set
        {
            if(energyMarker != null)
            {
                energyMarker.SetBool("ConnectEnergy", value);
            }
            _connectEnergy = value;
        }
    }

    [Space(10)]
    public bool startConnectEnergy;

    private bool _connectEnergy;
    private bool active;
    private string tipText;

    private void Start()
    {
        ConnectEnergy = startConnectEnergy;
        active = true;
    }

    public void OnChangeEnergyHandler(bool value)
    {
        if(tipText == null)
        {
            tipText = tip.text;
        }
        ConnectEnergy = value;
        if (ConnectEnergy)
        {
            tip.text = tipText;
        }
        else
        {
            tip.text = "Нет энергии";
        }
    }

    public override void Use()
    {
        if(active && ConnectEnergy)
        {
            UseAll();
            active = false;
            Invoke("ResetActive", 1f);
        }
    }
    public void UseAll()
    {
        foreach(var c in actionObjects)
        {
            c.Use();
        }
    }

    private void ResetActive()
    {
        active = true;
    }

}
