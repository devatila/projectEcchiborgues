using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_PoolManager : MonoBehaviour
{

    public static VFX_PoolManager instance;
    [SerializeField] private VfxClass VFX_Handler;

    private void Awake()
    {
        instance = this;
    }

    public void PlayGenericExplosion(Vector3 DesiredPos)
    {

    }
    public void PlayFragExplosion(Vector3 DesiredPos, bool canRicochete = false)
    {
        foreach (GameObject g in VFX_Handler.Pool_FragExplosion)
        {
            if (!g.activeInHierarchy)
            {
                g.SetActive(true);
                g.transform.position = DesiredPos;
            }
        }
    }
    public void PlayFlameExplosion(Vector3 DesiredPos, float radius = 5f)
    {

    }
    public void PlayPEM_Effect(Vector3 desriedPos, float duration = 5f)
    {
        Debug.Log("Só as mágoas do que poderia ser um efeito de PEM");
    }

    public void PlaySmokeEffect(Vector3 DesiredPos, float duration = 20f, float radius = 5f)
    {

    }
}

[System.Serializable]
public class VfxClass
{
    public GameObject VFX_GenericExplosion;
    public GameObject VFX_FragExplosion;
    public GameObject VFX_Molotov;
    public GameObject VFX_PEM;
    public GameObject VFX_Smoke;

    public List<GameObject> Pool_GenericExplosion = new List<GameObject>();
    public List<GameObject> Pool_FragExplosion = new List<GameObject>();
    public List<GameObject> Pool_Molotov = new List<GameObject>();
    public List<GameObject> Pool_PEM = new List<GameObject>();
    public List<GameObject> Pool_Smoke = new List<GameObject>();

    public GameObject InstantiateAndRegisterVFXsInPool(GameObject VFX_Object, ref List<GameObject> desiredPool)
    {
        GameObject g = MonoBehaviour.Instantiate(VFX_Object);
        g.SetActive(false);
        desiredPool.Add(g);
        return g;
    }
}
