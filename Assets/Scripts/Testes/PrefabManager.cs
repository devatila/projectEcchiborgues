using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public GameObject[] prefabs;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            GunsInfosBetweenScenes.instance.ImaginaUmaArmaAqui.Add(prefabs[i]);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
