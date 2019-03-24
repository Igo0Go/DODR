using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimlineActivator : UsingObject
{
    public PlayableDirector[] timlines;
    public bool active;
    public bool destroyed;

    public override void Use()
    {
        active = !active;
        CheckActive();
        if(destroyed)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CheckActive();
    }

    private void CheckActive()
    {
        foreach (var c in timlines)
        {
            if (active)
            {
                c.Play();
            }
            else
            {
                c.Stop();
            }
        }
    }
}
