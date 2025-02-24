using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SoldierBehavior : MonoBehaviour
{
    [SerializeField] private enum State { Idle, Chase, Circle, Retreat, Erratic }
    [SerializeField] private State currentState;
    [SerializeField] private System.Action stateUpdate;

    private NavMeshAgent agent;
    private Transform player;
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private float retreatDistance = 1.5f;
    [SerializeField] private float circleRadius = 3.0f;
    [SerializeField] private float circleSpeed = 3f;
    private Vector2 erraticDirection;

    private Coroutine CircleLoop;
    private float startCircleSpeed;

    private Vector3 normalScale;

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
        Debug.Log(CircleLoop == null);
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
            case State.Idle:
                stateUpdate = UpdateIdle;
                break;
            case State.Chase:
                stateUpdate = UpdateChase;
                break;
            case State.Circle:
                stateUpdate = UpdateCircle;
                break;
            case State.Retreat:
                stateUpdate = UpdateRetreat;
                break;
            case State.Erratic:
                stateUpdate = UpdateErratic;
                break;
        }
    }

    private void UpdateIdle()
    {
        agent.isStopped = true;
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
        
        Vector3 direction = (transform.position - player.transform.position).normalized; // Direção radial
        Vector3 perpendicularDirection = Vector3.Cross(direction, Vector3.forward); // Perpendicular para 2D

        // Mover o inimigo em um movimento circular ao redor do player
        transform.position += perpendicularDirection * circleSpeed * Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < retreatDistance)
        {
            CurrentState = State.Retreat;
            StopCoroutine(CircleLoop);
            CircleLoop = null;
        }
        if (distance > circleRadius * 1.1f)
        {
            CurrentState = State.Chase;
            StopCoroutine(CircleLoop);
            CircleLoop = null;
            
        }
    }
    IEnumerator CirclingLoop(float interludeTime)
    {
        float initialCircleSpeed = startCircleSpeed;

        while (true)
        {
            circleSpeed = 0;
            yield return new WaitForSeconds(interludeTime);
            circleSpeed = Random.Range(0, 2) > 0 ? initialCircleSpeed * -1 : initialCircleSpeed;
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

    private void UpdateErratic()
    {
        if (erraticDirection == Vector2.zero)
        {
            erraticDirection = Random.insideUnitCircle.normalized;
        }
        transform.position += (Vector3)(erraticDirection * agent.speed * Time.deltaTime);

        if (Random.value < 0.01f)
        {
            erraticDirection = Vector2.zero;
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
        bool isOnRightSide = player.gameObject.transform.position.x > transform.position.x;
        if (isOnRightSide)
        {
            transform.localScale = normalScale;
        }
        else
        {
            transform.localScale = new Vector3(normalScale.x * -1, normalScale.y, normalScale.z);
        }
    }
}
