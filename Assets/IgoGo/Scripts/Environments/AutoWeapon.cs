using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoWeapon : MyTools {

    public bool active;
    public int damage;
    public LayerMask ignoreMask; //какие слои должен игнорировать луч
    public WeaponType type;
    public SimpleHandler shootDelegate; //делегат для стрельбы
    public SimpleHandler returnDelegate; //делегат для возврата (прекратить стрельбу)
    public AudioSource aud;
    public LineRenderer lineRenderer;
    public bool removeDestructive;  //может ли уничтожать препятствия

	void Start () {
		switch(type)
        {
            case WeaponType.Ray:
                lineRenderer.positionCount = 2;
                shootDelegate = RayShoot;
                returnDelegate = RayReturn;
                aud.playOnAwake = true;
                aud.loop = true;
                aud.enabled = false;
                break;
        }
	}
	
	void Update () {
        if(active)
        {
            shootDelegate();
        }
        else
        {
            returnDelegate();
        }
    }

    private void RayShoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000, ignoreMask))
        {
            lineRenderer.enabled = true;
            aud.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hit.point);
            IAlive alive;
            if(MyGetComponent(hit.collider.gameObject, out alive))
            {
                alive.GetDamage(damage);
            }
            if (removeDestructive)
            {
                if (hit.collider.tag.Equals("Destructive"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
    private void RayReturn()
    {
        lineRenderer.enabled = true;
        aud.enabled = true;
    }



}
