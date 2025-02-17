using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEreceiver : MonoBehaviour
{
    public UnityE_declaire unityE_Declaire;
    public UnityEvent propertie;
    // Start is called before the first frame update
    void Start()
    {
        propertie = unityE_Declaire.unityEventTest;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
