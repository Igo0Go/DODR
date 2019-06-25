using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TransformOriginHandler (Transform origin);

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class ForceObject : MyTools
{
    public Transform FolderInWorldSpace;
    [Range(0.1f,1)]
    public float contactTime;
    [Range(1, 180)]
    public float thresholdAgle;
    [Space(20)]
    public bool rotate;
    public Vector3 rotVector;
    [Space(20)]
    public bool IsKey;
    public KeyReactor keyReactor;
    [Range(0.1f, 10)]
    public float controlDistance;

    [HideInInspector]
    public TransformOriginHandler contactMethod;

    private bool contact;
    private float timer;
    private Rigidbody rb;
    private Animator anim;
    private Transform point;
    private SimpleHandler updateMethod;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        if(IsKey)
        {
            keyReactor.AddForceObj(this);
        }
        contactMethod = Contact;
        updateMethod = DeltaContact;
    }
    private void Update()
    {
        updateMethod();
    }

    private void Contact(Transform point)
    {
        contact = true;
        timer = contactTime;
        //transform.parent = point;
        rb.useGravity = false;
        //rb.isKinematic = true;
        this.point = point;
    }
    private void DeltaContact()
    {
        if(contact)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;

                Vector3 dir = point.position - transform.position;

                if(Vector3.Angle(rb.velocity, dir) > thresholdAgle)
                {
                    rb.velocity = Vector3.zero;
                }

                if (dir.magnitude < controlDistance)
                {
                    rb.velocity = dir;
                }
                else
                { 
                    rb.AddForce(dir * Time.deltaTime / (rb.mass), ForceMode.Impulse);
                }
                
                if (rotate)
                {
                    transform.rotation *= Quaternion.Euler(rotVector);
                }
            }
            else
            {
                contact = false;
                rb.useGravity = true;
                transform.parent = FolderInWorldSpace;
                rb.isKinematic = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(IsKey)
        {
            if (other == keyReactor.colliderReact)
            {
                keyReactor.Use();
            }
        }
        if(other.tag.Equals("ForceReactor"))
        {
            ForceReactor forceReactor;
            if(MyGetComponent(other.gameObject, out forceReactor))
            {
                forceReactor.SetForce();
            }
        }

        if(other.tag.Equals("Enemy") && contact)
        {
            IAlive alive;
            if (MyGetComponent<IAlive>(other.gameObject, out alive))
            {
                alive.GetDamage((int)Mathf.Round(rb.velocity.magnitude * rb.mass));
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (IsKey)
        {
            if (other == keyReactor.colliderReact)
            {
                if(!keyReactor.enterOnly)
                {
                    keyReactor.Use();
                }
            }
        }
    }
}
