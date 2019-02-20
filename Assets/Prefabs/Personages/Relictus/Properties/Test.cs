using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public LayerMask mask;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward.normalized * 100, Color.red);
        if(Physics.Raycast(transform.position, transform.forward, out hit, 100, mask))
        {
            Debug.Log(hit.collider.gameObject.name);
        }
	}
}
