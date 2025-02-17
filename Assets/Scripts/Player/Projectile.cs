using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Pool_Projectiles pool;
    public int damage;
    public float lifetime;
    private GameObject gobj;
    [SerializeField] private GameObject GunPrefab;
    void Start()
    {
        GunPrefab = GameObject.FindGameObjectWithTag("PoolManager");
        pool = GunPrefab.GetComponent<Pool_Projectiles>();
        gobj = this.gameObject;
    }

    private void OnEnable()
    {
        Invoke("Deactivate", lifetime);
    }
    // Update is called once per frame
    void Deactivate()
    {
        if(gobj != null)
            pool.ReturnObject(gobj);
    }

    private void OnTriggerEnter2D(Collider2D collision) //Não esquecer de trocar para Physics2D.OverlapCircle
    {
        IDamageable dmg = collision.gameObject.GetComponent<IDamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage);
        }

        IThrowable throwable = collision.GetComponent<IThrowable>();
        if(throwable != null)
        {
            throwable.OnHitObject();
        }
    }

}
