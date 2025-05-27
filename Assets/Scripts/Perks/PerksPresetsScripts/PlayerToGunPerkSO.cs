using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AffectGunAndPlayerStats", menuName = "New Perk/Player And Gun Perk")]
public class PlayerToGunPerkSO : PerkSO
{
    [Header("Player Attributes")]
    public float playerSpeed;
    public int playerMaxHealthMultiplier; // Seta a vida maxima
    public float playerReductionDamage;
    [Min(0)]
    public int armorToGain;

    [Space()]

    [Header("Gun Attributes")]
    public bool affectAllWeapons;
    public PlayerInventory.ammoTypeOfGunEquipped gunTypeToAffect;
    public PlayerGunMultipliers.GunMultipliers gunMultipliers;

    public override PerkBase CreatePerkInstance()
    {
        throw new NotImplementedException();
    }

    public override Type GetPerkType()
    {
        throw new NotImplementedException();
    }

}
