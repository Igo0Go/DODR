using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimActivator : UsingObject {

    [Tooltip("Аниматор должен содержать параметр Active (bool)")]
    public Animator[] animObjects;

    public bool triggerReactor;
    public bool active;

    private void Start()
    {
        SetActiveForAll(active);
    }

    public override void Use()
    {
        active = !active;
        SetActiveForAll(active);
    }

    public void SetActiveForAll(bool value)
    {
        foreach(var c in animObjects)
        {
            c.SetBool("Active", value);
        }
    }
}
