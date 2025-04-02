using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Atributos Básicos")]
    [Tooltip("Ativar movimentação basica de perseguição e parar quando se aproximar")]
    [SerializeField] protected bool enableDefaultBehavior = true;

    [SerializeField] protected float speed;
    [SerializeField] protected int health;
    [SerializeField] protected int damageAmmount;
    [SerializeField] protected int cashToDrop;
    [Space()]

    [SerializeField] protected bool useAttackRangeAsStoppingDistance = true;
    [SerializeField] protected float attackRange;
    [SerializeField] protected Vector2 attackRangeOffset;

    protected Vector2 ultimaPosicao;
    protected Transform playerPos; // Posição do player
    protected EnemyBasicsAttributes EnemyBasics = new EnemyBasicsAttributes();

    protected Collider2D VisibleBoundsCollider;    // Armazena o Collider2D encontrado
    protected string targetLayer = "VisibleBound"; // Nome da layer que é responsavel para encontrar o objeto que sera usado como limites visiveis deste inimigo
    protected SpriteRenderer[] spriteRenderers;    // Todas as partes de sprites do inimigo ficam nessa variavel
    [SerializeField] protected bool canAttack, isAttacking;         // Valido para todos os tipos de ataques dos inimigos


    public Transform centralPosition { get; protected set; }


    private bool isVisible;
    private bool _isVisible { get => isVisible; 
        set {
            if (isVisible != value)
            {
                isVisible = value;
                EnemyVisiblePartsHandler();
            }        
        } 
    }

    

    public virtual void Start()
    {
        EnemyBasics.GetReferences(this);
        EnemyBasics.agent.speed = speed;
        if(useAttackRangeAsStoppingDistance) EnemyBasics.agent.stoppingDistance = attackRange;
        ultimaPosicao = transform.position;

        playerPos = FindObjectOfType<PlayerInventory>().gameObject.transform;

        VisibleBoundsCollider = GetChildColliderWithLayer(gameObject, targetLayer);

        GetAllSprites();
        GetCentralPoint();
    }

    public virtual void Update()
    {
        AjustarDirecao();
        if (enableDefaultBehavior)
            DefaultBehavior();
    }

    public virtual void SetStun(float timeStunned)
    {
        // Deixar o inimigo Stunnado
    }

    public virtual void TakeDamage(int damage, bool shouldPlayDamageAnim = true)
    {
        
    }

    public virtual void Move()
    {
        // Faça o inimigo se mover aqui
    }

    public virtual void Die()
    {

    }

    public virtual void Attack()
    {

    }

    private void DefaultBehavior() => EnemyBasics.agent.SetDestination(playerPos.position);

    public void AjustarDirecao()
    {
        _isVisible = IsVisibleOnCamera(VisibleBoundsCollider);
        if (!_isVisible) return; // Para o codigo se o inimigo não estiver aparecendo na camera

        // Se o inimigo estiver parado, verifica se precisa mudar a rotação
        if (IsEnemyStopped())
        {
            float indexRotation = EnemyBasics.agent.destination.x > EnemyBasics.agent.transform.position.x ? 0 : 180;

            if (transform.eulerAngles.y != indexRotation)
            {
                transform.rotation = Quaternion.Euler(0, indexRotation, 0);
            }
        }
        else
        {
            Vector2 direcaoMovimento = (Vector2)EnemyBasics.agent.transform.position - ultimaPosicao;

            if (Mathf.Abs(direcaoMovimento.x) > 0.001f) // Apenas atualiza se houver movimento
            {
                float novaRotacao = direcaoMovimento.x > 0 ? 0 : 180;

                if (transform.eulerAngles.y != novaRotacao)
                {
                    transform.rotation = Quaternion.Euler(0, novaRotacao, 0);
                }

                ultimaPosicao = EnemyBasics.agent.transform.position;
            }
        }
    }

    #region VisibleParts
    public void EnemyVisiblePartsHandler()
    {
        if (_isVisible)
        {
            Debug.Log("Apareceu na tela");
            ShowOnInScreen();
        }
        else
        {
            Debug.Log("Saiu da tela");
            HideOnOffScreen();
        }
    }

    private void HideOnOffScreen()
    {
        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            sprite.enabled = false;
        }
    }

    private void ShowOnInScreen()
    {
        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            sprite.enabled = true;
        }
    }
    #endregion
    private Collider2D GetChildColliderWithLayer(GameObject parent, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);

        foreach (Collider2D col in parent.GetComponentsInChildren<Collider2D>())
        {
            if (col.gameObject.layer == layer)
            {
                return col; // Retorna o primeiro Collider2D encontrado na layer correta
            }
        }

        return null; // Retorna nulo se nenhum Collider2D for encontrado na layer
    }

    private bool IsVisibleOnCamera(Collider2D objectTransform)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        Collider2D col = objectTransform;

        if (col != null)
        {
            return GeometryUtility.TestPlanesAABB(frustumPlanes, col.bounds);
        }
        else
        {
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(objectTransform.gameObject.transform.position);
            return viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1;
        }
    }

    void GetAllSprites() => spriteRenderers = GetComponentsInChildren<SpriteRenderer>();


    private bool IsEnemyStopped()
    {
        return EnemyBasics.agent.remainingDistance <= EnemyBasics.agent.stoppingDistance && EnemyBasics.agent.velocity.magnitude < 0.1f;
    }

    void GetCentralPoint()
    {
        GameObject centroObj = GameObject.FindWithTag("PontoCentral");

        if (centroObj != null && centroObj.transform.IsChildOf(transform))
        {
            centralPosition = centroObj.transform;
        }
        else
        {
            Debug.LogWarning($"PontoCentral não encontrado em {gameObject.name}, usando posição padrão.");
            centralPosition = transform;
        }
    }

    public bool IsFacingPlayer()
    {
        Vector2 directionToPlayer = (EnemyBasics.agent.destination - transform.position).normalized;
        Vector2 facingDirection = transform.right; // Direção que o inimigo está olhando

        return Vector2.Dot(directionToPlayer, facingDirection) > 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x + attackRangeOffset.x, transform.position.y + attackRangeOffset.y, 0), attackRange);
    }

    [System.Serializable]
    public class EnemyBasicsAttributes
    {
        public NavMeshAgent agent;
        public Vector2 lastPosition;
        public Transform objectTransform;

        public void GetReferences(MonoBehaviour owner)
        {
            agent = owner.GetComponent<NavMeshAgent>();
            lastPosition = owner.transform.position;
            objectTransform = owner.transform;

            
            agent.updateRotation = false;
            agent.updateUpAxis = false;

            
        }
    }
}
