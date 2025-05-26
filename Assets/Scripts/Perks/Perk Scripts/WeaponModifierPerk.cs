using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModifierPerk : PerkBase
{
    private PlayerPerkManager player;
    private Projectile.StatesPercentage projectileEffects;
    private bool applyToAllWeapons;
    private PlayerInventory.ammoTypeOfGunEquipped gunTypeToAffect;
    private PlayerGunMultipliers.GunMultipliers Multipliers;

    public WeaponModifierPerk 
        (
        PlayerPerkManager player,
        Projectile.StatesPercentage projectileEffects,
        bool applyToAllWeapons,
        PlayerInventory.ammoTypeOfGunEquipped gunTypeToAffect,
        PlayerGunMultipliers.GunMultipliers Multipliers
        )
    {
        this.player = player;
        this.projectileEffects = projectileEffects;
        this.applyToAllWeapons = applyToAllWeapons;
        this.gunTypeToAffect = gunTypeToAffect;
        this.Multipliers = Multipliers;
    }

    public override void OnApply()
    {
        if (applyToAllWeapons)
        {
            player.playerGunMultipliers.allGunsMultiplier += Multipliers.damageMultiplier;
            player.playerGunMultipliers.allGunsSpreadMultiplier += Multipliers.spreadMultiplier;
        }
        else
        {
            player.playerGunMultipliers.SetGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.Damage, Multipliers.damageMultiplier);
            player.playerGunMultipliers.SetGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.Spread, Multipliers.spreadMultiplier);
            player.playerGunMultipliers.SetGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.FireRate, Multipliers.firerateMultiplier);
            player.playerGunMultipliers.SetGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.ReloadTime, Multipliers.reloadTimeMultiplier);
        }

        player.effectStatesProbabilites.SlowEffectProbability += projectileEffects.SlowEffectProbability;
        player.effectStatesProbabilites.StunEffectProbability += projectileEffects.StunEffectProbability;
        player.effectStatesProbabilites.FireEffectProbability += projectileEffects.FireEffectProbability;

        player.SetGunsMultipliers(); // Atualiza os atributos das armas
    }

    public override void OnRemove()
    {
        if (applyToAllWeapons)
        {
            player.playerGunMultipliers.allGunsMultiplier -= Multipliers.damageMultiplier;
            player.playerGunMultipliers.allGunsSpreadMultiplier -= Multipliers.spreadMultiplier;
        }
        else
        {
            player.playerGunMultipliers.RemoveGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.Damage, Multipliers.damageMultiplier);
            player.playerGunMultipliers.RemoveGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.Spread, Multipliers.spreadMultiplier);
            player.playerGunMultipliers.RemoveGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.FireRate, Multipliers.firerateMultiplier);
            player.playerGunMultipliers.RemoveGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.ReloadTime, Multipliers.reloadTimeMultiplier);
        }

        player.effectStatesProbabilites.SlowEffectProbability -= projectileEffects.SlowEffectProbability;
        player.effectStatesProbabilites.StunEffectProbability -= projectileEffects.StunEffectProbability;
        player.effectStatesProbabilites.FireEffectProbability -= projectileEffects.FireEffectProbability;

        player.SetGunsMultipliers(); // Atualiza os atributos das armas
    }
}
