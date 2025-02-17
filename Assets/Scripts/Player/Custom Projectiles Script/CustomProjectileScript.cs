using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class CustomProjectileScript : MonoBehaviour
{
    private bool _isActive;
    public bool isActive { get { return _isActive; } set { if (_isActive != value) { _isActive = value; OnActive(); } } }
    public bool startActive;


    private Rigidbody2D rb;
    public float lifetime = 1.5f;
    public float range = 1f;
    public float speed = 10f;
    public bool invertXDirection = true;
    private int directionIndex;
    public LayerMask layer;

    public int customID;
    private bool isOnRight;

    public event Action<Collider2D, int> OnHitCollides;
    public int localDamage { get; set;}

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        directionIndex = invertXDirection ? 1 : -1;

        if (startActive)
        {
            isActive = startActive;
        }
    }

    private void FixedUpdate()
    {
        if(!isActive) return;
        //OnActive();
        rb.MovePosition(rb.transform.position + transform.right * (speed * directionIndex) * Time.fixedDeltaTime);
        CheckAnyTouch();
    }

    void CheckAnyTouch()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, range, layer);
        if(hit != null)
        {
            OnHitCollides?.Invoke(hit, localDamage);
            isActive = false;
            CancelInvoke();
            this.gameObject.SetActive(false);
        }
           

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    void OnActive()
    {
        if (!isActive) return;
        Invoke("AutoExplosion", lifetime);
        
    }
    void AutoExplosion()
    {
        this.gameObject.SetActive(false);
        isActive = false;
        OnHitCollides?.Invoke(null, localDamage);
    }

}
