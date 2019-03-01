using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationReactor : UsingOrigin
{

    public bool enterOnly;
    public bool once;

    public override void Use()
    {
        foreach(var c in actionObjects)
        {
            c.Use();
        }
        if(once)
        {
            Destroy(gameObject);
        }
    }
}
