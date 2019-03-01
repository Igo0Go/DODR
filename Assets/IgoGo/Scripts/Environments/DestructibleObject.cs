using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : UsingOrigin {

    public Rigidbody partRb;
    public List<GameObject> destructibleObject;

    [Range(0,100)]public int criticalCount;
    private Vector3 newScale;

    

    private void Start()
    {
        if(partRb != null)
        {
            newScale = partRb.transform.localScale * 0.9f;
            partRb.useGravity = false;
            partRb.isKinematic = true;
        }
    }
    private void Update()
    {
        if(destructibleObject.Count > 0)
        {
            CheckList();
        }
    }
    private void CheckList()
    {
        for (int i = 0; i < destructibleObject.Count; i++)
        {
            if (destructibleObject[i] == null)
            {
                destructibleObject.Remove(destructibleObject[i]);
                i--;
            }
        }

        if (destructibleObject.Count <= criticalCount)
        {
            if (partRb != null)
            {
                partRb.useGravity = true;
                partRb.isKinematic = false;
                partRb.transform.localScale = newScale;
                partRb.AddForce(partRb.transform.forward.normalized * 2f, ForceMode.Impulse);
                Destroy(partRb.gameObject, 5);
            }
            Use();
        }
    }

    public override void Use()
    {
        foreach (var c in actionObjects)
        {
            c.Use();
        }
    }

    private void OnDrawGizmos()
    {
        if(debug)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, partRb.position);
        }
    }
}
