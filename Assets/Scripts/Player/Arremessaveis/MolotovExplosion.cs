using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolotovExplosion : MonoBehaviour, IThrowableEffect
{
    public float fireRange;
    public float fireEffectProbability = 0.5f;
    public LayerMask cullingMask;
    private ThrowablesSO m_throwableData;

    public void ApplyEffect(GameObject hitObject, int damage)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, fireRange, cullingMask);

        foreach (Collider2D hitCollider in hitColliders)
        {
            // Verificar se o objeto tem um componente que implemente IDamageable
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Aplica dano, se necessário // Mas é necessário
                damageable.TakeDamage(damage);
            }

            IStateable stateable = hitCollider.GetComponent<IStateable>();
            if (stateable != null)
            {
                bool canEffect = (Random.Range(0f,1f) < fireEffectProbability) ? true : false;
                if (!canEffect) return;
                stateable.SetState(NaturalStates.Fire);
            }
        }
            VFX_PoolManager.instance.PlayFlameExplosion(transform.position, fireRange * 2); // quando isso ficar pronto vai ser outra historia heheh
    }

    public void SetThrowableData(ThrowablesSO throwableData)
    {
        m_throwableData = throwableData;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
