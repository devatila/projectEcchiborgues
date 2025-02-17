using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurretProjectileBehavior : MonoBehaviour
{
    public float speed;
    public float lifetime = 3f; // Em Segundos
    public Rigidbody2D rb;
    private GameObject obj;
    private int damage;

    public event Action<Collider2D, int> OnHitOrTimeTrigger;
    public LayerMask layerToDetect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        obj = this.gameObject;
    }

    private void OnEnable()
    {
        Invoke("Deactivate", lifetime);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.transform.position + transform.right * speed * Time.fixedDeltaTime);

        Collider2D hit = Physics2D.OverlapCircle(transform.position, transform.localScale.x, layerToDetect);
        if (hit != null)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && OnHitOrTimeTrigger == null)
            {
                damageable.TakeDamage(damage);
                OnHitOrTimeTrigger?.Invoke(hit, damage);
                CancelInvoke();
                Deactivate();
            }

            
        }
    }
    void Deactivate()
    {
        if (obj != null)
        {
            TurretProjectilesPoolingManager.Instance.ReturnObject(obj);
        }
    }

    public void GetDamageData(int damageValue)
    {
        damage = damageValue;
    }

    
}
