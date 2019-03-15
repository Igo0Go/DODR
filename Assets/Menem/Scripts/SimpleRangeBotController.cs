using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleRangeBotController : MonoBehaviour
{
    public Transform Target;
    public Vector3 LastPosition;
    public Vector3 StandardPosition;
    public float Distance;
    [Range(0,50)]
    public float RangePursuit;
    [Range(0,50)]
    public float RangeShoot;
    public float AngleVision;
    
    public NavMeshAgent NavAgent;

    public GameObject NavObject;
    public GameObject ShockTurret;
    
    void Start()
    {
        NavAgent = NavObject.GetComponent<NavMeshAgent>();
        LastPosition = new Vector3(1000f, 1000f, 1000f);
        StandardPosition = LastPosition;
    }
    //угол зрения, реагирование на действия в зоне

    void Update()
    {
        if (Target != null)
        {
            Distance = Vector3.Distance(transform.position, Target.position);
            Quaternion look = Quaternion.LookRotation(Target.transform.position - transform.position);
            float angle = Quaternion.Angle(transform.rotation, look);
            RaycastHit hit;
            Ray ray =new Ray(transform.position+Vector3.up, Target.transform.position - transform.position);
            if (Distance< RangePursuit && Distance > RangeShoot && angle < AngleVision &&
                Physics.Raycast(ray, out hit, RangePursuit) && hit.transform.gameObject == Target)
            {
            
                NavAgent.destination = Target.position;
                ShockTurret.GetComponent<TurretScriptController>().target = Target;
                LastPosition = Target.position;

            }
            else if (Distance <= RangeShoot && angle < AngleVision &&
                     Physics.Raycast(ray, out hit, RangePursuit) && hit.transform.gameObject == Target)
            {
                NavAgent.destination = transform.position;
                ShockTurret.GetComponent<TurretScriptController>().target = Target;
                LastPosition = Target.position;
            }
            else
            {
                if (LastPosition != StandardPosition)
                {
                    
                    NavAgent.destination = LastPosition;
                }
                
                ShockTurret.GetComponent<TurretScriptController>().target = null;
            }
        } 
    }
  
}
