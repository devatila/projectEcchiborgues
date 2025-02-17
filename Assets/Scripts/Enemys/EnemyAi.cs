using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// O c�digo funciona t�o bem que estou com medo. Acho que tem algum erro s� esperando pra acontecer no momento em que eu mexer em qualquer linha
// desse c�digo
public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Perseguindo,
        Circulando,
        Recuando
    }
    public EnemyState _currentState;
    public EnemyState currentState 
    { get { return _currentState; } 
      set { if (_currentState != value) { _currentState = value; OnChangeEnum(); } } } // Estado atual do inimigo
    public Transform player;                      // Refer�ncia ao player
    public NavMeshAgent agent;                    // Refer�ncia ao NavMeshAgent para movimenta��o
    public float pursuitDistance = 10f;           // Dist�ncia para come�ar a perseguir o jogador
    public float retreatDistance = 2f;            // Dist�ncia para recuar ou atacar
    public float circulationDistance = 4f;        // Dist�ncia m�nima para come�ar a circular
    public float circulationDurationMin = 0.5f;   // Tempo m�nimo circulando
    public float circulationDurationMax = 2f;     // Tempo m�ximo circulando
    [Space()]
    public GameObject positionTracker;
    public float reactionTime = 3f;
    public Animator animator;

    private bool isCirculating = false;

    private bool isOnRightSide;
    private Vector3 normalScale;
    private bool canPursuit; public bool canRetreat = true;
    private bool arrivedThePoint = false;

    void OnChangeEnum()
    {
        switch (currentState) {
            case EnemyState.Recuando:
                //canRetreat = true;
                StopCoroutine(CirculatePlayer());
                break;
            case EnemyState.Circulando:
                canRetreat = true;
                arrivedThePoint = false;
                Debug.Log("Mudou");
                break;
            case EnemyState.Perseguindo:
                canRetreat = true;
                arrivedThePoint = false;

                break;
        }   
    }
    private void Start()
    {
        arrivedThePoint = false;
        normalScale = transform.localScale;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        currentState = EnemyState.Perseguindo;
        StartCoroutine(StateMachine()); // Inicia a m�quina de estados
    }

    private IEnumerator StateMachine()
    {
        while (true)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            switch (currentState)
            {
                case EnemyState.Perseguindo:
                    PursuePlayer();
                    if (distanceToPlayer < circulationDistance && canRetreat)
                    {
                        currentState = EnemyState.Circulando;
                    }
                    else if (distanceToPlayer < retreatDistance)
                    {
                        currentState = EnemyState.Recuando;
                    }
                    
                    break;

                case EnemyState.Circulando:
                    if (!isCirculating && currentState != EnemyState.Recuando)
                    {
                        isCirculating = true;
                        Debug.Log("Contagem");
                        StartCoroutine(CirculatePlayer());
                    }
                    if (distanceToPlayer < retreatDistance)
                    {
                        currentState = EnemyState.Recuando;
                    }
                    if (distanceToPlayer > pursuitDistance)
                    {
                        
                        currentState = EnemyState.Perseguindo;
                    }
                    break;

                case EnemyState.Recuando:
                    
                    RetreatOrAttack();
                    if (arrivedThePoint)
                    {
                        currentState = EnemyState.Circulando;
                    }
                    if (distanceToPlayer > pursuitDistance)
                    {
                        
                        //currentState = EnemyState.Perseguindo;
                    }

                    break;
            }

            yield return null; // Espera at� o pr�ximo frame
        }
    }

    private void PursuePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        //Debug.Log("Perseguindo o jogador...");
        // Aqui poderia ser chamada uma fun��o de ataque � dist�ncia, por exemplo
        AttemptAttack();
    }

    private IEnumerator CirculatePlayer()
    {
        isCirculating = true;


        float randomDuration = Random.Range(circulationDurationMin, circulationDurationMax); // Dura��o aleat�ria de circula��o
        Vector3 circulationDirection = (player.position - transform.position).normalized;

        // Define se o inimigo vai circular no sentido hor�rio ou anti-hor�rio
        float directionMultiplier = Random.value > 0.5f ? 1f : -1f;

        float elapsedTime = 0f;
        while (elapsedTime < randomDuration)
        {
            if (!canRetreat)
            {
                yield break;
            }
            
            Vector3 perpendicularDirection = Vector3.Cross(circulationDirection, Vector3.forward) * directionMultiplier;

            Vector3 targetPosition = player.position + perpendicularDirection * circulationDistance;
            agent.SetDestination(targetPosition);

            elapsedTime += Time.deltaTime;
            yield return null; // Espera at� o pr�ximo frame

        }
        agent.isStopped = true;
        yield return new WaitForSeconds(reactionTime);
        agent.isStopped = false;

        isCirculating = false;
        currentState = EnemyState.Perseguindo; // Volta a perseguir o jogador ap�s circular
    }

    private void RetreatOrAttack()
    {
        //Debug.Log("Recuando ou atacando...");

        if (canRetreat == true) {
            canRetreat = false;
            
            agent.isStopped = false;
            StopCoroutine(CirculatePlayer());
            StartCoroutine(RetreatFromPlayer()); 
        }
        //MeleeAttack();
    }

    IEnumerator RetreatFromPlayer()
    {
        
        Debug.Log("Recuando...");
        
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        Vector3 retreatPosition = transform.position + directionAwayFromPlayer * retreatDistance * 2;
        agent.isStopped = false;
        
        agent.SetDestination(retreatPosition);
        positionTracker.transform.position = retreatPosition;

        while (Vector3.Distance(transform.position, retreatPosition) > 0.1f)
        {
            StopCoroutine(CirculatePlayer());
            yield return null;
        }
        canRetreat = true;
        arrivedThePoint = true;
    }

    private void MeleeAttack()
    {
        Debug.Log("Ataque corpo a corpo!");
        // Aqui seria onde o ataque corpo a corpo seria implementado
    }

    private void AttemptAttack()
    {
        // Verifica se o inimigo est� em uma posi��o que pode atacar o jogador
        // Pode ser um ataque de longo alcance enquanto persegue/circula
        //Debug.Log("Tentando atacar...");
    }

    private void Update()
    {
        PositionCorrection();
        CHECK_PLAYER_POSITION_AND_FLIP();
        CHECK_AGENT_VELOCITY_AND_CHANGE_THE_DESIRED_ANIMATION();

        if (Input.GetKeyDown(KeyCode.P))
        {
            StopCoroutine(CirculatePlayer());
        }

    }

    void CHECK_PLAYER_POSITION_AND_FLIP()
    {
        isOnRightSide = player.position.x > transform.position.x;
        if (isOnRightSide)
        {
            transform.localScale = normalScale;
        }
        else
        {
            transform.localScale = new Vector3(normalScale.x * -1, normalScale.y, normalScale.z);
        }
    }
    private void PositionCorrection()
    {
        Vector3 position = transform.position;
        position.z = 0f;
        transform.position = position;
    }

    private void CHECK_AGENT_VELOCITY_AND_CHANGE_THE_DESIRED_ANIMATION()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            animator.SetInteger("StateID", 2);
        }
        else if (agent.remainingDistance <= agent.stoppingDistance + 0.1f || agent.isStopped)
        {
            animator.SetInteger("StateID", 1);
        }
    }
}

// EU SOU O CARA MAIS BURRO DO MUNDO NAMORAL KKKKKK ;-;
// EU REFIZ O C�DIGO DE IA DE INIMIGO PADR�O S� PRA RESOLVER O PROBLEMA DE ANIMA��O...
// E ERA S� EU TER INCREMENTADO UM IF NO OUTRO C�DIGO
// O JEITO � FAZER O L E ENGOLIR O CHORO QUE PASSA.
