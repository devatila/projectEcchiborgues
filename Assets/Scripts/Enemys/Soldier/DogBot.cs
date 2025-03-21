using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogBot : EnemyBase
{
    
    private Transform playerPos;

    private EnemyBasicsAttributes basicAttributes = new EnemyBasicsAttributes();
    private void Start()
    {
        playerPos = FindObjectOfType<PlayerInventory>().gameObject.transform;
        EnemyBasics.GetReferences(this);
        speed = 5f;

        EnemyBasics.agent.speed = speed;

    }

    public override void Move()
    {
        EnemyBasics.agent.SetDestination(playerPos.position);
    }

    private void Update()
    {
        Move();

        AjustarDirecao(EnemyBasics.agent, transform,ref ultimaPosicao);

        if (Input.GetKeyDown(KeyCode.L)) Debug.Log(IsFacingPlayer());
    }

    public override void TakeDamage(int damage, bool shouldPlayDamageAnim = true)
    {
        
    }

    


}
