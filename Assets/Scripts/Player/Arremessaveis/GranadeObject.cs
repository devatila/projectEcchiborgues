using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//                                                            -- A T E N Ç Ã O --                                                           //
                     // ESSA CLASSE ENGLOBA O BEHAVIOR DE GRANADE, IMPACT GRANADE, FRAG GRANADE, PEM GRANADE E MOLOTOV //
//                                                                                                                                          //
public class GranadeObject : MonoBehaviour, IThrowable
{
    public bool canBeShooted = true;
    public int damage;
    public float speed;
    public float delayToExplode; // Se for Impact Granade, é só por = 0
    private bool isActive;

    public Rigidbody2D rb;
    private Vector3 target;

    private bool _arrived;
    private bool arrived { get { return _arrived; } set {
        if(_arrived != value)
            {
                _arrived = value;
                Debug.Log("Alterou para " + value);
                if(_arrived == true) ExecuteEvent();
            }
        } }

    //public float lifetime = 2f;

    public LayerMask layerToCollide;

    private IThrowableEffect throwableEffect;

    private void Start()
    {
        throwableEffect = GetComponent<IThrowableEffect>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        // Verifica se há um destino definido
        if (target == Vector3.zero || arrived) return;

        // Calcula a direção para o destino
        Vector2 direction = ((Vector2)target - rb.position).normalized;

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

        

    }
    public void SetDamage(int newDamage)
    {
        this.damage = newDamage;
    }
    public void OnHitObject()
    {
        if (!canBeShooted) return;
        throwableEffect.ApplyEffect(transform.position, damage); 
        HandThrowablePoolSystem.instance.ReturnObject(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Tocou em algo");
        arrived = true;
    }

    private void ExecuteEvent()
    {
        StartCoroutine(TimeToExecute(delayToExplode));
    }

    private IEnumerator TimeToExecute(float timer)
    { // VOLTOOOOOU
        yield return new WaitForSeconds(timer);
 
        throwableEffect.ApplyEffect(transform.position ,damage); // Chama a interface mandando a posição e a quantidade de dano aplicado
        HandThrowablePoolSystem.instance.ReturnObject(this.gameObject); // Desativa o objeto
        yield break;
    }

    private void OnEnable()
    {
        // Certifica que a variavel volte ao seu estado normal quando o objeto aparecer
        arrived = false;
    }
}
