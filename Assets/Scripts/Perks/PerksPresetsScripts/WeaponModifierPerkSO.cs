using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Modifier Perk", menuName = "New Perk/Weapon Modifiers")]
public class WeaponModifierPerkSO : PerkSO
{
    [Space()]
    [Header("Multiplicadores de arma")]
    public bool affectAllWeapons;
    public PlayerInventory.ammoTypeOfGunEquipped gunTypeToAffect;
    public PlayerGunMultipliers.GunMultipliers gunMultipliers;

    [Header("Efeitos para todas armas")]
    public Projectile.StatesPercentage statesPercentage;

    public override PerkBase CreatePerkInstance()
    {
        PlayerPerkManager player = FindObjectOfType<PlayerPerkManager>();
        return new WeaponModifierPerk(player, statesPercentage, affectAllWeapons, gunTypeToAffect, gunMultipliers);
    }

    public override Type GetPerkType()
    {
        return null; // Eu não estou suportando mais configurar esse type 
    }
}