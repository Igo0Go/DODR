using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBActivator : UsingObject
{
    public List<Rigidbody> rigidbodies;
    public bool debug;

    public override void Use()
    {
        foreach (var item in rigidbodies)
        {
            if(item != null)
            {
                item.isKinematic = false;
                item.useGravity = true;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(debug)
        {
            Gizmos.color = Color.yellow;
            foreach (var item in rigidbodies)
            {
                if (item != null)
                {
                    Gizmos.DrawLine(transform.position, item.transform.position);
                }
            }
        }
    }
}
