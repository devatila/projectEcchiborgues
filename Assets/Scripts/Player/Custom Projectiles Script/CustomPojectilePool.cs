using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPojectilePool : MonoBehaviour
{
    public static CustomPojectilePool instance;

    public List<CustomProjectilesData> projectiles;
    public int startPoolSize = 10;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    public void RegisterOnPool(GameObject projectile,ref int cID)
    {
        CustomProjectilesData c = new CustomProjectilesData();
        projectiles.Add(c);

        c.projectileID = projectiles.Count - 1;
        cID = c.projectileID;
        c.projectilePrefab = projectile;

        for (int i = 0; i < startPoolSize; i++)
        {
            GameObject p = Instantiate(c.projectilePrefab);
            c.pool.Add(p);
            p.transform.localScale = c.projectilePrefab.transform.lossyScale;
            p.transform.parent = transform;
            p.SetActive(false);
        }
    }

    public bool CheckAlreadyExists(int id)
    {
        foreach (CustomProjectilesData p in projectiles)
        {
            if (p.projectileID == id)
            {
                return true;
            }
            
        }
        return false;
    }

    public GameObject GetObject(int id)
    {
        foreach (GameObject obj in projectiles[id].pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;

            }
        }
        return null;
    }
}

[System.Serializable]
public class CustomProjectilesData
{
    public int projectileID;
    public GameObject projectilePrefab;
    public List<GameObject> pool = new List<GameObject>();
}
