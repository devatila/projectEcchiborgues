using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectilesPoolingManager : MonoBehaviour
{
    public static TurretProjectilesPoolingManager Instance; //Singleton da pool
    public TurretProjectilesDataForPools[] Pools;
    public int startPoolSize = 10;

    private GameObject poolOrganizer;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        poolOrganizer = new GameObject("Turret Projectiles Organizer");
    }

    public void SetMoreProjectilesInPool(int projectileID)
    {
        for (int i = 0; i < startPoolSize; i++)
        {
            GameObject obj = Instantiate(Pools[projectileID].projectilePrefab, poolOrganizer.transform);
            obj.SetActive(false);
            Pools[projectileID].projectilePool.Add(obj);
        }
    }

    public GameObject GetDesiredProjectile(int projectileId)
    {
        foreach (GameObject obj in Pools[projectileId].projectilePool)
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

[System.Serializable]
public class TurretProjectilesDataForPools
{
    public GameObject projectilePrefab;
    public List<GameObject> projectilePool = new List<GameObject>();
}
