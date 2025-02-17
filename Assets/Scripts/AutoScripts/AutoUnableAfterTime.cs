using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AutoUnableAfterTime : MonoBehaviour
{
    private Pool_Projectiles pool;
    public float lifetime;
    private GameObject obj;
    void Start()
    {
        pool = GameObject.FindObjectOfType(typeof(Pool_Projectiles)).GetComponent<Pool_Projectiles>();
        obj = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        Invoke("Deactivate", lifetime);
    }
    // Update is called once per frame
    void Deactivate()
    {
        if(obj != null)
            pool.ReturnObject(obj);
    }
}
