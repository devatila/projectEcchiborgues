using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandThrowablePoolSystem : MonoBehaviour
{
    public static HandThrowablePoolSystem instance;
    
    //Serializadas só nesse inicio para fins de testes
    [SerializeField] private List<GameObject> granadePool = new List<GameObject>();
    [SerializeField] private List<GameObject> impactGranadePool = new List<GameObject>();
    [SerializeField] private List<GameObject> fragGranadePool = new List<GameObject>();
    [SerializeField] private List<GameObject> PEM_GranadePool = new List<GameObject>();
    [SerializeField] private List<GameObject> dynamitePool = new List<GameObject>();
    [SerializeField] private List<GameObject> molotovPool = new List<GameObject>();
    [SerializeField] private List<GameObject> throwingKnifePool = new List<GameObject>();
    [SerializeField] private List<GameObject> ninjaStarPool = new List<GameObject>();

    private Dictionary<ThroableObjects, List<GameObject>> throwablePools;

    public int startNumber;

    public GameObject granadePrefab;
    public GameObject impactGranadePrefab;
    public GameObject fragGranadePrefab;
    public GameObject PEM_GranadePrefab;
    public GameObject dynamitePrefab;
    public GameObject molotovPrefab;
    public GameObject throwingKnifePrefab;
    public GameObject ninjaStarPrefab;


    private void Awake()
    {
        instance = this;

        

        throwablePools = new Dictionary<ThroableObjects, List<GameObject>>
        {
            { ThroableObjects.Granade,         granadePool },
            { ThroableObjects.ImpactGranade,   impactGranadePool },
            { ThroableObjects.FragGranade,     fragGranadePool },
            { ThroableObjects.PEM_Granade,     PEM_GranadePool },
            { ThroableObjects.Dynamite,        dynamitePool },
            { ThroableObjects.Molotov,         molotovPool },
            { ThroableObjects.ThrowingKnife,   throwingKnifePool },
            { ThroableObjects.NinjaStar,       ninjaStarPool }
        };
    }

    public GameObject GetObject(ThroableObjects throwableType)
    {
        Debug.Log("Tipo de Arremessável escolhido é: " +  throwableType);
        if (!throwablePools.ContainsKey(throwableType))
        {
            Debug.LogWarning($"Nenhuma pool encontrada para o tipo: {throwableType}");
            return null;
        }

        foreach (GameObject g in throwablePools[throwableType])
        {
            if (!g.activeInHierarchy)
            {
                g.SetActive(true);
                return g;
            }
        }

        Debug.LogWarning($"Nenhum objeto disponível na pool para: {throwableType}");
        return InstanceNewObjectForPool(throwableType);
    }

    public GameObject InstanceNewObjectForPool(ThroableObjects throwableType)
    {
        Debug.Log("Faltou pool, instanciando mais um...");
        if (!throwablePools.ContainsKey(throwableType))
        {
            Debug.LogWarning($"Nenhuma pool encontrada para o tipo: {throwableType}");
            return null;
        }

        // Obtém um prefab base para o tipo de arremessável
        GameObject prefab = GetPrefabForType(throwableType);
        if (prefab == null)
        {
            Debug.LogWarning($"Prefab não encontrado para: {throwableType}");
            return null;
        }

        // Instancia um novo objeto e adiciona à pool
        GameObject newObject = Instantiate(prefab);
        newObject.SetActive(true);
        throwablePools[throwableType].Add(newObject); // Adiciona à lista correspondente

        return newObject;
    }

    private GameObject GetPrefabForType(ThroableObjects throwableType)
    {
        switch (throwableType)
        {
            case ThroableObjects.Granade: return granadePrefab;
            case ThroableObjects.ImpactGranade: return impactGranadePrefab;
            case ThroableObjects.FragGranade: return fragGranadePrefab;
            case ThroableObjects.PEM_Granade: return PEM_GranadePrefab;
            case ThroableObjects.Dynamite: return dynamitePrefab;
            case ThroableObjects.Molotov: return molotovPrefab;
            case ThroableObjects.ThrowingKnife: return throwingKnifePrefab;
            case ThroableObjects.NinjaStar: return ninjaStarPrefab;
            default: return null;
        }
    }

    public void ReturnObject(GameObject g)
    {
        g.SetActive(false);
    }
}
