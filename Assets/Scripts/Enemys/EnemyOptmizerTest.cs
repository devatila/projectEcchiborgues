using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOptmizerTest : MonoBehaviour
{
    public SpriteRenderer[] enemySprites;
    public bool show;
    
    void Start()
    {
        enemySprites = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        ChangeActive();
    }

    void ChangeActive()
    {
        if (show)
        {
            foreach (var sprites in enemySprites)
            {
                sprites.enabled = true;
            }
        }
        else 
        {
            foreach(var sprites in enemySprites)
            {
                sprites.enabled = false;
            }
        }
    }
}
