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
    public bool removeDestructive;  //может ли уничтожать препятствия


    [Tooltip("Толькодля лучевого")] public LineRenderer lineRenderer;
    [Tooltip("Только для автоматического и oneShoot")] public GameObject bullet;
    [Tooltip("Только для автоматического и oneShoot")] public Transform shootPoint;
    [Tooltip("Только для автоматического и oneShoot")] public ParticleSystem muzzleFlash;
    [Tooltip("Только для автоматического и oneShoot")] public AudioClip shootClip;
    [Tooltip("Только для автоматического и oneShoot")] public Animator anim;
    [Tooltip("Только для автоматического и oneShoot")] public bool recoil;
    

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
            case WeaponType.Automatic:
                shootDelegate = AutoShoot;
                aud.playOnAwake = false;
                aud.loop = false;
                aud.enabled = true;
                recoil = false;
                break;
        }
	}
	
	void Update () {

        if (active)
        {
            shootDelegate();
        }
        else if(type == WeaponType.Ray)
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
    public void AutoShoot()
    {
        if (!recoil)
        {
            InstanceBullet();
            aud.PlayOneShot(shootClip);
            muzzleFlash.Play();
            recoil = true;
            anim.SetTrigger("Recoil");
        }
    }


    private void InstanceBullet()
    {
        GameObject progectile = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        Bullet bulletConfig = progectile.GetComponent<Bullet>();
        bulletConfig.damage = damage;
        bulletConfig.ignoreMask = ignoreMask;
        Destroy(progectile, bulletConfig.lifeTime);
    }
}
