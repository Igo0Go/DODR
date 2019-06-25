using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Animator))]
public class ForceReactor : UsingOrigin
{
    [Space(20)]
    [Range(1, 100)]
    public int useForce;
    [Range(0, 120)]
    public float returnTime;
    private int Force
    {
        get
        {
            return currentForce;
        }
        set
        {
            currentForce = value;
            anim.SetFloat("Force", currentForce / useForce);
        }
    }

    private Animator anim;
    private int currentForce;
    private bool acitve;

    void Start()
    {
        anim = GetComponent < Animator>();
    }

    public void SetForce()
    {
        if(!acitve)
        {
            Force++;
            if(Force == useForce)
            {
                Use();
                acitve = true;
                Invoke("ReturnAction", returnTime);
            }
        }
    }

    private void ReturnAction()
    {
        Force = 0;
        acitve = false;
        Use();
    }

    public override void Use()
    {
        foreach (var item in actionObjects)
        {
            item.Use();
        }
    }
}
