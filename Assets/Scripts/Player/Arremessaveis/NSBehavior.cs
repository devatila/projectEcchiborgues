using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NSBehavior : MonoBehaviour
{
    public bool canBounce { get; set; }
    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
    }

    private Rigidbody2D rb;
    private Vector2 lastVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        lastVelocity = rb.velocity;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canBounce)
        {
            gameObject.SetActive(false);
            return;
        }
        // Obt�m a normal da colis�o (dire��o perpendicular � superf�cie)
        Vector2 normal = collision.contacts[0].normal;

        // Calcula a nova dire��o refletida
        Vector2 newDirection = Vector2.Reflect(lastVelocity, normal).normalized;

        // Aplica a nova velocidade mantendo a magnitude anterior
        rb.velocity = newDirection * 35f;
    }
}
