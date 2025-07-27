using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyOnSceneManager : MonoBehaviour
{
    public List<EnemyPool> enemyPools = new List<EnemyPool>();

    public static EnemyOnSceneManager instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject GetEnemy(GameObject enemyPrefab)
    {
        // Verifica se já existe uma pool para esse prefab
        EnemyPool pool = enemyPools.Find(p => p.enemyPrefabToInstantiate == enemyPrefab);

        // Se não existir, cria e adiciona uma nova pool
        if (pool == null)
        {
            pool = new EnemyPool(enemyPrefab);
            enemyPools.Add(pool);
        }

        // Pede um inimigo da pool (instanciado ou reutilizado)
        return pool.GetEnemyFromPool();
    }
}

[System.Serializable]
public class EnemyPool
{
    public GameObject enemyPrefabToInstantiate;
    public List<GameObject> thisEnemyPool;

    public EnemyPool(GameObject prefab)
    {
        enemyPrefabToInstantiate = prefab;
        thisEnemyPool = new List<GameObject>();
    }

    public GameObject GetEnemyFromPool()
    {
        // Procura um inimigo inativo na pool
        foreach (GameObject enemy in thisEnemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }

        // Nenhum inativo? Instancia novo
        return InstantiateMore();
    }

    public GameObject InstantiateMore()
    {
        GameObject newEnemy = GameObject.Instantiate(enemyPrefabToInstantiate);
        newEnemy.SetActive(true);
        thisEnemyPool.Add(newEnemy);
        return newEnemy;
    }
}

