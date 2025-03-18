using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogBot : EnemyBase
{
    private NavMeshAgent agent;
    private Transform playerPos;
    private Vector2 ultimaPosicao;
    private void Start()
    {
        playerPos = FindObjectOfType<PlayerInventory>().gameObject.transform;
        agent = GetComponent<NavMeshAgent>();
        speed = 5f;
        agent.speed = speed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        Calcular(5, 7);
        ultimaPosicao = transform.position;
    }

    public override void Move()
    {
        agent.SetDestination(playerPos.position);
    }

    private void Update()
    {
        Move();

        AjustarDirecao();
    }

    public override void TakeDamage(int damage, bool shouldPlayDamageAnim = true)
    {
        
    }

    void AjustarDirecao()
    {
        Vector2 direcaoMovimento = (Vector2)transform.position - ultimaPosicao;

        if (direcaoMovimento.x > 0.001f)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // Direita
        }
        else if (direcaoMovimento.x < -0.001f)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // Esquerda
        }

        ultimaPosicao = transform.position;
    }


}
