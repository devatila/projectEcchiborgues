using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour, IDamageable
{
    protected float speed;
    protected int health;
    protected int damageAmmount;
    protected int cashToDrop;
    protected Vector2 ultimaPosicao;
    protected EnemyBasicsAttributes EnemyBasics = new EnemyBasicsAttributes();

    protected Collider2D VisibleBoundsCollider; // Armazena o Collider2D encontrado
    protected string targetLayer = "VisibleBound"; // Nome da layer que é responsavel para encontrar o objeto que sera usado como limites visiveis deste inimigo
    protected SpriteRenderer[] spriteRenderers; // Todas as partes de sprites do inimigo ficam nessa variavel
    protected bool canAttack, isAttacking; // Valido para todos os tipos de ataques dos inimigos


    public Transform centralPosition;


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
        VisibleBoundsCollider = GetChildColliderWithLayer(gameObject, targetLayer);
        GetAllSprites();
        GetCentralPoint();
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
    public void Calcular(int x, int y)
    {
        UnityEngine.Debug.Log(x + y);
    }

    public void AjustarDirecao(NavMeshAgent agent, Transform objectTransform, ref Vector2 lastPos)
    {
        _isVisible = IsVisibleOnCamera(VisibleBoundsCollider);
        if (!objectTransform.gameObject.activeInHierarchy) return; // Evita processamento de objetos desativados
        if (!_isVisible) return; // Para o codigo se o inimigo não estiver aparecendo na camera

        // Se o inimigo estiver parado, verifica se precisa mudar a rotação
        if (IsEnemyStopped())
        {
            float indexRotation = agent.destination.x > agent.transform.position.x ? 0 : 180;

            if (objectTransform.eulerAngles.y != indexRotation)
            {
                objectTransform.rotation = Quaternion.Euler(0, indexRotation, 0);
            }
        }
        else
        {
            Vector2 direcaoMovimento = (Vector2)agent.transform.position - lastPos;

            if (Mathf.Abs(direcaoMovimento.x) > 0.001f) // Apenas atualiza se houver movimento
            {
                float novaRotacao = direcaoMovimento.x > 0 ? 0 : 180;

                if (objectTransform.eulerAngles.y != novaRotacao)
                {
                    objectTransform.rotation = Quaternion.Euler(0, novaRotacao, 0);
                }

                lastPos = agent.transform.position;
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
