using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragExplosion : MonoBehaviour, IThrowableEffect
{
    // Vou fazer do jeito pratico e anti democratico

    [SerializeField] private GameObject[] fragsObject; // volto já so n sei quando // será que eu deva jogar o avowed?
    [SerializeField] private float shakeForce = 16f;
    [SerializeField] private bool hasImpulse;
    [SerializeField] private LayerMask cullingMask;
    [SerializeField] private float impulseRadius = 5;
    [SerializeField] private int impulseDamage;
    [SerializeField] private float impulseForce;


    private FragBehaviors[] fragsDamage;
    private GameObject fragParent;

    private void Start()
    {
        fragParent = fragsObject[0].transform.parent.gameObject;
        fragsDamage = new FragBehaviors[fragsObject.Length];
        for (int i = 0; i < fragsObject.Length; i++)
        {
            fragsDamage[i] = fragsObject[i].GetComponent<FragBehaviors>();
            fragsObject[i].SetActive(false);
        }
    }
    public void ApplyEffect(Vector3 position, int damage)
    {
        CameraShake.instance.Shake(shakeForce);
        if (fragParent.activeSelf)
        {
            for (int i = 0; i < fragsObject.Length; i++)
            {
                fragsObject[i].SetActive(false);
            }
        }
        fragParent.transform.parent = null;
        fragParent.transform.position = transform.position;
        for (int i = 0; i < fragsObject.Length; i++)
        {
            fragsObject[i].SetActive(true);
            fragsDamage[i].damage = damage / fragsDamage.Length;
        }
        ExplosionImpulse(hasImpulse);
    }

    void ExplosionImpulse(bool canImpulse)
    {
        if (!canImpulse) return;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, impulseRadius, cullingMask);

        foreach (Collider2D hitCollider in hitColliders)
        {
            // Verificar se o objeto tem um componente que implemente IDamageable
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Aplica dano, se necessário // Mas é necessário
                damageable.TakeDamage(impulseDamage);
                
            }

            // Aplicar a força de repulsão
            Rigidbody2D rb = hitCollider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 direction = (hitCollider.transform.position - transform.position).normalized;
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                float force = impulseForce * (1 - (distance / impulseRadius)); // Força diminui com a distância
                force = Mathf.Clamp(force, 10, force);

                rb.AddForce(direction * force, ForceMode2D.Impulse);
            }
        }
    }

    float RandomAngle()
    {
        return Random.Range(0,360);
    }
}
