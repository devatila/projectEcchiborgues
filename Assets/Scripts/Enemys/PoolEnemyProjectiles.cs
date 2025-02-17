using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolEnemyProjectiles : MonoBehaviour
{
    public GameObject projectile;
    public int poolSize;

    private List<GameObject> pool = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = Instantiate(projectile);
            go.SetActive(false);
            pool.Add(go);
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
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
