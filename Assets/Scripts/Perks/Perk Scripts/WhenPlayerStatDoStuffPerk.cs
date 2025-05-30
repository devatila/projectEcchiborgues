using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhenPlayerStatDoStuffPerk : PerkBase
{
    private PlayerStatType choosenType;
    private float valuteToWatch;

    private float gainHealthValue;
    private float gainArmorValue;
    private float gainDamageValue;
    private PlayerPerkManager player;

    private bool oncePerWave;
    private bool removeEffectWhenValueIsOverThanMinimum;

    private bool isEffectActive;

    public WhenPlayerStatDoStuffPerk(
        PerkSO perkso,
        PlayerStatType choosenType,
        int valuteToWatch,
        float gainHealthValue,
        float gainArmorValue,
        float gainDamageValue,
        PlayerPerkManager player,
        bool oncePerWave,
        bool removeEffectWhenValueIsOverThanMinimum) : base(perkso)
    {
        this.choosenType = choosenType;
        this.valuteToWatch = valuteToWatch / 100f;
        this.gainHealthValue = gainHealthValue;
        this.gainArmorValue = gainArmorValue;
        this.gainDamageValue = gainDamageValue;
        this.player = player;
        this.oncePerWave = oncePerWave;
        this.removeEffectWhenValueIsOverThanMinimum = removeEffectWhenValueIsOverThanMinimum;

        oneTimeForWave = oncePerWave;
    }

    public override void OnApply()
    {
        player.playerHealth.OnPlayerHit += OnPlayerGetHit;
    }

    public override void OnRemove()
    {
        player.playerHealth.OnPlayerHit -= OnPlayerGetHit;
    }

    private void OnPlayerGetHit()
    {
        if (alreadyExectuedInThisWave && oneTimeForWave) return;

        if (player == null)
        {
            Debug.LogWarning("Referencia do PlayerPerkManager não foi instanciada corretamente. Verificar pontos de chamadas");
            return;
        }
        switch(choosenType)
        {
            case PlayerStatType.Life:
                int actualLife = player.playerHealth.GetActualHealth();
                if (actualLife < player.playerHealth.GetActualMaxHealth() * valuteToWatch)
                {
                    if (isEffectActive) return;
                    ApplyStuffs();
                }
                else if (removeEffectWhenValueIsOverThanMinimum && isEffectActive)
                {
                    DeactivateApplyChanges();
                }
                break;
            case PlayerStatType.Armor:
                int actualArmor = player.playerHealth.GetActualArmor();
                if (actualArmor < player.playerHealth.GetActualMaxHealth() * valuteToWatch)
                {
                    if (isEffectActive) return;
                    ApplyStuffs();
                }
                else if (removeEffectWhenValueIsOverThanMinimum && isEffectActive)
                {
                    DeactivateApplyChanges();
                }
                break;
        }
        // Faço assim para que este mesmo perk possa reaparecer posteriormente
        if (removeOnExecute)
        {
            OnRemove();
        }

    }

    private void ApplyStuffs() // To sem ideia de nome perdão
    {
        player.playerHealth.GetHeal(Mathf.RoundToInt(player.playerHealth.GetActualMaxHealth() * gainHealthValue));
        player.playerHealth.GetArmor(Mathf.RoundToInt(player.playerHealth.GetActualMaxArmor() * gainArmorValue));
        player.playerGunMultipliers.allGunsMultiplier += gainDamageValue;
        if (gainDamageValue != 0) player.SetGunsMultipliers();
        alreadyExectuedInThisWave = true;
        isEffectActive = true;
    }

    private void DeactivateApplyChanges()
    {
        player.playerGunMultipliers.allGunsMultiplier -= gainDamageValue;
        if (gainDamageValue != 0) player.SetGunsMultipliers();
        isEffectActive = false;
    }
}
