using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifePerk : PerkBase
{
    private int startHealthAfterDeath;
    private bool startWithRegen;
    private float regenAmmount, regenInterval;

    private float damageEffectBuff;
    private float damageReductionBuff;
    private float moveSpeedBuff;

    private float timeToExit;
    private PlayerPerkManager player;

    private bool regenerationWasAlreadyInExecution;
    private float timer;

    public PlayerLifePerk(PerkSO so,
        PlayerPerkManager player,
        int startHealthAfterDeath,
        bool startWithRegen,
        float regenAmmount,
        float regenInterval,
        float damageEffectBuff,
        float damageReductionBuff,
        float moveSpeedBuff,
        float timeToExit)
        : base(so)
    {
        this.player = player;
        this.startHealthAfterDeath = startHealthAfterDeath;
        this.startWithRegen = startWithRegen;
        this.regenAmmount = regenAmmount;
        this.regenInterval = regenInterval;
        this.damageEffectBuff = damageEffectBuff;
        this.damageReductionBuff = damageReductionBuff;
        this.moveSpeedBuff = moveSpeedBuff;
        this.timeToExit = timeToExit;
    }

    public override void OnApply()
    {
        
        timer = timeToExit;
        regenerationWasAlreadyInExecution = player.playerHealth.canRegenerate;
        player.playerHealth.AddLifeCount();
        player.playerHealth.OnPlayerHealthZero += OnDeath;
    }

    public override void OnRemove()
    {
        player.playerHealth.SetRegenerationValues(-regenAmmount, -regenInterval, regenerationWasAlreadyInExecution);

        player.playerHealth.damageMultiplier += damageReductionBuff;
        player.SetGeneralDamageMultiplier(-damageEffectBuff);
        player.SetGunsMultipliers();

        player.SetMovementMultiplier(-moveSpeedBuff);

        player.playerHealth.OnPlayerHealthZero -= OnDeath;
        isActive = false;
    }

    void OnDeath()
    {
        
        Debug.Log($"O player est[a com {player.playerHealth.GetActualHealth()} de vida e foi acionado");
        isActive = true;
        if (startHealthAfterDeath > 0) player.playerHealth.GetHeal(startHealthAfterDeath);
        if (startWithRegen)
        {
            if(regenAmmount != 0 || regenInterval != 0)
            {
                player.playerHealth.SetRegenerationValues(regenAmmount, regenInterval);
            }
            else
            {
                player.playerHealth.EnableRegeneration();
            }
        }
        player.playerHealth.damageMultiplier -= damageReductionBuff;
        player.SetGeneralDamageMultiplier(damageEffectBuff);
        player.SetGunsMultipliers();
        player.playerHealth.AddLifeCount(-1);
        player.SetMovementMultiplier(moveSpeedBuff);
    }

    public override void Update(float deltaTime = 0)
    {
        if (!isActive) return;

        timer -= deltaTime;
        if(timer <= 0)
        {
            OnRemove();
            isActive = false;
        }
        Debug.Log(timer);
    }


    // WE COULD BE SO HOOTTT HOT TOGETHER
}
