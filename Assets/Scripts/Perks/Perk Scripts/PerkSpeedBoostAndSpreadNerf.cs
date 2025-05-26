using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkSpeedBoostAndSpreadNerf : PerkBase
{
    private float speedMultiplier;
    private float spreadMultiplier;
    private PlayerPerkManager player;
    private bool isActive;

    public PerkSpeedBoostAndSpreadNerf(float speedMultiplier, float spreadMultiplier, PlayerPerkManager player)
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

    public override bool IsExpired => !isActive;
}
