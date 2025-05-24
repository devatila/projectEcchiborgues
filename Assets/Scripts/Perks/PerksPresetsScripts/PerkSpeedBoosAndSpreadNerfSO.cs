using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBoostAndSpreadNerfPerk", menuName = "New Perk/New SpeedBoost - SpreadNerf")]
public class PerkSpeedBoosAndSpreadNerfSO : PerkSO
{
    [Tooltip("Multiplicador que aumenta a velocidade do player")]
    public float speedMultiplier = 0.6f;

    [Tooltip("Multiplicador que aumenta o spread das armas do player. Quanto maior, mais spread a arma terá. (Não influenciará a força do spread)")]
    public float spreadMultiplier = 0.4f;
    public override PerkBase CreatePerkInstance()
    {
        PlayerPerkManager player = FindObjectOfType<PlayerPerkManager>();
        return new PerkSpeedBoostAndSpreadNerf(speedMultiplier, spreadMultiplier, player);
    }

    public override Type GetPerkType()
    {
        return typeof(PerkSpeedBoostAndSpreadNerf);
    }
}
