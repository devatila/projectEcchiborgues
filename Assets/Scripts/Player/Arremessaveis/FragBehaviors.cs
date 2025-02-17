using UnityEngine;


public class FragBehaviors : MonoBehaviour
{
    public float speed = 10f;  // Velocidade do fragmento
    public int maxBounces = 3; // Quantidade máxima de ricochetes
    public float lifeTime = 3f; // Tempo antes de ser desativado
    public LayerMask collisionMask; // Máscara para colisão

    public int damage { get; set; }

    private Vector2 moveDirection;
    [SerializeField] private int bounceCount;
    private float spawnTime;

    void OnEnable()
    {
        spawnTime = Time.time;
        bounceCount = 0;

        float angle = Random.Range(0, 360);
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        Initialize(direction);
    }

    public void Initialize(Vector2 direction)
    {
        transform.localPosition = Vector3.zero;
        moveDirection = direction.normalized;
        spawnTime = Time.time;
        bounceCount = 0;
    }
    private void Start()
    {
        Physics2D.queriesStartInColliders = false;
    }

    void Update()
    {
        if (Time.time - spawnTime > lifeTime)
        {
            gameObject.SetActive(false); // Desativa após tempo limite
        }

        Vector2 currentPosition = (Vector2)transform.position;
        Vector2 newPosition = currentPosition + moveDirection * speed * Time.deltaTime;

        // Raycast para detectar colisões
        RaycastHit2D hit = Physics2D.CircleCast(currentPosition, 0.1f, moveDirection, Vector2.Distance(currentPosition, newPosition), collisionMask);

        if (hit.collider != null)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if(damageable != null)
            {
                damageable.TakeDamage(damage);
                gameObject.SetActive(false);
            }

            if (bounceCount < maxBounces)
            {
                // Calcula reflexão do fragmento
                moveDirection = Vector2.Reflect(moveDirection, hit.normal);
                bounceCount++;
            }
            else
            {
                gameObject.SetActive(false); // Desativa ao atingir o limite de ricochetes
            }
        }

        // Move o fragmento
        transform.position = newPosition;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Vector2 currentPosition = transform.position;
        Vector2 newPosition = currentPosition + moveDirection * speed * Time.deltaTime;

        Gizmos.color = Color.red; // Linha do cast
        Gizmos.DrawLine(currentPosition, newPosition);

        Gizmos.color = Color.yellow; // Origem do CircleCast
        Gizmos.DrawWireSphere(currentPosition, 0.1f);

        Gizmos.color = Color.green; // Destino do CircleCast
        Gizmos.DrawWireSphere(newPosition, 0.1f);
    }


}
