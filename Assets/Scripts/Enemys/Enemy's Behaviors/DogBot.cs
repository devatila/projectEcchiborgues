using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogBot : EnemyBase
{
    // FIQUE AQUIIII (STAY WITH ME)
    // A NOITE NA SUA PORTA EU BATIII

    [Space(20)]
    public bool HasExplode;
    private DogBotAttacks currentAttack;
    private Dictionary<EnemyAttackTypes.DogBot, DogBotAttacks> dogBotAttackCache = new();

    public override void Start()
    {
        base.Start();
        
        //SetAttackType(DogBotAttacks.AttackTypes.Bite, 150);
        //SetGenericAttackType(EnemyAttackTypes.DogBot.Bite, 150);
        
    }

    void SetAttackType(EnemyAttackTypes.DogBot attackType, int damage)
    {
        if (!dogBotAttackCache.TryGetValue(attackType, out var cachedAttack))
        {
            cachedAttack = new DogBotAttacks(attackType, damage, this);
            dogBotAttackCache[attackType] = cachedAttack;
        }

        // Cancela ataque atual se for diferente
        if (currentAttack != cachedAttack) currentAttack?.CancelAttacks();

        currentAttack = cachedAttack;
        if (!currentAttack.canAttack)
        {
            currentAttack.canAttack = true;
            Debug.Log("O estado de ataque foi corrigido para: " + currentAttack.canAttack);
        }
        enemyAttack = currentAttack;
    }


    public override void SetGenericAttackType<T>(T attackType, int damage, int probability = 100)
    {
        base.SetGenericAttackType(attackType, damage);
        if (attackType is EnemyAttackTypes.DogBot enumAttacks)
        {
            SetAttackType(enumAttacks, damage);
            currentAttack.currentProbability = probability;
            attackAllowanceByProbability = currentAttack.CanAttackWithProbabilites(probability);
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
        
        if (CanPerformAttack())
        {
            currentAttack.ExecuteAttack(transform);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            NewApplyNaturalState(NaturalStates.Eletric, 3f); // isso aqui vai aplicar o efeito de "Stun"
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            NewApplyNaturalState(NaturalStates.Fire, 10f); // isso aqui vai aplicar o efeito de "Stun"
        }
    }

    


}
