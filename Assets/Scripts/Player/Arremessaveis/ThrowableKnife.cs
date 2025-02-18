using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableKnife : MonoBehaviour, IThrowableEffect
{
    public bool isExplosive;
    public LayerMask cullingMask;
    public void ApplyEffect(GameObject hitObject, int damage)
    {
        // Tratando possivel erro
        if (hitObject == null)
        {
            Debug.LogWarning("Ojeto necessário que esta sendo acessado é nulo, verificar onde esta sendo chamado e corrigir");
            return;
        }
        IDamageable damageable = hitObject.GetComponent<IDamageable>();
        damageable.TakeDamage(damage);
    }

    public void SetThrowableData(ThrowablesSO throwableData)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
