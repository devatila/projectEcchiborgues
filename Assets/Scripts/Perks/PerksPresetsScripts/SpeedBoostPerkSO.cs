using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSpeedBoostPerk", menuName = "New Perk/New PlayerSpeedBoostPerk")]
public class SpeedBoostPerkSO : PerkSO
{

    public float multiplier;
    public override PerkBase CreatePerkInstance()
    {
        Player_Movement player = FindObjectOfType<Player_Movement>();
        if(player == null)
        {
            Debug.LogError("Player Movement não encontrado, Esta classe esta devidamente referenciada no objeto player?");
            return null;
        }
        return new PlayerSpeedBoostPerk(player, multiplier);
    }

    public override Type GetPerkType()
    {
        return typeof(PlayerSpeedBoostPerk);
    }
}
