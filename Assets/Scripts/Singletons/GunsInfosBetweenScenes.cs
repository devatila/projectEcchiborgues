using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunsInfosBetweenScenes : MonoBehaviour
{
    public List<GameObject> ImaginaUmaArmaAqui;
    public int IdGun;

    public static GunsInfosBetweenScenes instance { get; private set; } 

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
