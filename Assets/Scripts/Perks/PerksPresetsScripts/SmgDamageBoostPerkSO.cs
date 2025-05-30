using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SmgDamageBoostPerk", menuName = "New Perk/SmgDamagePerk")]
public class SmgDamageBoostPerkSO : PerkSO
{
    public float damageMultiplier;

    public override PerkBase CreatePerkInstance()
    {
        PlayerPerkManager player = FindAnyObjectByType<PlayerPerkManager>();
        return new PerkSmgDamageBoost(this, player, damageMultiplier);
    }

    public override Type GetPerkType()
    {
        return typeof(PerkSmgDamageBoost);
    }
}
