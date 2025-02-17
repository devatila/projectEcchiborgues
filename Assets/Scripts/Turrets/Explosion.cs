using UnityEngine;

public class Explosion : MonoBehaviour, IThrowableEffect
{
    public float explosionRadius = 5f;
    public float explosionForce = 10f;
    public LayerMask enemyLayer;

    private CustomProjectileScript customProj;
    private TurretProjectileBehavior turretBehavior;
    private void Start()
    {
        
        customProj = GetComponent<CustomProjectileScript>();
        if(customProj != null) customProj.OnHitCollides += TriggerExplosion;

        turretBehavior = GetComponent<TurretProjectileBehavior>();
        if (turretBehavior != null) turretBehavior.OnHitOrTimeTrigger += TriggerExplosion;
    }
    public void TriggerExplosion(Collider2D hit, int damage)
    {
        Debug.Log(damage);
        // Detectar inimigos próximos
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
        foreach (Collider2D hitCollider in hitColliders)
        {
            // Verificar se o objeto tem um componente que implemente IDamageable
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Aplica dano, se necessário // Mas é necessário
                damageable.TakeDamage(damage);
            }

            // Aplicar a força de repulsão
            Rigidbody2D rb = hitCollider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 direction = (hitCollider.transform.position - transform.position).normalized;
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                float force = explosionForce * (1 - (distance / explosionRadius)); // Força diminui com a distância
                force = Mathf.Clamp(force, 10, force);

                rb.AddForce(direction * force, ForceMode2D.Impulse);
            }
        }
    }
    public void TriggerExplosionManual(int damage)
    {
        CameraShake.instance.Shake(16f);
        // Detectar inimigos próximos
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
        foreach (Collider2D hitCollider in hitColliders)
        {
            // Verificar se o objeto tem um componente que implemente IDamageable
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Aplica dano, se necessário // Mas é necessário
                damageable.TakeDamage(damage);
            }

            // Aplicar a força de repulsão
            Rigidbody2D rb = hitCollider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector3 direction = (hitCollider.transform.position - transform.position).normalized;
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                float force = explosionForce * (1 - (distance / explosionRadius)); // Força diminui com a distância
                force = Mathf.Clamp(force, 10, force);

                rb.AddForce(direction * force, ForceMode2D.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public void ApplyEffect(Vector3 position, int damage)
    {
        TriggerExplosionManual(damage);
        // VFX_Manager.Instance.PlayExplosionEffect(position);
        // SFX_Manager.Instance.PlayExplosionSoundEffect();
    }
}
