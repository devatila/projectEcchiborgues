using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//                                                            -- A T E N Ç Ã O --                                                           //
// ESSA CLASSE ENGLOBA O BEHAVIOR DE GRANADE, IMPACT GRANADE, FRAG GRANADE, PEM GRANADE E MOLOTOV //
//                                                                                                                                          //
public class GranadeObject : MonoBehaviour, IThrowable
{
    public ThrowablesSO throwableData;
    public bool canBeShooted = true;
    public int damage;
    public float speed;
    public float delayToExplode; // Se for Impact Granade, é só por = 0

    public bool autoDeactivate = true;
    public bool instaActivate;
    public bool isContinuous;
    public bool isBounceable;
    public int maxBounce = 3;
    private int bounceCount;

    private bool isActive;

    public Rigidbody2D rb;
    private Vector3 target;

    private bool _arrived;
    private bool arrived { get { return _arrived; } set {
        if(_arrived != value)
            {
                _arrived = value;
                //Debug.Log("Alterou para " + value);
                if(_arrived == true) ExecuteEvent();
            }
        } }

    private Vector2 lastVelocity;

    //public float lifetime = 2f;

    public LayerMask layerToCollide;

    private IThrowableEffect throwableEffect;

    private void Start()
    {
        throwableEffect = GetComponent<IThrowableEffect>();
        //GetDataFromSO(throwableData);
        //throwableEffect.SetThrowableData(throwableData);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Verifica se há um destino definido
        if (target == Vector3.zero || arrived) return;

        // Calcula a direção para o destino
        Vector2 direction = ((Vector2)target - rb.position).normalized;

        if (!isContinuous)
        {
            // Calcula a distância restante
            float distance = Vector2.Distance(transform.position, target);

            // Se a distância for maior que o limite, continua o movimento
            if (distance > 0.8f)
            {
                rb.velocity = direction * speed; // Define a velocidade
            }
            else
            {
                rb.velocity = Vector2.zero; // Para o movimento ao alcançar o destino
                arrived = true;
            }
        }
        lastVelocity = rb.velocity;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(target, 2f);
    }

    public void SetTargetObject(Vector3 target)
    {
        this.target = target;
    }
    public void ThrowObject(Vector3 mousePosition, Vector3 launchPos)
    {
        this.target = mousePosition;
        arrived = false;

        transform.position = launchPos;

        Vector3 dir = launchPos - mousePosition;
        dir.Normalize();
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,angle);

        if (!isContinuous) return;
        StartCoroutine(LifetimeCountdown(2));
        // Calcula a direção apenas uma vez ao ativar o objeto
        Vector2 moveDirection = ((Vector2)target - (Vector2)transform.position).normalized;

        // Aplica a velocidade inicial
        rb.velocity = moveDirection * speed;


    }
    public void SetDamage(int newDamage)
    {
        this.damage = newDamage;
    }
    public void OnHitObject()
    {
        if (!canBeShooted) return;
        throwableEffect.ApplyEffect(null, damage); 
        HandThrowablePoolSystem.instance.ReturnObject(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Tocou em algo");
        if (instaActivate) return;
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            throwableEffect.ApplyEffect(collision.gameObject, damage);
            if (autoDeactivate) HandThrowablePoolSystem.instance.ReturnObject(this.gameObject);
        }

        if (!isBounceable) arrived = true;
        else
        {
            if (bounceCount >= maxBounce)
            {
                arrived = true;
                return;
            } // Para de ricochetear se atingir o limite

            // Obtém a normal da colisão (direção perpendicular à superfície)
            Vector2 normal = collision.contacts[0].normal;

            // Calcula a nova direção refletida
            Vector2 newDirection = Vector2.Reflect(lastVelocity, normal).normalized;

            // Aplica a nova velocidade mantendo a magnitude anterior
            rb.velocity = newDirection * speed;

            // Ajusta a rotação para apontar na direção do movimento
            float angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            bounceCount++;
        }
    }

    

    private void ExecuteEvent()
    {
        StartCoroutine(TimeToExecute(delayToExplode));
    }

    private IEnumerator TimeToExecute(float timer)
    { // VOLTOOOOOU
        yield return new WaitForSeconds(timer);
 
        throwableEffect.ApplyEffect(null ,damage); // Chama a interface mandando a posição e a quantidade de dano aplicado
        if (autoDeactivate) HandThrowablePoolSystem.instance.ReturnObject(this.gameObject); // Desativa o objeto
        yield break;
    }

    private void OnEnable()
    {
        // Certifica que a variavel volte ao seu estado normal quando o objeto aparecer
        arrived = instaActivate;
        

        if (isContinuous) bounceCount = 0;
    }

    IEnumerator LifetimeCountdown(float time)
    {
        yield return new WaitForSeconds(time);
        HandThrowablePoolSystem.instance.ReturnObject(this.gameObject);
    }

    void GetDataFromSO(ThrowablesSO data)
    {
        canBeShooted = data.canBeShooted;
        damage = data.damage;
        speed = data.speed;
        delayToExplode = data.delayToExplode;

        autoDeactivate = data.autoDeactivate;
        instaActivate = data.instaActivate;
        isContinuous = data.isContinuous;
        isBounceable = data.isBounceable;
        maxBounce = 3;
    }
}
