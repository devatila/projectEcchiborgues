using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverListener : MonoBehaviour
{
    private GameObject thisGameObject;
    private PlayerHealth player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerHealth>();
        thisGameObject = GetComponentInChildren<Transform>().gameObject;
        thisGameObject.SetActive(false);

        player.OnDeath += ActivateItSelf;
    }

    void ActivateItSelf()
    {
        thisGameObject.SetActive(true);
    }
}
