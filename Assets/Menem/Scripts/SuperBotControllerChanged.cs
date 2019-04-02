using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Stay,
    Attack,
    Walk,
    Fight,
    WaitAttack
}

public class SuperBotControllerChanged : MonoBehaviour
{


    public Transform Target;

    public Vector3 LastPosition;
    public Vector3 StandardPosition;
    public AutoWeapon gun;
    
    public float Distance; //дистанция до цели
    [Range(0, 200)] public float HP;
    [Range(0, 50)] public float RangePursuit; // радиус преследования/обнаружения
    [Range(0, 50)] public float RangeShoot; //радиус стрельбы
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
    public bool alive;

    public Version Version; //енум

    public EnemyState State;

    public Animator Anim;

    public GameObject Geosphere; // rename to EyeGeosphere

    
    void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        LastPosition = new Vector3(1000f, 1000f, 1000f);
        StandardPosition = LastPosition;
        wait = false;
    }

    void Update()
    {
        if (Target != null)
        {
            DistanceTP = Vector3.Distance(transform.position, Target.position);
        }
    }

    private void Attack() // кооод
    {
        if (IsMeleeAttack || Distance <= RangeMeleeAttack)
        {
            
            if (Anim.GetBool("Shoot"))
            {
                Anim.SetBool("Shoot",false);
            }
            NavAgent.destination = transform.position;
            Anim.SetTrigger("Fight");
            Anim.SetFloat("Xstate",0);
            Anim.SetFloat("Ystate",-1);
            //State = EnemyState.Fight;
        }
        else
        {
            NavAgent.destination = transform.position;
           Anim.SetBool("Shoot", true); 
            
            
        }

    }

    public void Shoot()
    {
        gun.shootDelegate();
    }

    public virtual EnemyState StateSolver
    {
        get { return State; }

        set
        {
            State = value;
            switch (StateSolver)
            {
                case EnemyState.Stay:
                    if (!wait)
                    {
                        wait = true;
                        CaseMethod(false, UnityEngine.Random.Range(-1, 1.1f), -1,  Target.transform.position);
                        StartCoroutine(GetRandomStayState());
                    }

                    break;

                case EnemyState.Walk:
                    //PlayThisClip(audioSource, Moves[1]);
                    CaseMethod(true, 0, 1, Target.transform.position);
                    break;

                case EnemyState.Attack:
                    //GetAttackDistance();
                    //PlayThisClip(audioSource, Moves[0]);
                     // подкорректить 
                    break;
                case EnemyState.Fight:
                    Debug.Log("1234");
                    Anim.SetTrigger("Fight");
                    break;
                case EnemyState.WaitAttack:
                    break;
                    
            }
        }
    }

    protected void CaseMethod(bool navAgentEnebled, float xstate, float ysate,  Vector3 destenation) //сделать что то сдесь как либо
    {
        if (navAgentEnebled)
        {
            NavAgent.destination = Target.position;
            
        }
        else
        {
            NavAgent.destination = transform.position;
        }
        
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

    public virtual float DistanceTP
    {
        get { return Distance; }

        set
        {
            Distance = value;
            Quaternion look = Quaternion.LookRotation(Target.transform.position - transform.position);
            float anglevis = Quaternion.Angle(transform.rotation, look); //угол между ботом и персом
            Vector3 VectorCam = MainCamera.transform.position - transform.position;
            anglecam = Vector3.Angle(MainCamera.transform.forward, -VectorCam);
            RaycastHit hit;
            Ray ray = new Ray(Geosphere.transform.position, (Target.transform.position + Vector3.up) - Geosphere.transform.position);

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
                    Physics.Raycast(ray, out hit, RangePursuit) &&
                    hit.transform.gameObject == Target.gameObject) // если он видит перса но не в дистанции атаки
                {
                    if (anglecam < Camshooting)
                    {
                        IsMeleeAttack = false;
                    }

                    if (Anim.GetBool("Shoot"))
                    {
                        Anim.SetBool("Shoot",false);
                    }
                    NavAgent.destination = Target.transform.position;
                    StateSolver = EnemyState.Walk;
                    LastPosition = Target.position;
                }
                else if (Distance <= RangeAttack && anglevis < AngleVision &&
                         Physics.Raycast(ray, out hit, RangePursuit) &&
                         hit.transform.gameObject == Target.gameObject) // если в ренже атаки
                {
                    
                    if (!IsMeleeAttack && anglecam > AngleCamera) // переключатель
                    {
                        IsMeleeAttack = true;

                    }
                    else // код атаки
                    {
                        //подготовка к стрельбе 
                        // стрельба = карутин с ключом
                        LastPosition = Target.position;
                        NavAgent.destination = transform.position;
                        Attack(); // делать код к атаке полностью в этом методе

                    }

                }
                else if (LastPosition != StandardPosition)
                {
                    if (Anim.GetBool("Shoot"))
                    {
                        Anim.SetBool("Shoot",false);
                    }
                    NavAgent.destination = LastPosition;
                }
                else
                {
                    if (Anim.GetBool("Shoot"))
                    {
                        Anim.SetBool("Shoot",false);
                    }
                    NavAgent.destination = transform.position;
                    //стоп
                    StateSolver = EnemyState.Stay;
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
                if (Distance < RangePursuit && Distance > RangeShoot && anglevis < AngleVision &&
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

    #region Health Damage and Death

    public virtual void GetDamage(float value)
    {
        if (alive)
        {
            //Alarm = true; надо подумать как интегрировать получение урона и стоит ли вообще
            NavAgent.enabled = false;
            Health -= value;
            Anim.SetInteger("Damage", (int)value);
            Anim.SetTrigger("GetDamage");
        }
    }
  
    public virtual void Death()
    {
        alive = false;
        NavAgent.enabled = false;
        Anim.SetTrigger("Dead");
    }
    
    public virtual float Health
    {
        get { return HP; }

        set
        {
            if (value <= 0 && alive)
            {
                Death();
                alive = false;
                HP = 0;
            }

            HP = value;
        }
    }
    
    #endregion 


    protected virtual void OnTriggerEnter(Collider other)
    {
    }
}
