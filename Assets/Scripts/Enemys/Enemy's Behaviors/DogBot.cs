using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogBot : EnemyBase
{
    
    [Space(20)]
    public bool HasExplode;
    private DogBotAttacks currentAttack;
    public override void Start()
    {
        base.Start();
        //SetAttackType(DogBotAttacks.AttackTypes.Bite, 150);
        SetGenericAttackType(DogBotAttacks.AttackTypes.Bite, 150);
        
    }

    void SetAttackType(DogBotAttacks.AttackTypes attackType, int damage)
    {
        currentAttack = new DogBotAttacks(attackType, damage, this);
    }

    public override void SetGenericAttackType<T>(T attackType, int damage)
    {
        base.SetGenericAttackType(attackType, damage);
        if (attackType is DogBotAttacks.AttackTypes enumAttacks)
        {
            SetAttackType(enumAttacks, damage);
        }
        
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
        if (isPlayerOnAttackRange)
        {
            currentAttack.ExecuteAttack(transform);
        }
    }

    


}
