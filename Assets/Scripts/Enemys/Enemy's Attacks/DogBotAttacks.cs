using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogBotAttacks : EnemyAttack
{
    private EnemyAttackTypes.DogBot attackType;
    private int m_damage;
    private float delayBetweenAttacks = 2f;
    public Coroutine actualCoroutine;
    private EnemyBase m_enemy;
    public int currentProbability;

    public DogBotAttacks(EnemyAttackTypes.DogBot type, int damage, EnemyBase enemy)
    {
        attackType = type;
        m_damage = damage;
        m_enemy = enemy;
    }
    public EnemyAttackTypes.DogBot CurrentAttackType;

    public override void ExecuteAttack(Transform enemyTransform)
    {
        if (!canAttack) return;
        switch (attackType)
        {
            case EnemyAttackTypes.DogBot.Bite:

                if (actualCoroutine == null) actualCoroutine = CoroutineRunner.Instance.StartCoroutine(BiteRoutine());
                break;

            case EnemyAttackTypes.DogBot.Dash:
                canAttack = false;
                Debug.Log($"{enemyTransform.name} fez uma INVESTIDA causando {m_damage} de dano!");
                canAttack = true;
                break;
        }
    }

    public override void CancelAttacks()
    {
        if (actualCoroutine != null)
        {
            CoroutineRunner.Instance.StopCoroutine(actualCoroutine);
            actualCoroutine = null;
        }
            
    }

    IEnumerator BiteRoutine()
    {
        canAttack = false;
        Debug.Log($"DogBot fez uma MORDIDA causando {m_damage} de dano!");

        yield return new WaitForSeconds(delayBetweenAttacks);

        Debug.Log("Pronto para atacar novamente");

        canAttack = true;
        actualCoroutine = null;
    }
}
