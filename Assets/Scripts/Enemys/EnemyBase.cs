using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    public virtual void SetStun(float timeStunned)
    {
        
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
        if (IsEnemyStopped())
        {
            float indexRotation = agent.destination.x > agent.transform.position.x ? 0 : 180;

            objectTransform.rotation = Quaternion.Euler(0, indexRotation, 0);
        }
        else
        {
            Vector2 direcaoMovimento = (Vector2)transform.position - lastPos;

            if (direcaoMovimento.x > 0.001f)
            {
                objectTransform.rotation = Quaternion.Euler(0, 0, 0); // Direita
            }
            else if (direcaoMovimento.x < -0.001f)
            {
                objectTransform.rotation = Quaternion.Euler(0, 180, 0); // Esquerda
            }

            lastPos = objectTransform.position;
        }

        
    }

    private bool IsEnemyStopped()
    {
        return EnemyBasics.agent.remainingDistance <= EnemyBasics.agent.stoppingDistance && EnemyBasics.agent.velocity.magnitude < 0.1f;
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
