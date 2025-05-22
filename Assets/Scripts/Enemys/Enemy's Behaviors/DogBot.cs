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
    private Dictionary<string, DogBotAttacks> attackCache = new Dictionary<string, DogBotAttacks>();
    private string objectName;

    

    public override void Start()
    {
        base.Start();
        // Configura as zonas de ataque (exemplo)
        objectName = this.gameObject.name;
        EnemyAttackZone[] zones = GetComponentsInChildren<EnemyAttackZone>();

        // Zona 1: Bite e TripleBite (raio menor)
        if (zones.Length > 0)
        {
            zones[0].GetAttacks().AddRange(new[]
            {
                new EnemyAttackZone.AttackConfig { attackType = "Bite", damage = 10, probability = 70 },
                new EnemyAttackZone.AttackConfig { attackType = "TripleBite", damage = 20, probability = 30 }
            });
        }

        // Zona 2: Dash (raio maior, baixa probabilidade)
        if (zones.Length > 1)
        {
            zones[1].GetAttacks().Add(new EnemyAttackZone.AttackConfig { attackType = "Dash", damage = 15, probability = 20, isSingleUsePerEntry = true });
        }

        // Inicializa o cache de ataques
        attackCache["Bite"] = new DogBotAttacks(EnemyAttackTypes.DogBot.Bite, 10, this, objectName);
        attackCache["TripleBite"] = new DogBotAttacks(EnemyAttackTypes.DogBot.TripleBite, 20, this, objectName);
        attackCache["Dash"] = new DogBotAttacks(EnemyAttackTypes.DogBot.Dash, 15, this, objectName);



        //SetAttackType(DogBotAttacks.AttackTypes.Bite, 150);
        //SetGenericAttackType(EnemyAttackTypes.DogBot.Bite, 150);

    }

    protected override void ExecuteAttack(EnemyAttackZone.AttackConfig attackConfig)
    {
        if (attackCache.TryGetValue(attackConfig.attackType, out var attack))
        {
            if (attackConfig.isSingleUsePerEntry && attackConfig.hasBeenTriggered) return;

            if (attack.canAttack && attack.CanAttackWithProbabilites(attackConfig.probability))
            {
                currentAttack = attack;
                enemyAttack = currentAttack;
                attack.ExecuteAttack(transform);

                if (attackConfig.isSingleUsePerEntry) attackConfig.hasBeenTriggered = true;
            }
            else
            {
                if (attackConfig.isSingleUsePerEntry) attackConfig.hasBeenTriggered = true;
            }
        }
        else
        {
            Debug.LogWarning($"Ataque {attackConfig.attackType} não encontrado no cache do DogBot!");
        }
        
    }

    void SetAttackType(EnemyAttackTypes.DogBot attackType, int damage)
    {
        if (!dogBotAttackCache.TryGetValue(attackType, out var cachedAttack))
        {
            cachedAttack = new DogBotAttacks(attackType, damage, this, objectName);
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
