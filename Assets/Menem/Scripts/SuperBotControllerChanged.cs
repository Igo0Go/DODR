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
}

[RequireComponent(typeof(CharacterController))]
public class SuperBotControllerChanged : MonoBehaviour, IAlive
{
    public Transform Target; //цель

    public Vector3 LastPosition; //последняя позиция цели
    public Vector3 StandardPosition; //стандартная позиция для сравнивания с LastPosition

    public AutoWeapon gun;//внешний класс стрельбы. Стрельба бота адаптирована под этот класс

    public LayerMask ignoreMask;//маска позволяющая игнорировать некоторые препятствия в RaycastHit

    [Range(0, 300)] public int health;//значение Здоровья требуемое для метода Health и смежными

    public float Distance; //дистанция до цели
    [Range(0, 50)] public float RangePursuit; // радиус преследования/обнаружения
    [Range(0, 50)] public float RangeShoot; //радиус стрельбы
    public float RangeMeleeAttack; // радиус ближней атаки
    public float RangeAttack; // радиус атаки в момент времени, сделан для упрощения кода
    public float AngleVision; // угол обзора бота в  текущий момент времени, сделан для упрощения кода
    public float AngleVisionAlert; // угол обзора бота во время alert
    public float AngleVisionStandart; //угол обзора бота во время stay
    public float AngleCamera; // угол обзора камеры, т.е предельное значение anglecam
    public float anglecam; // угол между forward камеры и ботом, требуется для отслеживания игрока.
    public float Camshooting; // угол, при котором можно сказать о том, что игрок целится в бота
    public float AngleShhotingForBot;//угол, который является критерием для стрельбы бота, т.е. угол предельного прицела

    public NavMeshAgent NavAgent;//переменная для компонента NavMeshAgent

    public Camera MainCamera;//переменная для компонента камеры игрока

    public bool IsMeleeAttack;//ключ для переключения между атакой ближнего и дальнего действия
    public bool wait;//ключ требуемый для задержки в анимациях
    public bool alive;//ключ требуемый для определения, жив ли еще бот, или нет
    public bool Alert;//ключ для тревоги
    public bool stun;

    public Version Version; //переменная енума переключающая версии бота

    public EnemyState State;//переменная енума переключающая состояния бота

    public Animator Anim;//переменная компонента аниматор - требуемый для логики анимаций

    public GameObject Geosphere; // переменная для хранения "глаза" бота

    private CharacterController charController;

    #region Логика

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
        stun = false;
    }

    void Update()
    {
        if (alive)
        {
            if (Target != null)
            {
                DistanceTP = Vector3.Distance(transform.position, Target.position);
            }
        }
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
                        CaseMethod(false, UnityEngine.Random.Range(-1, 1.1f), -1, Target.transform.position);
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

    public void IsAttacketd()
    {
        
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
            Ray ray = new Ray(Geosphere.transform.position,
                (Target.transform.position + Vector3.up) - Geosphere.transform.position);

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


            if (!stun)
            {


                if (Version == Version.Standart)
                {
                    if (Distance < RangePursuit && Distance > RangeAttack && anglevis < AngleVision &&
                        Physics.Raycast(ray, out hit, RangePursuit, ignoreMask) &&
                        hit.transform.gameObject == Target.gameObject) //Move
                    {
                        if (anglecam < Camshooting)
                        {
                            IsMeleeAttack = false;
                        }

                        Alert = true;


                        if (Anim.GetBool("Shoot"))
                        {
                            Anim.SetBool("Shoot", false);
                        }
                        // переключалка опасности

                        NavAgent.destination = Target.transform.position;
                        StateSolver = EnemyState.Walk;
                        LastPosition = Target.position;
                    }
                    else if (Distance <= RangeAttack && anglevis < AngleVision &&
                             Physics.Raycast(ray, out hit, RangePursuit, ignoreMask) &&
                             hit.transform.gameObject == Target.gameObject) // \Attack
                    {
                        Alert = true;
                        if (!IsMeleeAttack && anglecam > AngleCamera) // переключатель
                        {
                            IsMeleeAttack = true;
                        }
                        else // код атаки
                        {
                            LastPosition = Target.position;
                            NavAgent.destination = transform.position;
                            Attack();
                        }
                    }
                    else if (LastPosition != StandardPosition) // Pursuit 
                    {
                        Alert = true;
                        if (Anim.GetBool("Shoot"))
                        {
                            Anim.SetBool("Shoot", false);
                        }
                        StateSolver = EnemyState.Walk;
                        NavAgent.destination = LastPosition;
                        if (LastPosition.x == transform.position.x && LastPosition.z == transform.position.z)
                        {
                            LastPosition = StandardPosition;
                        }
                    }
                    else //Stay
                    {
                        if (Alert)
                        {
                            StartCoroutine(Alarm());
                        }

                        if (Anim.GetBool("Shoot"))
                        {
                            Anim.SetBool("Shoot", false);
                        }

                        if (!wait)
                        {
                            wait = true;
                            CaseMethod(false, UnityEngine.Random.Range(-1, 1.1f), -1, Target.transform.position);
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
            else
            {
                NavAgent.destination = transform.position;
            }
        }
    }

    #endregion

    #region Поиск пути

    protected void CaseMethod(bool navAgentEnebled, float xstate, float ysate, Vector3 destenation)
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

    #endregion

    #region Урон

    private void Attack() 
    {
        Quaternion look = Quaternion.LookRotation(Target.transform.position - transform.position);
        float anglevis = Quaternion.Angle(transform.rotation, look); //угол между ботом и персом
        if (anglevis > AngleShhotingForBot)
        {
            Vector3 relativePos = Target.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos),
                Time.deltaTime * 5);
        }
        else
        {
            if (IsMeleeAttack || Distance <= RangeMeleeAttack)
            {
                if (Anim.GetBool("Shoot"))
                {
                    Anim.SetBool("Shoot", false);
                }

                NavAgent.destination = transform.position;

                Anim.SetTrigger("Fight");
                Anim.SetFloat("Xstate", 0);
                Anim.SetFloat("Ystate", -1);
                
            }
            else
            {
                NavAgent.destination = transform.position;
                Anim.SetBool("Shoot", true);
            }
        }
    }



    #endregion

    #region Раздражители

    protected virtual IEnumerator Alarm()
    {
        yield return new WaitForSeconds(2);
        Alert = false;
    }

    #region Получение урона

    public void GetDamage(int damage)//вызов карутина на секунду в котором ставится ключ на стоп машина ставится навмеш в ноги и в дистанции ключ не дает делать ничего
    {
        //Debug.Log("!");
        if (alive)
        {
            if (Health > 0)
            {
                Health -= damage;
                StartCoroutine(Damage());
            }
        }
    }
    protected virtual IEnumerator Damage()
    {
        stun = true;
        NavAgent.destination = transform.position;
        LastPosition = Target.transform.position;
        Anim.SetTrigger("Damage");        
        yield return new WaitForSeconds(3F);
        stun = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Sword"))
        {
            GetDamage(5);
        }

        Alert = true;
        IsAttacketd();
    }



    #endregion

    #endregion

    #region Потребности

    public float Health
    {
        get { return health; }

        set
        {
            health = (int) value;
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
    
    public void Dead()
    {
        alive = false;
        NavAgent.destination = transform.position;
        Anim.SetInteger("Health", health);
        Anim.SetTrigger("Damage");
    }
    #endregion


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


    #region IAlive

    public void ShockEffect(float time)
    {
    }


    public void DestroyModel()
    {
        Destroy(gameObject);
    }

    #endregion
}