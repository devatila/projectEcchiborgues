using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogBot : EnemyBase
{
    private NavMeshAgent agent;
    private Transform playerPos;
    private void Start()
    {
        playerPos = FindObjectOfType<PlayerInventory>().gameObject.transform;
        agent = GetComponent<NavMeshAgent>();
        speed = 5f;
        ultimaPosicao = transform.position;
        agent.speed = speed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

    }

    public override void Move()
    {
        agent.SetDestination(playerPos.position);
    }

    private void Update()
    {
        Move();

        AjustarDirecao(agent, transform,ref ultimaPosicao);
    }

    public override void TakeDamage(int damage, bool shouldPlayDamageAnim = true)
    {
        
    }

    


}
