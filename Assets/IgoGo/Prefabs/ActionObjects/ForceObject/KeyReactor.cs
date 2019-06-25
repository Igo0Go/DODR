using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyReactor : UsingOrigin
{

    public bool once;
    public bool enterOnly;
   
    
    [HideInInspector]
    public Collider colliderReact;
    [HideInInspector]
    public List<ForceObject> forceObjects;

    private int status;

    // Start is called before the first frame update
    void Start()
    {
        colliderReact = GetComponent<Collider>();
    }
    public void AddForceObj(ForceObject force)
    {
        if(forceObjects == null)
        {
            forceObjects = new List<ForceObject>();
        }
        forceObjects.Add(force);
    }
    public override void Use()
    {
        foreach (var item in actionObjects)
        {
            item.Use();
        }
        if(once)
        {
            Destroy(this.gameObject);
        }
    }
    
    private void ClearAllForceKeys()
    {
        foreach (var item in forceObjects)
        {
            item.IsKey = false;
            item.keyReactor = null;
        }
        forceObjects.Clear();
    }
}
