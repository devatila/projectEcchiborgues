using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnDeathPlayerPerk", menuName = "New Perk/On Death Player Perk")]
public class PlayerDeathPerkSO : PerkSO
{
    [Space()]
    public int startHealthAfterDeath;
    public bool startWithRegen;
    public float regenAmmount, regenInterval;

    public float damageEffectBuff;
    public float damageReductionBuff;
    public float moveSpeedBuff;

    public float timeToExit;
    private PlayerPerkManager player;
    public override PerkBase CreatePerkInstance()
    {
        player = FindObjectOfType<PlayerPerkManager>();
        return new PlayerLifePerk(this, player, startHealthAfterDeath, startWithRegen, regenAmmount, regenInterval, damageEffectBuff, damageReductionBuff,
            moveSpeedBuff, timeToExit);
    }

    public override Type GetPerkType()
    {
        return null;
    }
}
