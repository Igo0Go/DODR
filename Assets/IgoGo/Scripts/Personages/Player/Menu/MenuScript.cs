using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour {

    [Range(0,1)] public float weight;
    public Camera cam;

    private Animator anim;
    private Ray ray;
    private RaycastHit hit;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetLookAtWeight(weight);
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 30))
        {
            anim.SetLookAtPosition(hit.point);
        }
    }
}
