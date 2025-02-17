using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorScript : MonoBehaviour
{
    public GameObject indicatorIcon;
    public GameObject target;

    Renderer rd;
    void Start()
    {
        rd = GetComponent<Renderer>();
        target = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        if (rd.isVisible == false)
        {
            if(indicatorIcon.activeSelf == false)
            {
                indicatorIcon.SetActive(true);
            }

            Vector2 direction = target.transform.position - transform.position;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
            if(hit.collider != null)
            {
                indicatorIcon.transform.position = hit.point;
            }
        }
        else
        {
            if (indicatorIcon.activeSelf == true)
            {
                indicatorIcon.SetActive(false);
            }
        }
    }
}
