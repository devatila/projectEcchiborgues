using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour, IDamageable
{
    private EnemyBase m_Enemy;
    private BoxCollider2D m_BoxCollider;

    [Range(-100, 100)]
    [SerializeField] private int damageEffector; // Valor dado em porcentagem...Tendeu?
    void Start()
    {
        m_Enemy = GetComponentInParent<EnemyBase>();
        m_BoxCollider = GetComponent<BoxCollider2D>();
    }

    

    void SetDamage(int damageAmmount, bool playAnim)
    {
        // Se o damage effector for diferente de 0, significa que alguma alteração sera realizada no dano tomado
        if(damageEffector != 0) damageAmmount += (damageAmmount * damageEffector) / 100; 

        if (m_Enemy != null)
        {
            m_Enemy.TakeDamage(damageAmmount, playAnim);
        }
        else
        {
            Debug.LogError("Classe base de inimigo não encontrado, verificar o objeto pai ou a posição na hierarquia deste objeto");
        }
    }

    public void TakeDamage(int damage, bool shouldPlayDamageAnim = true)
    {
        SetDamage(damage, shouldPlayDamageAnim);
    }

    public void SetStun(float timeStunned)
    {
        
    }
}
