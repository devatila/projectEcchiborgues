using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogBot : EnemyBase
{
    
    private Transform playerPos;
    public override void Start()
    {
        base.Start();
        playerPos = FindObjectOfType<PlayerInventory>().gameObject.transform;
        EnemyBasics.GetReferences(this);
        speed = 5f;

        EnemyBasics.agent.speed = speed;

    }

    public override void TakeDamage(int damage, bool shouldPlayDamageAnim = true)
    {
        Debug.Log($"tomou {damage} de dano, e o objeto tocar animação é igual a {shouldPlayDamageAnim}");
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

    


}
