using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChanger : UsingObject
{
    public int musicNumber;
    public bool destroyed;
    public MusicManager manager;

    public override void Use()
    {
        manager.CurrentBox = musicNumber;
        if(destroyed)
        {
            Destroy(gameObject);
        }
    }
}
