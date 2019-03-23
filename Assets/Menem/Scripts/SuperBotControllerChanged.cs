using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Stay,
    Attack,
    Walk,
    Fight
}
public class SuperBotControllerChanged : MonoBehaviour
{
    
    
    public Transform Target;
    
    public Vector3 LastPosition;
    public Vector3 StandardPosition;
    
    public float Distance; //дистанция до цели
    [Range(0,50)]
    public float RangePursuit; // радиус преследования/обнаружения
    [Range(0,50)]
    public float RangeShoot; //радиус стрельбы
    public float RangeMeleeAttack; // радиус мили атаки
    public float RangeAttack; // радиус атаки в момент времени
    public float AngleVision; // угол обзора бота
    public float AngleCamera; // угол обзора камеры
    public float anglecam; // угол между фронтом камеры и ботом
    public float Camshooting; // угол при котором можно сказать что игрок целится в нас
    
    public NavMeshAgent NavAgent;
    
    public Camera MainCamera;

    public bool IsMeleeAttack;
    public bool wait;
    
    public Version Version; //енум
    
    public EnemyState State;

    public Animator Anim;
    
    // Start is called before the first frame update
    void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        LastPosition = new Vector3(1000f, 1000f, 1000f);
        StandardPosition = LastPosition;
        wait = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Target != null)
        {
            
            Distance = Vector3.Distance(transform.position, Target.position);
            Quaternion look = Quaternion.LookRotation(Target.transform.position - transform.position);
            float anglevis = Quaternion.Angle(transform.rotation, look); //
            Vector3 VectorCam = MainCamera.transform.position - transform.position;
            anglecam = Vector3.Angle(MainCamera.transform.forward, -VectorCam);
            RaycastHit hit;
            Ray ray =new Ray(transform.position, Target.transform.position - transform.position);
            

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
                    Physics.Raycast(ray, out hit, RangePursuit) && hit.transform.gameObject == Target.gameObject ) // если он видит перса но не в дистанции атаки
                {
                    
                       
                            if (anglecam < Camshooting)
                            {
                                IsMeleeAttack = false;
                            }
                            // код для бега

                            StateSolver = EnemyState.Walk;
                            //ShockTurret.GetComponent<TurretScriptController>().target = Target;
                       
                    

                }
                else if (Distance <= RangeAttack && anglevis < AngleVision &&
                         Physics.Raycast(ray, out hit, RangePursuit) &&
                         hit.transform.gameObject == Target.gameObject)
                {
                    if (!IsMeleeAttack && anglecam > AngleCamera)
                    {
                        IsMeleeAttack = true;

                    }
                    else //
                    {
                        //подготовка к стрельбе 
                        // стрельба = карутин с ключом
                        LastPosition = Target.position;
                        NavAgent.destination = transform.position;
                        //ShockTurret.GetComponent<TurretScriptController>().target = Target;
                        Attack();
                        
                    }

                }
                else if (LastPosition != StandardPosition)
                {
                    NavAgent.destination = LastPosition;
                    //ShockTurret.GetComponent<SuperTurretScriptController>().target = null;
                }
                else
                {
                    NavAgent.destination = transform.position;
                    //стоп
                    StateSolver = EnemyState.Stay;
                    // ShockTurret.GetComponent<SuperTurretScriptController>().target = null;
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
                   // ShockTurret.GetComponent<TurretScriptController>().target = Target;
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
                   // ShockTurret.GetComponent<SuperTurretScriptController>().target = null;
                }
                else
                {
                    NavAgent.destination = transform.position;
                   //ShockTurret.GetComponent<SuperTurretScriptController>().target = null;
                }
                
            }
            else
            {
                IsMeleeAttack = false;
                if (Distance< RangePursuit && Distance > RangeShoot && anglevis < AngleVision &&
                    Physics.Raycast(ray, out hit, RangePursuit) && hit.transform.gameObject == Target)
                {
            
                    NavAgent.destination = Target.position;
                   // ShockTurret.GetComponent<TurretScriptController>().target = Target;
                    LastPosition = Target.position;

                }
                else if (Distance <= RangeShoot && anglevis < AngleVision)
                {
                    NavAgent.destination = transform.position;
                   // ShockTurret.GetComponent<TurretScriptController>().target = Target;
                    LastPosition = Target.position;
                    Attack();
                }
                else if (LastPosition != StandardPosition)
                {
                    NavAgent.destination = LastPosition;
                   // ShockTurret.GetComponent<SuperTurretScriptController>().target = null;
                }
                else
                {
                    NavAgent.destination = transform.position;
                   // ShockTurret.GetComponent<SuperTurretScriptController>().target = null;
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
    
   
        public virtual EnemyState StateSolver
        {
            get
            {
                return State;
            }

            set
            {
                State = value;
                switch (StateSolver)
                {
                    case EnemyState.Stay:
                        if (!wait)
                        {
                            wait = true;  
                            CaseMethod(false, UnityEngine.Random.Range(-1, 1.1f), -1, 0, Target.transform.position);
                            StartCoroutine(GetRandomStayState());
                        }
                        break;

                    case EnemyState.Walk:
                        //PlayThisClip(audioSource, Moves[1]);
                        Debug.Log("begiii");
                        CaseMethod(true, 0, 1, 0, Target.transform.position);
                        break;

                    case EnemyState.Attack:
                        //GetAttackDistance();
                        //PlayThisClip(audioSource, Moves[0]);
                        CaseMethod(false, 0, 0, 1/*attackType*/, Target.transform.position);
                        break;
                }
            }
        }
    protected void CaseMethod(bool navAgentEnebled, float xstate, float ysate, int attack, Vector3 destenation)
    {
        if (navAgentEnebled)
        {
            NavAgent.destination = Target.position;
            Debug.Log("begiiii");
        }
        else
        {
            NavAgent.destination = transform.position;
        }

        Anim.SetInteger("Attack", attack);
        Anim.SetFloat("Xstate", xstate);
        Anim.SetFloat("Ystate", ysate);
    }
    protected IEnumerator GetRandomStayState()
    {
        Anim.SetFloat("Ystate", -1);
        Anim.SetFloat("Xstate", UnityEngine.Random.Range(-1, 1.1f));
        //FindPlayers();
        yield return new WaitForSeconds(2);
        wait = false;
    }
}
