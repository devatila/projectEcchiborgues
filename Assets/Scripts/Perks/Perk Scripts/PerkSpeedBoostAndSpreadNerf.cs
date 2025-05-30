using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkSpeedBoostAndSpreadNerf : PerkBase
{
    private float speedMultiplier;
    private float spreadMultiplier;
    private PlayerPerkManager player;

    public PerkSpeedBoostAndSpreadNerf(PerkSO perkso, float speedMultiplier, float spreadMultiplier, PlayerPerkManager player) : base(perkso)
    {
        this.speedMultiplier = speedMultiplier;
        this.spreadMultiplier = spreadMultiplier;
        this.player = player;
    }

    public override void OnApply()
    {
        player.SetMovementMultiplier(speedMultiplier);
        player.playerGunMultipliers.allGunsSpreadMultiplier += spreadMultiplier;
        player.SetGunsMultipliers();
    }

    public override void OnRemove()
    {
        isActive = false;
        player.SetMovementMultiplier(-speedMultiplier);
        player.playerGunMultipliers.allGunsSpreadMultiplier -= spreadMultiplier;
        player.SetGunsMultipliers();
    }
}
