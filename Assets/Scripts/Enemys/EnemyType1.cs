using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyType1 : MonoBehaviour//, //IDamageable
{                                   // DESCONTINUADO //
    public EnemyDataSO enemyDataSO;

    public bool isEnemyAlive {get; private set; }

    [SerializeField] private int health = 100;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private GameObject projectilePrefab;

    private bool canEvade;
    private AudioClip StepSound;
    private int[] damagesCombos;
    private int damage;
    private int gunHoldIndex; // 0 = One Hand, 1 = Two Hand, 2 = RPG, 3 = Large Gun, 4 = Incorpored
    private GameObject gunOrMeleePrefab;


    private PlayerInventory player;
    [SerializeField] private float attackDistance;
    public float cadency;
    private bool isOnRange, canShoot;
    public LayerMask playerLayer;
    public float circleSpeed;
    private bool isCircling = false;
    public float followDistance;
    public enum enemyTypes { low, medium, high }
    public enemyTypes enemyType;
    private bool podeParar = true;
    public float retreatDistance;
    public float minDistance;
    private float initialFollowDistance;
    private float initialCircleSpeed;
    private PoolEnemyProjectiles poolEnemyProjectiles;

    private bool isOnRightSide = true;
    public bool _isOnRightSide { get { return isOnRightSide; } set { isOnRightSide = value; } }
    private Vector3 normalScale;
    private Animator animator;

    private LookPlayerPositionTracker lookPlayerPositionTracker;
    private bool isStunned;

    private Coroutine CadencyLoop;
    private Coroutine StunLoop;

    private void OnEnable()
    {
        isEnemyAlive = true;
        isStunned = false;
    }
    public void TakeDamage(int damage, bool shouldPlayDamageAnim = true)
    {
        health -= damage;
        
        if (health <= 0)
        {
            if(isEnemyAlive) Death();
        }
    }

    public void SetStun(float timeStunned)
    {
        // Lógica para Atordoamento
        if (!isEnemyAlive) return;
        Debug.Log("Stunnou");
        if (StunLoop != null)
        {
            StopCoroutine(StunLoop);
            StunLoop = null;
        }
        StunLoop = StartCoroutine(StunEffector(timeStunned));
    }

    IEnumerator StunEffector(float timer)
    {
        SetStunState();

        yield return new WaitForSeconds(timer);

        SetStunState(false);
    }
    void SetStunState(bool stunned = true)
    {
        isStunned = stunned;
        agent.isStopped = stunned;
        circleSpeed = stunned ? 0 : initialCircleSpeed;

        if (stunned)
        {
            if (CadencyLoop != null) StopCoroutine(CadencyLoop);
            lookPlayerPositionTracker.SetDefaultPosition();
            animator.SetBool("IsStunned", true);
            animator.SetTrigger("GotStun");
        }
        else
        {
            lookPlayerPositionTracker.SetParent();
            animator.SetBool("IsStunned", false);
        }
    }


    void DropOnDeath()
    {
        // Lógica de drop quando morre --
        ItemDropManager.Instance.OnEnemyDeath(transform.position);
    }

    void Death()
    {
        isEnemyAlive = false;
        //Debug.Log("Morreu");
        DropOnDeath();
        EnemyIndicator.instance.OnEnemyDeath(this.gameObject);
        RaidManager.instance.RemoveEnemyOnDeath(this.gameObject);
        this.gameObject.SetActive(false);
    }

    void GetDataSO(EnemyDataSO enemyDataSO)
    {
        GetBasicsSO(enemyDataSO);
        switch (enemyDataSO.attackTypeOfTheEnemy)
        {
            case EnemyDataSO.attackTypes.FireGun:
                damage = enemyDataSO.gunDamage;
                cadency = enemyDataSO.FireRate;
                switch (enemyDataSO.howToHoldTheChoicedGun)
                {
                    case EnemyDataSO.HoldTypes.OneHand:
                        gunHoldIndex = 0;
                        break;
                    case EnemyDataSO.HoldTypes.TwoHand:
                        gunHoldIndex = 1;
                        break;
                    case EnemyDataSO.HoldTypes.RPG:
                        gunHoldIndex = 2;
                        break;
                    case EnemyDataSO.HoldTypes.LargeGun:
                        gunHoldIndex = 3;
                        break;
                }
                gunOrMeleePrefab = Instantiate(enemyDataSO.GunPrefab);

                break;
            case EnemyDataSO.attackTypes.Melee:

                break;
        }
    }

    private void GetBasicsSO(EnemyDataSO enemyDataSO)
    {
        health = enemyDataSO.health;
        agent.speed = enemyDataSO.speed;
        canEvade = enemyDataSO.canHeEvade;
        StepSound = enemyDataSO.stepsSound_SFX;
    }

    void Start()
    {
        if (enemyDataSO != null) GetDataSO(enemyDataSO);
        InitializationAndGettingReferences();
        animator.SetInteger("GunID", gunHoldIndex);
    }

    private void InitializationAndGettingReferences()
    {
        poolEnemyProjectiles = FindObjectOfType<PoolEnemyProjectiles>();
        canShoot = true;
        player = FindObjectOfType<PlayerInventory>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        //agent.stoppingDistance = followDistance;
        initialFollowDistance = followDistance;
        initialCircleSpeed = circleSpeed;
        normalScale = transform.localScale;
        animator = GetComponent<Animator>();
        lookPlayerPositionTracker = GetComponent<LookPlayerPositionTracker>();
    }

    void Update()
    {
        CHECK_PLAYER_POSITION_AND_FLIP();
        CHECK_AGENT_VELOCITY_AND_CHANGE_THE_DESIRED_ANIMATION();

        Vector3 position = transform.position;
        position.z = 0f;
        transform.position = position;

        switch (enemyType)
        {
            case enemyTypes.low:
                CommumBehavior(true, 4f);
                break;
            case enemyTypes.medium:
                CommumBehavior(false, 6f);
                break;
            case enemyTypes.high:
                CommumBehavior(false, 3f);
                break;

                // Adicionar outros comportamentos de inimigos conforme necessário
        }
    }

    private void CHECK_AGENT_VELOCITY_AND_CHANGE_THE_DESIRED_ANIMATION()
    {
        
        if (agent.velocity.sqrMagnitude > 0.1f || circleSpeed != 0 && agent.isStopped)
        {
            animator.SetInteger("StateID", 2);
        }
        else if (agent.remainingDistance <= agent.stoppingDistance + 0.1f || circleSpeed == 0 && agent.isStopped)
        {
            animator.SetInteger("StateID", 1);
        }
    }

    void CHECK_PLAYER_POSITION_AND_FLIP()
    {
        if (isStunned) return;
        isOnRightSide = player.gameObject.transform.position.x > transform.position.x;
        if (isOnRightSide)
        {
            transform.localScale = normalScale;
        }
        else
        {
            transform.localScale = new Vector3(normalScale.x * -1, normalScale.y, normalScale.z);
        }
    }
    private void CommumBehavior(bool isLow,float timeToReact)
    {
        if (isStunned) return;
        Vector3 direction = player.transform.position - shootPosition.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        shootPosition.rotation = Quaternion.Euler(0, 0, angle);
        if (canShoot && isOnRange)
        {
            if (isLow)
            {
                canShoot = false;
                StartCoroutine(ChargingAttack(3f));
            }
            else
            {
                GameObject projectile = poolEnemyProjectiles.GetObject();
                projectile.transform.position = shootPosition.position;
                projectile.transform.rotation = shootPosition.rotation;
                //Instantiate(projectilePrefab, shootPosition.position, shootPosition.rotation);
                canShoot = false;
                CadencyLoop = StartCoroutine(Cadencia(cadency));
            }
            
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < followDistance)
        {
            // Começar a circular o player
            if (!isCircling)
            {
                isCircling = true;
                agent.isStopped = true; // Para o movimento do NavMeshAgent
            }

            // Calcular o movimento circular e considerar o recuo
            if (podeParar)
            {
                podeParar = false;
                StartCoroutine(timeStoppedAndRetreat(timeToReact, false));
            }
            else
            {
                CircleAroundPlayer();
            }
        }
        else
        {
            // Perseguir o player usando o NavMesh
            if (isCircling)
            {
                isCircling = false;
                agent.isStopped = false; // Retomar o movimento do NavMeshAgent
                StopCoroutine(timeStoppedAndRetreat(timeToReact, false));
            }

            agent.SetDestination(player.transform.position);
        }
    }

    
    IEnumerator ChargingAttack(float time)
    {
        Debug.Log("Carregando Ataque");
        yield return new WaitForSeconds(time);
        Debug.Log("Soltando Ataque");
        GameObject projectile = poolEnemyProjectiles.GetObject();
        projectile.transform.position = shootPosition.position;
        projectile.transform.rotation = shootPosition.rotation;
        //Instantiate(projectilePrefab, shootPosition.position, shootPosition.rotation);
        StartCoroutine(Cadencia(cadency));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        Gizmos.DrawWireSphere(transform.position, followDistance);
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }

    void CircleAroundPlayer()
    {
        Vector3 direction = (transform.position - player.transform.position).normalized; // Direção radial
        Vector3 perpendicularDirection = Vector3.Cross(direction, Vector3.forward); // Perpendicular para 2D

        // Mover o inimigo em um movimento circular ao redor do player
        transform.position += perpendicularDirection * circleSpeed * Time.deltaTime;
    }

    IEnumerator Cadencia(float gunCadency)
    {
        yield return new WaitForSeconds(gunCadency);
        canShoot = true;
        CadencyLoop = null;
    }

    IEnumerator timeStoppedAndRetreat(float time, bool canRetreat)
    {

        bool samePos = false;
        // Parar de circular e recuar
        circleSpeed = 0;
        yield return new WaitForSeconds(time);
        circleSpeed = Random.Range(0, 2) == 0 ? initialCircleSpeed : initialCircleSpeed * -1;
        yield return new WaitForSeconds(1f);
        circleSpeed = 0f;

        if (Vector3.Distance(transform.position, player.transform.position) <= minDistance)
        {
            samePos = true;
        }

        // Escolher uma direção aleatória para recuar
        Vector3 retreatDirection = (transform.position - player.transform.position).normalized;

        if (samePos)
        {
            Vector3 randomPos = Random.insideUnitCircle * attackDistance;
            Vector3 desiredPos = transform.position + randomPos;
            agent.isStopped = false; // AAAAAhhhh AAHH AAHH MINA DO BAILE EU VO GOZA A PANANANTATANTAN PANANA TAM TAM VO GOZA-A odeio minha vida ;-;
            agent.SetDestination(desiredPos);  //new Vector3(randomPos.x, randomPos.y, transform.position.z)
            yield return new WaitForSeconds(1f);
            samePos = false;
        }

        //TONIGHT THE MUSIC SEEMS SO LOUDDDDD!!!!!

        if (canRetreat)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < retreatDistance)
            {
                
                Vector3 retreatTarget = transform.position + retreatDirection * (retreatDistance);
                followDistance = initialFollowDistance;
                agent.isStopped = false;
                agent.SetDestination(retreatTarget);
                yield return new WaitForSeconds(time);
            }

            if (followDistance != retreatDistance)
            {
                bool canAproximate = Random.Range(0, 2) == 0 ? true : false;
                if (canAproximate)
                {
                    Vector3 retreatTarget = transform.position + retreatDirection * (retreatDistance - 2);
                    followDistance = retreatDistance;
                    agent.isStopped = false;
                    agent.SetDestination(retreatTarget);
                }
                

            }
            else
            {
                bool canAproximate = Random.Range(0, 2) == 0 ? true : false;
                if (canAproximate)
                {
                    followDistance = initialFollowDistance;
                    agent.isStopped = false;
                    agent.SetDestination(player.transform.position);
                }
            }
        }
        
        yield return new WaitForSeconds(1f);
        followDistance = initialFollowDistance;
        // Retomar movimento circular
        
        podeParar = true;
    }
}
