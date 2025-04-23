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

        if (!canAttack || isRunning) return;


        Debug.Log(attackType);
        switch (attackType)
        {
            case EnemyAttackTypes.DogBot.Bite:

                if (!isRunning) CoroutineRunner.Instance.StartCoroutine(BiteRoutine());
                break;

            case EnemyAttackTypes.DogBot.Dash:
                canAttack = false;
                Debug.Log($"{enemyTransform.name} fez uma INVESTIDA causando {m_damage} de dano!");
                canAttack = true;
                break;

            case EnemyAttackTypes.DogBot.TripleBite:
                if (!isRunning) CoroutineRunner.Instance.StartCoroutine(TripleBiteRoutine());
                break;
        }
    }

    public override void CancelAttacks()
    {
        if (actualCoroutine != null)
        {
            CoroutineRunner.Instance.StopAllCoroutines();
            actualCoroutine = null;
        }
            
    }

    IEnumerator BiteRoutine()
    {
        isRunning = true;
        
        Debug.Log($"DogBot fez uma MORDIDA causando {m_damage} de dano! {actualCoroutine != null}");

        yield return new WaitForSeconds(delayBetweenAttacks);

        // Debug.Log("Pronto para atacar novamente");

        canAttack = true;
        isRunning = false;
        actualCoroutine = null;
    }

    IEnumerator TripleBiteRoutine ()
    {
        isRunning = true;
        canAttack = false;
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("Atacando com MORDIDA TRIPLA, o ataque index é de: " + (i + 1));
            yield return new WaitForSeconds(delayBetweenAttacks / 2); 
        }
        // Debug.Log("Todos os ataques triplos performados");

        yield return new WaitForSeconds (delayBetweenAttacks);
        canAttack = true;
        actualCoroutine = null;
        isRunning = false;
    }
}
