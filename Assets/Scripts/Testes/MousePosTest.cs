using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 10;
        Debug.Log(mousePosition);
    }
}
