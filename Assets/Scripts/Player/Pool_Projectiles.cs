using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool_Projectiles : MonoBehaviour
{
    public GameObject projectilePrefab, feixe, flashMuzzle, capsules;
    public int poolSize;
    public int capsulesPoolSize = 60;

    private List<GameObject> pool;
    private List<GameObject> poolFlashes;
    private List<GameObject> poolCapsules = new List<GameObject>();
    public List<GameObject> guns;
    private void Awake()
    {
        
        pool = new List<GameObject>();
        poolFlashes = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab); 
            obj.SetActive(false);
            pool.Add(obj);

            GameObject fx = Instantiate(feixe);
            fx.SetActive(false);
            poolFlashes.Add(fx);
        }
        for (int i = 0; i < capsulesPoolSize; i++)
        {
            GameObject c = Instantiate(capsules, transform);
            c.SetActive(false);
            poolCapsules.Add(c);
        }
        
    }

    private void Start()
    {
        GameObject[] g = GameObject.FindGameObjectsWithTag("ShooterGun");
        for (int i = 0; i < g.Length; i++)
        {
            guns.Add(g[i]);
        }
    }

    public GameObject GetObject()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;

            }
        }
        return null;
    }

    public GameObject GetFlash()
    {
        foreach (GameObject obj in poolFlashes)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;

            }
        }
        return null;
    }

    public GameObject GetCapsules()
    {
        foreach(GameObject obj in poolCapsules)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        return null;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
