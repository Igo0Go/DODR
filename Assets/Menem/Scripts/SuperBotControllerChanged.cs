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

[RequireComponent(typeof(CharacterController))]
public class SuperBotControllerChanged : MonoBehaviour, IAlive
{
    public Transform Target;

    public Vector3 LastPosition;
    public Vector3 StandardPosition;
    
    public AutoWeapon gun;
    
    public float Distance; //дистанция до цели
    [Range(0, 50)] public float RangePursuit; // радиус преследования/обнаружения
    [Range(0, 50)] public float RangeShoot; //радиус стрельбы
    public float RangeMeleeAttack; // радиус мили атаки
    public float RangeAttack; // радиус атаки в момент времени
    public float AngleVision; // угол обзора бота
    public float AngleVisionAlert; // alert
    public float AngleVisionStandart; //standart
    public float AngleCamera; // угол обзора камеры
    public float anglecam; // угол между фронтом камеры и ботом
    public float Camshooting; // угол при котором можно сказать что игрок целится в нас
    public float AngleShhotingForBot;
   
    public NavMeshAgent NavAgent;

    public Camera MainCamera;

    public bool IsMeleeAttack;
    public bool wait;
    public bool alive;
    public bool Alert;

    public Version Version; //енум

    public EnemyState State;

    public Animator Anim;

    public GameObject Geosphere; // rename to EyeGeosphere


	[Range(0,300)]
    public int health;
	private CharacterController charController;
	public LayerMask ignoreMask;
    
	
	
	
    void Start()
    {
		alive = true;
        NavAgent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
        LastPosition = new Vector3(1000f, 1000f, 1000f);
        StandardPosition = LastPosition;
        wait = false;
        AngleVisionStandart = AngleVision;
        AngleVisionAlert = 360f;
		charController = GetComponent<CharacterController>();
    }

    void Update()
    {
		if(alive)
		{
			if (Target != null)
			{
				DistanceTP = Vector3.Distance(transform.position, Target.position);
			}	
		}
        
    }
    protected virtual IEnumerator Alarm()////почему он не ждет двух секунд и отключает сразу?
    {
        
        yield return new WaitForSeconds(2);
        Alert = false;

    }

    private void Attack() // кооод
    {
        Quaternion look = Quaternion.LookRotation(Target.transform.position - transform.position);
        float anglevis = Quaternion.Angle(transform.rotation, look); //угол между ботом и персом
        if (anglevis > AngleShhotingForBot)
        {
            Vector3 relativePos = Target.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos), Time.deltaTime * 5);
        }
        else
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
        

    }

	#region Обработчики событий анимаций
	
    public void Shoot()
    {
        gun.shootDelegate();
    }
	
	public void CorrecteShootPoint()
	{
		gun.shootPoint.LookAt(Target.position + Vector3.up);
	}

	#endregion

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
                    CaseMethod(true, 0, 1, Target.transform.position);
                    break;
                
                case EnemyState.Fight:
                    Debug.Log("1234");
                    Anim.SetTrigger("Fight");
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

            if (Alert)
            {
                AngleVision = AngleVisionAlert;
            }
            else
            {
                AngleVision = AngleVisionStandart;
            }
            
            if (Version == Version.Standart)
            {
                if (Distance < RangePursuit && Distance > RangeAttack && anglevis < AngleVision &&
                    Physics.Raycast(ray, out hit, RangePursuit, ignoreMask) &&
                    hit.transform.gameObject == Target.gameObject) // если он видит перса но не в дистанции атаки
                {
                    if (anglecam < Camshooting)
                    {
                        IsMeleeAttack = false;
                    }

                    if (Alert)
                    {
                        StartCoroutine(Alarm());
                    }

                    if (Anim.GetBool("Shoot"))
                    {
                        Anim.SetBool("Shoot",false);
                    }
                    // переключалка опасности
                    
                    NavAgent.destination = Target.transform.position;
                    StateSolver = EnemyState.Walk;
                    LastPosition = Target.position;
                }
                else if (Distance <= RangeAttack && anglevis < AngleVision &&
                         Physics.Raycast(ray, out hit, RangePursuit, ignoreMask) &&
                         hit.transform.gameObject == Target.gameObject) // если в ренже атаки
                {
                    
                    if (!IsMeleeAttack && anglecam > AngleCamera) // переключатель
                    {
                        IsMeleeAttack = true;

                    }
                    else // код атаки
                    {
                        LastPosition = Target.position;
                        NavAgent.destination = transform.position;
                        if (!Alert)
                        {
                            Alert = true;
                            //StartCoroutine(Alarm()); куда бы его переместить? 
                            
                        }
                        Attack(); // делать код к атаке полностью в этом методе

                    }

                }
                else if (LastPosition != StandardPosition )
                {
                    
                    if (Anim.GetBool("Shoot"))
                    {
                        Anim.SetBool("Shoot",false);
                    }
                    NavAgent.destination = LastPosition;
                    if (LastPosition.x == transform.position.x &&LastPosition.z == transform.position.z)
                    {
                        
                        LastPosition = StandardPosition;
                    }
                }
                else
                {
                    if (Anim.GetBool("Shoot"))
                    {
                        Anim.SetBool("Shoot",false);
                    }
                    if (!wait)
                    {
                        wait = true;
                        CaseMethod(false, UnityEngine.Random.Range(-1, 1.1f), -1,  Target.transform.position);
                        StartCoroutine(GetRandomStayState());
                    }
                    
                    NavAgent.destination = transform.position;
                    //стоп
                    //StateSolver = EnemyState.Stay;
                }
            }
            else if (Version == Version.Meele)
            {
                IsMeleeAttack = true;
                if (Distance < RangePursuit && Distance > RangeMeleeAttack && anglevis < AngleVision &&
                    Physics.Raycast(ray, out hit, RangePursuit, ignoreMask) && hit.transform.gameObject == Target)
                {
                    NavAgent.destination = Target.position;
                    LastPosition = Target.position;
                    // ShockTurret.GetComponent<TurretScriptController>().target = Target;
                }
                else if (Distance <= RangeMeleeAttack && anglevis < AngleVision &&
                         Physics.Raycast(ray, out hit, RangePursuit, ignoreMask))
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
                    Physics.Raycast(ray, out hit, RangePursuit, ignoreMask) && hit.transform.gameObject == Target)
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

    //public virtual void GetDamage(float value)
    //{
      //  if (alive)
        //{
            //Alarm = true; надо подумать как интегрировать получение урона и стоит ли вообще
          //  NavAgent.enabled = false;
            //Health -= value;
            //Anim.SetInteger("Damage", (int)value);
            //Anim.SetTrigger("GetDamage");
        //}
    //}
	#endregion
	
	#region IAlive
	
	public void GetDamage(int damage)
    {
		if(alive)
		{
			if (Health > 0)
            {
				NavAgent.destination = transform.position;
				Anim.SetTrigger("Damage");
                Health -= damage;
			}
		}
    }
  
	public void ShockEffect(float time)
    {
        
    }
	
	public void Dead()
    {
        alive = false;
        NavAgent.destination = transform.position;
		Anim.SetInteger("Health", health);
        Anim.SetTrigger("Damage");
    }
	
	public void DestroyModel()
	{
		Destroy(gameObject);
	}
  
	public float Health
    {
        get
        {
            return health;
        }

        set
        {
            health = (int)value;
            if (health <= 0)
            {
				alive = false;
                Dead();
            }
			else
			{
				Anim.SetInteger("Health", health);
			}
        }
    }
  
	#endregion
  
    //public virtual void Death()
    //{
      //  alive = false;
       // NavAgent.enabled = false;
        //Anim.SetTrigger("Dead");
    //}
    
    //public virtual float Health
    //{
      //  get { return HP; }

        //set
        //{
          //  if (value <= 0 && alive)
            //{
              //  Death();
                //alive = false;
                //HP = 0;
            //}

            //HP = value;
        //}
    //}
    


    protected virtual void OnTriggerEnter(Collider other)
    {
		if(other.tag.Equals("Sword"))
		{
			GetDamage(5);
		}			
    }
}
