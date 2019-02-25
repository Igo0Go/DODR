using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponReactor : UsingOrigin, IAlive
{
    [Range(0,300)]
    public float healt;
    [Range(0, 10)]
    public float removeTime;
    public ParticleSystem particle;
    public GameObject ofterDeadPrefab;
    public GameObject model;
    [Space(10)]
    public bool shock;


    public float Health
    {
        get
        {
            return healt;
        }

        set
        {
            healt = value;
            if (healt <= 0)
            {
                Dead();
            }
        }
    }

    public void Dead()
    {
        Use();
    }

    private void UseAl()
    {
        foreach(var c in actionObjects)
        {
            c.Use();
        }
    }
    private void Remove()
    {
        if(ofterDeadPrefab != null)
        {
            Instantiate(ofterDeadPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    public void GetDamage(int damage)
    {
        if(!shock)
        {
            if (Health > 0)
            {
                Health -= damage;
            }
        }
    }
    public override void Use()
    {
        Destroy(model);
        if (particle != null)
        {
            particle.Play();
        }
        UseAl();
        Invoke("Remove", removeTime);
    }
    public void ShockEffect(float time)
    {
        foreach(var c in actionObjects)
        {
            if(c is ActionObject)
            {
                ActionObject obj = ((ActionObject)c);
                obj.Use();
                obj.startConnectEnergy = obj.ConnectEnergy;
                obj.OnChangeEnergyHandler(false);
            }
        }
        Invoke("ReturnEnergyStatus", time);
    }
    private void ReturnEnergyStatus()
    {
        foreach (var c in actionObjects)
        {
            if (c is ActionObject)
            {
                ActionObject obj = ((ActionObject)c);
                obj.Use();
                obj.OnChangeEnergyHandler(obj.startConnectEnergy);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Sword"))
        {
            GetDamage(30);
        }
    }
}
