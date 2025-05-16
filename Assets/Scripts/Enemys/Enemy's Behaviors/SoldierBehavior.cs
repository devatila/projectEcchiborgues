using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SoldierBehavior : MonoBehaviour, IDamageable
{
    [SerializeField] private enum State { Idle ,Chase, Circle, Retreat }
    [SerializeField] private State currentState;
    private System.Action stateUpdate;

    private NavMeshAgent agent;
    private Transform player;
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private float retreatDistance = 1.5f;
    [SerializeField] private float circleRadius = 3.0f;
    [SerializeField] private float circleSpeed = 3f;
    

    private Coroutine CircleLoop;
    private float startCircleSpeed;

    private Vector3 normalScale;
    [Space()]

    // Atributos basicos
    [Header("Atributos Basicos")]
    [SerializeField] private int health = 1500;
    //[SerializeField] private int damageAttack = 30;

    [Space()]

    //References
    [Header("Referencias")]
    [SerializeField] private Animator anim;
    [SerializeField] private LookPlayerPositionTracker positionTracker;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        startCircleSpeed = circleSpeed;
        normalScale = transform.localScale;

        CurrentState = State.Chase;
    }

    private void Update()
    {
        //Debug.Log(CircleLoop != null);
        SettingAnimations();
        CheckToFlipEnemy();
        stateUpdate?.Invoke();
    }

    private State CurrentState
    {
        get => currentState;
        set
        {
            if (currentState != value)
            {
                currentState = value;
                HandleStateChange();
            }
        }
    }

    private void HandleStateChange()
    {
        switch (currentState)
        {
            case State.Chase:
                stateUpdate = UpdateChase;
                break;
            case State.Circle:
                stateUpdate = UpdateCircle;
                break;
            case State.Retreat:
                stateUpdate = UpdateRetreat;
                break;
        }
    }

    private void UpdateChase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        if (Vector2.Distance(transform.position, player.position) < circleRadius)
        {
            CurrentState = State.Circle;
        }
    }

    private void UpdateCircle()
    {
        agent.isStopped = true;
        

        if(CircleLoop == null)
        {
            Debug.Log(CircleLoop == null);
            CircleLoop = StartCoroutine(CirclingLoop(1.5f));
        }

        Vector3 direction = (transform.position - positionTracker.targetPosition.position).normalized; // Direção radial
        Vector3 perpendicularDirection = Vector3.Cross(direction, Vector3.forward); // Perpendicular para 2D

        // Mover o inimigo em um movimento circular ao redor do player
        transform.position += perpendicularDirection * circleSpeed * Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < retreatDistance)
        {
            CurrentState = State.Retreat;
            StopCircleLoop();
        }
        if (distance > circleRadius * 1.1f)
        {
            CurrentState = State.Chase;
            StopCircleLoop();
        }
    }

    void StopCircleLoop()
    {
        StopCoroutine(CircleLoop);
        CircleLoop = null;
        circleSpeed = startCircleSpeed;
    }
    IEnumerator CirclingLoop(float interludeTime)
    {
        float initialCircleSpeed = startCircleSpeed;

        while (true)
        {
            circleSpeed = 0;
            yield return new WaitForSeconds(interludeTime);
            
            circleSpeed = Random.Range(0, 2) > 0 ? initialCircleSpeed * -1 : initialCircleSpeed;
            yield return new WaitForSeconds(interludeTime);
        }
    }

    private void UpdateRetreat()
    {
        agent.isStopped = true;
        Vector2 retreatDirection = (transform.position - player.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)retreatDirection, agent.speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, player.position) > retreatDistance * 2)
        {
            CurrentState = State.Chase;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, circleRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);
    }

    void CheckToFlipEnemy()
    {
        float isOnRightSide = player.transform.position.x > transform.position.x ? 1 : -1;

        transform.localScale = new Vector3(normalScale.x * isOnRightSide, normalScale.y, normalScale.z);
        
    }

    void Death()
    {
        ItemDropManager.Instance.OnEnemyDeath(transform.position);
        EnemyIndicator.instance.OnEnemyDeath(this.gameObject);
        RaidManager.instance.RemoveEnemyOnDeath(this.gameObject);
        this.gameObject.SetActive(false);
    }

    public void TakeDamage(int damage, bool shouldPlayDamageAnim = true)
    {
        health -= damage;
        if (health < 0)
        {
            Death();
        }
    }

    public void SetStun(float timeStunned)
    {
        
    }

    void SettingAnimations()
    {
        int stateID = circleSpeed == 0 ? 1 : 2;
        
        anim.SetInteger("StateID", stateID);
    }

    public void SetStun(bool hasToStun = true)
    {
        throw new System.NotImplementedException();
    }

    public void ApplyNaturalState(NaturalStates newState, float duration, float DOTtime = 1)
    {
        throw new System.NotImplementedException();
    }
}
