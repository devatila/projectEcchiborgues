using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolotovExplosion : MonoBehaviour, IThrowableEffect
{
    public float fireRange;
    public LayerMask cullingMask;

    public void ApplyEffect(Vector3 position, int damage)
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
            if (stateable != null) stateable.SetState(NaturalStates.Fire);
        }
        VFX_PoolManager.instance.PlayFlameExplosion(position, fireRange * 2); // quando isso ficar pronto vai ser outra historia heheh
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
