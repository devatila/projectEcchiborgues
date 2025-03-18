using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    protected float speed;
    protected int health;
    protected int damageAmmount;
    protected int cashToDrop;

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
    
}
