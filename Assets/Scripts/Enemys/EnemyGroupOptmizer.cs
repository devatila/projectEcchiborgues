using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupOptmizer : MonoBehaviour
{
    public float rangeDistance; //Distancia para ativar os inimigos
    private List<GameObject> enemys = new List<GameObject>();
    private bool IsOnRange = false;
    private LayerMask player;


    void Start()
    {
        IsOnRange = false;
        player = 1 << LayerMask.NameToLayer("Player");
        foreach(Transform child in transform)
        {
            enemys.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, rangeDistance);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOnRange)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, rangeDistance, player);
            if (hit != null)
            {
                IsOnRange = true;
                ShowEnemies();
            }
        }
    }

    void ShowEnemies()
    {
        foreach(GameObject g in enemys)
        {
            g.SetActive(true);
        }
    }
}
