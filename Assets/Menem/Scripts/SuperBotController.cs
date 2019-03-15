using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering;

public enum Version
{
    Standart,
    Meele,
    Range
}

public class SuperBotController : MonoBehaviour
{
    public Transform Target;
    public Vector3 LastPosition;
    public Vector3 StandardPosition;
    public float Distance;
    [Range(0,50)]
    public float RangePursuit;
    [Range(0,50)]
    public float RangeShoot;
    public float RangeMeleeAttack;
    public float RangeAttack;
    public float AngleVision;
    public float AngleCamera;
    public float anglecam;
    public float Camshooting;
    public NavMeshAgent NavAgent;

    public GameObject NavObject;
    public GameObject ShockTurret;
    
    public Camera MainCamera;

    public bool IsMeleeAttack;
    

    public Version Version;
    
    void Start()
    {
        NavAgent = NavObject.GetComponent<NavMeshAgent>();
        LastPosition = new Vector3(1000f, 1000f, 1000f);
        StandardPosition = LastPosition;
    }
    //реагирование на действия в зоне
    
    void Update()
    {
        if (Target != null)
        {
            
            Distance = Vector3.Distance(transform.position, Target.position);
            Quaternion look = Quaternion.LookRotation(Target.transform.position - transform.position);
            float anglevis = Quaternion.Angle(transform.rotation, look);
            Vector3 VectorCam = MainCamera.transform.position - transform.position;
            anglecam = Vector3.Angle(MainCamera.transform.forward, -VectorCam);
            RaycastHit hit;
            Ray ray =new Ray(transform.position+Vector3.up, Target.transform.position - transform.position);

            if (IsMeleeAttack)
            {
                RangeAttack = RangeMeleeAttack;
            }
            else
            {
                RangeAttack = RangeShoot;
            }

            if (Version == Version.Standart)
            {

                if (Distance < RangePursuit && Distance > RangeAttack && anglevis < AngleVision &&
                    Physics.Raycast(ray, out hit, RangePursuit) && hit.transform.gameObject == Target)
                {
                    if (anglecam < Camshooting)
                    {
                        IsMeleeAttack = false;
                    }

                    NavAgent.destination = Target.position;
                    LastPosition = Target.position;
                    ShockTurret.GetComponent<TurretScriptController>().target = Target;
                }
                else if (Distance <= RangeAttack && anglevis < AngleVision &&
                         Physics.Raycast(ray, out hit, RangePursuit) &&
                         hit.transform.gameObject == Target)
                {
                    if (!IsMeleeAttack && anglecam > AngleCamera)
                    {
                        IsMeleeAttack = true;

                    }
                    else //
                    {
                        LastPosition = Target.position;
                        NavAgent.destination = transform.position;
                        ShockTurret.GetComponent<TurretScriptController>().target = Target;
                        Attack();
                        
                    }

                }
                else if (LastPosition != StandardPosition)
                {
                    NavAgent.destination = LastPosition;
                    ShockTurret.GetComponent<SuperTurretScriptController>().target = null;
                }
                else
                {
                    NavAgent.destination = transform.position;
                    ShockTurret.GetComponent<SuperTurretScriptController>().target = null;
                }
            }
            else if (Version == Version.Meele)
            {
                IsMeleeAttack = true;
                if (Distance < RangePursuit && Distance > RangeMeleeAttack && anglevis < AngleVision &&
                                        Physics.Raycast(ray, out hit, RangePursuit) && hit.transform.gameObject == Target)
                {
                    NavAgent.destination = Target.position;
                    LastPosition = Target.position;
                    ShockTurret.GetComponent<TurretScriptController>().target = Target;
                }
                else if (Distance <= RangeMeleeAttack && anglevis < AngleVision &&
                         Physics.Raycast(ray, out hit, RangePursuit))
                {
                    IsMeleeAttack = true;
                    Attack();
                }
                else if (LastPosition != StandardPosition)
                {
                    NavAgent.destination = LastPosition;
                    ShockTurret.GetComponent<SuperTurretScriptController>().target = null;
                }
                else
                {
                    NavAgent.destination = transform.position;
                    ShockTurret.GetComponent<SuperTurretScriptController>().target = null;
                }
                
            }
            else
            {
                IsMeleeAttack = false;
                if (Distance< RangePursuit && Distance > RangeShoot && anglevis < AngleVision &&
                    Physics.Raycast(ray, out hit, RangePursuit) && hit.transform.gameObject == Target)
                {
            
                    NavAgent.destination = Target.position;
                    ShockTurret.GetComponent<TurretScriptController>().target = Target;
                    LastPosition = Target.position;

                }
                else if (Distance <= RangeShoot && anglevis < AngleVision)
                {
                    NavAgent.destination = transform.position;
                    ShockTurret.GetComponent<TurretScriptController>().target = Target;
                    LastPosition = Target.position;
                    Attack();
                }
                else if (LastPosition != StandardPosition)
                {
                    NavAgent.destination = LastPosition;
                    ShockTurret.GetComponent<SuperTurretScriptController>().target = null;
                }
                else
                {
                    NavAgent.destination = transform.position;
                    ShockTurret.GetComponent<SuperTurretScriptController>().target = null;
                }
            }
        } 
        
       
    }

    private void Attack()
    {
        if (IsMeleeAttack || Distance<= RangeMeleeAttack)
        {
            
        }

    }
}
