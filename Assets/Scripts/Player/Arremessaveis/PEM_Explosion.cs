using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PEM_Explosion : MonoBehaviour, IThrowableEffect
{
    public float effectRadius;
    public float stunTime;

    public LayerMask cullingMask;
    public void ApplyEffect(Vector3 position, int damage)
    {
        VFX_PoolManager.instance.PlayPEM_Effect(position, stunTime);
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
                stateable.SetState(NaturalStates.Eletric);
            }
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
}
