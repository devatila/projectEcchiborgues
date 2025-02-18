using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PEM_Explosion : MonoBehaviour, IThrowableEffect
{
    public float effectRadius;
    public float stunTime;
    [Range(0f,1f)]
    [SerializeField] private float stunProbability;

    public LayerMask cullingMask;
    private ThrowablesSO m_throwableData;
    public void ApplyEffect(GameObject hitObject, int damage)
    {
        VFX_PoolManager.instance.PlayPEM_Effect(transform.position, stunTime);
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, effectRadius, cullingMask);

        foreach (Collider2D hitCollider in hitColliders)
        {
            // Verificar se o objeto tem um componente que implemente IDamageable
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Aplica dano, se necessário // Mas é necessário
                damageable.TakeDamage(damage);
                damageable.SetStun(stunTime);
            }
            IStateable stateable = hitCollider.GetComponent<IStateable>();
            if (stateable != null)
            {
                bool canStun = (Random.Range(0f, 1f) <= stunProbability) ? true : false;
                if (!canStun) return;
                stateable.SetState(NaturalStates.Eletric);
            }
            
        }
    }

    public void SetThrowableData(ThrowablesSO throwableData)
    {
        m_throwableData = throwableData;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}
