using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour, IDamageable
{
    protected float speed;
    protected int health;
    protected int damageAmmount;
    protected int cashToDrop;
    protected Vector2 ultimaPosicao;

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
        Debug.Log(x + y);
    }

    public void AjustarDirecao(NavMeshAgent agent, Transform objectTransform, ref Vector2 lastPos)
    {
        if (IsEnemyStopped(agent))
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

    private bool IsEnemyStopped(NavMeshAgent agent)
    {
        return agent.remainingDistance <= agent.stoppingDistance && agent.velocity.magnitude < 0.1f;
    }

}
