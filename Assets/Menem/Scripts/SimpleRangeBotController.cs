using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleRangeBotController : MonoBehaviour
{
    public Transform Target;
    
    public float Distance;
    [Range(0,50)]
    public float RangePursuit;
    [Range(0,50)]
    public float RangeShoot;
    
    public NavMeshAgent NavAgent;

    public GameObject NavObject;
    public GameObject ShockTurret;
    
    void Start()
    {
        NavAgent = NavObject.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Target != null)
        {
            Distance = Vector3.Distance(transform.position, Target.position);
            
            if (Distance< RangePursuit && Distance > RangeShoot)
            {
            
                NavAgent.destination = Target.position;
                ShockTurret.GetComponent<TurretScriptController>().target = Target;

            }
            else if (Distance <= RangeShoot)
            {
                NavAgent.destination = transform.position;
                ShockTurret.GetComponent<TurretScriptController>().target = Target;
                
            }
            else
            {
                NavAgent.destination = transform.position;
                Debug.Log("wat");
                ShockTurret.GetComponent<TurretScriptController>().target = null;
            }
        } 
    }
  
}
