using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MyTools
{
    public WeaponConfig weaponConfig;
}



public class ShootWeapon : Weapon {

    public Transform shootPoint;
    [HideInInspector] public Transform targetLook;
    public Transform leftHandTarget;
    public ParticleSystem muzzleFlash;
    public AudioClip shootClip;
    public bool removeDestruct;
    [HideInInspector] public GameObject cameraMain;
    public LayerMask ignoreMask;
    public GameObject bullet;
    [Range(1,100)] public float thermalStep;
    [Range(0, 2)] public float thermalFrize;
    [Range(0, 7)] public float thermalTime;
    [Range(0, 100)] public float coolingsSpeed;


    [Space(10)]
    [Tooltip("Отсечка между выстрелами (только для автоматического)")] public float delayTime;
    [Tooltip("Камера для спецприцела (только для снайперского)")] public Camera sniperCam;

    [HideInInspector] public SimpleHandler shootDelegate;
    [HideInInspector] public Slider ammoSlider;
    public bool force;
    public Transform forceBufer;
    [Range(1,150)]
    public int countOfControls;
    [Space(10)]
    public bool shotgun;
    [Range(1,10)]
    public int bulletCount;
    [Range(0, 1)]
    public float angel;
    

    private LineRenderer lineRenderer;
    private AudioSource aud;
    private bool delay;

    private int thermDelay;
    private bool thermalError;
    private float ThermalValue
    {
        get
        {
            return ammoSlider.value;
        }
        set
        {
            ammoSlider.value = value;
        }
    }
    private float counter;

    private Vector3 origin;
    private Vector3 dir;
    private List<ForceObject> forceObjects;

    private void Start()
    {
        aud = GetComponent<AudioSource>();
        forceObjects = new List<ForceObject>();
        switch(weaponConfig.type)
        {
            case WeaponType.OneShoot:
                shootDelegate = OneShoot;
                break;
            case WeaponType.Automatic:
                shootDelegate = AutoShoot;
                break;
            case WeaponType.Ray:
                shootDelegate = RayShoot;
                lineRenderer = GetComponent<LineRenderer>();
                lineRenderer.positionCount = 2;
                aud.loop = true;
                aud.playOnAwake = true;
                aud.enabled = false;
                break;
        }
    }
    private void Update()
    {
        shootPoint.LookAt(targetLook);
        origin = shootPoint.position;
        dir = targetLook.position;
        Therm();

        //?
        Debug.DrawLine(origin, dir, Color.red);
        Debug.DrawLine(cameraMain.transform.position, dir, Color.red);
        //?
    }

    public void OneShoot()
    {
        if(!thermalError)
        {
            InstanceBullet();
            aud.PlayOneShot(shootClip);
            muzzleFlash.Play();
        }
    }
    public void AutoShoot()
    {
        if(!thermalError)
        {
            if (!delay)
            {
                InstanceBullet();
                aud.PlayOneShot(shootClip);
                muzzleFlash.Play();
                delay = true;
                Invoke("FinalDelay", delayTime);
            }
        }
    }
    public void RayShoot()
    {
        if(!thermalError)
        {
            aud.enabled = true;

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, shootPoint.position);
            lineRenderer.SetPosition(1, targetLook.position);
            if (removeDestruct)
            {
                RaycastHit hit;
                if (Physics.Raycast(shootPoint.position, shootPoint.forward, out hit, (Vector3.Distance(shootPoint.position, targetLook.position) + 3)))
                {
                    if (hit.collider.tag.Equals("Destructive"))
                    {
                        Destroy(hit.collider.gameObject);
                    }
                }
            }

            if (!delay)
            {
                InstanceBullet();
                if(aud.enabled)
                {
                    aud.PlayOneShot(shootClip);
                }
                muzzleFlash.Play();
                delay = true;
                Invoke("FinalDelay", delayTime);
            }

            if(force)
            {
                RaycastHit hit;
                Vector3 dir = targetLook.position - shootPoint.position;
                if(Physics.Raycast(shootPoint.position, dir, out hit, 200, ~ignoreMask))
                {
                    if(hit.collider.tag.Equals("ForceObj"))
                    {
                        GameObject bufer = hit.collider.gameObject;
                        ForceObject forceObject;
                        if(MyGetComponent<ForceObject>(bufer, out forceObject))
                        {
                            if(forceObjects.Count < countOfControls && !forceObjects.Contains(forceObject))
                            {
                                forceBufer.position = bufer.transform.position;
                                forceObjects.Add(forceObject);
                            }
                        }
                        else
                        {
                            Vector3 forceDir = hit.point - transform.position;
                            Rigidbody rb = bufer.GetComponent<Rigidbody>();
                            float mult = 10 / (Vector3.Distance(hit.point, transform.position) * rb.mass);
                            rb.AddForce(forceDir.normalized * mult, ForceMode.Impulse);
                        }
                    }
                    if(hit.collider.tag.Equals("Enemy"))
                    {
                        forceBufer.position = hit.point;
                    }
                }

                foreach (var item in forceObjects)
                {
                    item.contactMethod(forceBufer);
                }
            }
        }
    }
    public void ReturnRay()
    {
        lineRenderer.enabled = false;
        aud.enabled = false;
        foreach (var item in forceObjects)
        {
            item.transform.parent = null;
        }
        forceObjects.Clear();
    }
    private void FinalDelay()
    {
        delay = false;
    }

    private void InstanceBullet()
    {
        if(shotgun)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                GameObject progectile = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
                Bullet bulletConfig = progectile.GetComponent<Bullet>();
                bulletConfig.damage = weaponConfig.damage;
                bulletConfig.ignoreMask = ignoreMask;

                Vector3 dir = progectile.transform.forward +
                    Vector3.up * Random.Range(-angel, angel) + Vector3.right * Random.Range(-angel, angel);

                progectile.transform.forward = dir;
                Destroy(progectile, bulletConfig.lifeTime);
            }
        }
        else
        {
            GameObject progectile = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
            Bullet bulletConfig = progectile.GetComponent<Bullet>();
            bulletConfig.damage = weaponConfig.damage;
            bulletConfig.ignoreMask = ignoreMask;
            Destroy(progectile, bulletConfig.lifeTime);
        }
        ThermalValue += thermalStep;
        thermDelay = 1;
        
        if(ThermalValue == 100)
        {
            thermDelay = 2;
            thermalError = true;
            if(weaponConfig.type == WeaponType.Ray)
            {
                ReturnRay();
            }
        }
        counter = 0;
    }
    private void Therm()
    {
        if(thermDelay == 1)
        {
            if(counter < thermalFrize)
            {
                counter += Time.deltaTime;
            }
            else
            {
                thermDelay = 0;
            }
        }
        else if(thermDelay == 2)
        {
            if (counter < thermalTime)
            {
                counter += Time.deltaTime;
            }
            else
            {
                thermDelay = 0;
            }
        }
        else
        {
            if(ThermalValue > 0)
            {
                ThermalValue -= coolingsSpeed * Time.deltaTime;
            }
            else
            {
                thermalError = false;
            }
        }
    }
}
