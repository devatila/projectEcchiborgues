using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkSmgDamageBoost : PerkBase
{
    private float damageMultiplier;
    private PlayerPerkManager player;
    private bool isActive;

    public PerkSmgDamageBoost (PlayerPerkManager player, float multiplicador)
    {
        this.player = player;
        this.damageMultiplier = multiplicador;
    }
    public override void OnApply()
    {
        isActive = true;
        player.playerGunMultipliers.SetGunMultiplier(PlayerInventory.ammoTypeOfGunEquipped.Sub, PlayerGunMultipliers.GunMultiplierType.Damage, damageMultiplier);
        player.SetGunsMultipliers();
    }

    public override void OnRemove()
    {
        isActive = false;
        player.playerGunMultipliers.SetGunMultiplier(PlayerInventory.ammoTypeOfGunEquipped.Sub, PlayerGunMultipliers.GunMultiplierType.Damage, -damageMultiplier);
        player.SetGunsMultipliers();
    }

    public override bool IsExpired => !isActive;
}
