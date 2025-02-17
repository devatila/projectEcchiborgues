using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage;
    public float speed;
    public float lifettime = 2f;
    private Rigidbody2D rb;
    private GameObject obj;
    private PoolEnemyProjectiles poolEnemyProjectiles;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        poolEnemyProjectiles = FindObjectOfType<PoolEnemyProjectiles>();
        obj = this.gameObject;
    }

    private void OnEnable()
    {
        Invoke("Deactivate", lifettime);
    }

    void Deactivate()
    {
        if (obj != null)
        {
            poolEnemyProjectiles.ReturnObject(obj);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(rb.transform.position + transform.right * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPlayableCharacter c = collision.GetComponent<IPlayableCharacter>();
        if (c != null)
        {
            c.TakeDamage(damage);
            Deactivate();
        }
    }
}
