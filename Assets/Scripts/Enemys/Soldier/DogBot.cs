using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogBot : EnemyBase
{
    
    [Space(20)]
    public bool HasExplode;
    public override void Start()
    {
        base.Start();
        
        
    }

    public override void TakeDamage(int damage, bool shouldPlayDamageAnim = true)
    {
        Debug.Log($"tomou {damage} de dano, e o objeto tocar animação é igual a {shouldPlayDamageAnim}");
    }

    public override void Move()
    {
        //EnemyBasics.agent.SetDestination(playerPos.position);
    }

    public override void Update()
    {
        base.Update();

    }

    


}
