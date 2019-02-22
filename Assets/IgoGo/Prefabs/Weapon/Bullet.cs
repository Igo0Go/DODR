using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MyTools {

    public int speed;   //скорость полёта снаряда
    public float lifeTime; //время, которое держится эффект от выстрела (следы или шок)
    public bool connectDecal; //становится ли декаль дочерним от поверхности объектом
    public bool shock; //шоковый ли снаряд

    public GameObject metalDecal;    //декаль для непробиваемых поверхностей 
    public GameObject unMetalDecal;  //декаль для обычных поверхностей

    [HideInInspector]
    public int damage;

    private GameObject spawnDecal;  //буфер для выбранного декаля
    private Vector3 lastPos;        //буфер для позиции снаряда из предыдущего кадра

	void Start () {
        lastPos = transform.position;
        Destroy(gameObject, 10);
	}
	
	void Update () {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        RaycastHit hit;

        if(Physics.Linecast(lastPos, transform.position, out hit))
        {
            PhysicMaterial pm;
            if(ContainsPhisicsMaterial(hit.collider, out pm))
            {
                string hitMaterial = hit.collider.sharedMaterial.name;
                if (pm.name.Equals("Metal"))
                {
                    SpawnEffect(hit, metalDecal);
                }
                else
                {
                    SpawnEffect(hit, unMetalDecal);
                }
            }
            else
            {
                SpawnEffect(hit, unMetalDecal);
            }
           

            WeaponReactor reactor;
            if (MyGetComponent(hit.collider.gameObject, out reactor))
            {
                if(shock && reactor.shock)
                {
                    reactor.ShockEffect(lifeTime);
                }
                else
                {
                    reactor.GetDamage(damage);
                }
            }
            Destroy(gameObject);
        }
        lastPos = transform.position;
	}

    private void SpawnEffect(RaycastHit hit, GameObject prefab)
    {
        spawnDecal = Instantiate(prefab, hit.point + hit.normal*0.01f, Quaternion.LookRotation(-hit.normal));
        if(connectDecal)
        {
            spawnDecal.transform.SetParent(hit.collider.transform);
        }
        Destroy(spawnDecal, lifeTime);
    }
}
