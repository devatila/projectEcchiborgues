using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GhostDamagePerk", menuName = "New Perk/Ghost Damage Perk")]
public class GhostDamagePerkSO : PerkSO
{
    public float valueToAdd;
    public override PerkBase CreatePerkInstance()
    {
        PlayerPerkManager player = FindObjectOfType<PlayerPerkManager>();
        return new GhostDamagePerk(player, this, valueToAdd);
    }

    public override Type GetPerkType()
    {
        return null;
    }
}
