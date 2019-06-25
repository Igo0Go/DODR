using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : ActionObject {

    [Space(30)]
    [Header("Входные элементы")]
    public UsingOrigin[] actors;

    private int counter;

    private void Start()
    {
        counter = 0;
    }
    public override void Use()
    {
        counter++;
        if(counter == actors.Length)
        {
            UseAll();
        }
    }
    private void OnDrawGizmosSelected()
    {
        if(debug)
        {
            Gizmos.color = Color.red;
            foreach(var c in actors)
            {
                if(c != null)
                {
                    Gizmos.DrawLine(transform.position, c.transform.position);
                }
            }
        }
    }
}

