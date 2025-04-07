using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogBotAttacks : EnemyAttack
{
    public enum AttackTypes {Bite, RushHead, TripleBite } // RushHead é uma tentativa minha de dizer que é aquelas investidas do pokemon
    private AttackTypes attackType;
    private int m_damage;
    private float delayBetweenAttacks = 2f;
    private Coroutine actualCoroutine;
    private bool canAttack = true;
    private EnemyBase m_enemy;

    public DogBotAttacks(AttackTypes type, int damage, EnemyBase enemy)
    {
        attackType = type;
        m_damage = damage;
        m_enemy = enemy;
    }

    public override void ExecuteAttack(Transform enemyTransform)
    {
        if (!canAttack) return;

        switch (attackType)
        {
            case AttackTypes.Bite:
                canAttack = false;
                actualCoroutine = CoroutineRunner.Instance.StartCoroutine(BiteRoutine());
                break;

            case AttackTypes.RushHead:
                canAttack = false;
                Debug.Log($"{enemyTransform.name} fez uma INVESTIDA causando {m_damage} de dano!");
                canAttack = true;
                break;
        }
    }

    public override void CancelAttacks()
    {
        CoroutineRunner.Instance.StopCoroutine(actualCoroutine);
    }

    IEnumerator BiteRoutine()
    {
        
        Debug.Log($"DogBot fez uma MORDIDA causando {m_damage} de dano!");

        yield return new WaitForSeconds(delayBetweenAttacks);

        canAttack = true;
    }
}
