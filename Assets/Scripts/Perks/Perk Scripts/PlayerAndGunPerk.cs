using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAndGunPerk : PerkBase
{
    private float playerSpeed;
    private int playerMaxHealthMultiplier; // Seta a vida maxima
    private float playerReductionDamage;
    private int armorToGain;

    private bool affectAllWeapons;
    private PlayerInventory.ammoTypeOfGunEquipped gunTypeToAffect;
    private PlayerGunMultipliers.GunMultipliers gunMultipliers;

    private PlayerPerkManager player;

    public PlayerAndGunPerk(
        PerkSO perkso,
        float playerSpeed,
        int playerMaxHealthMultiplier,
        float playerReductionDamage,
        int armorToGain,
        bool affectAllWeapons,
        PlayerInventory.ammoTypeOfGunEquipped gunTypeToAffect,
        PlayerGunMultipliers.GunMultipliers gunMultipliers,
        PlayerPerkManager player)
        : base(perkso)
    {
        this.playerSpeed = playerSpeed;
        this.playerMaxHealthMultiplier = playerMaxHealthMultiplier;
        this.playerReductionDamage = playerReductionDamage;
        this.armorToGain = armorToGain;
        this.affectAllWeapons = affectAllWeapons;
        this.gunTypeToAffect = gunTypeToAffect;
        this.gunMultipliers = gunMultipliers;
        this.player = player;
    }

    public override void OnApply()
    {
        player.playerHealth.GetArmor(armorToGain);
        player.playerHealth.damageMultiplier += playerReductionDamage;
        player.SetMovementMultiplier(playerSpeed);
        player.playerHealth.SetMaxHealth(playerMaxHealthMultiplier);


        if (affectAllWeapons)
        {
            player.playerGunMultipliers.allGunsMultiplier += gunMultipliers.damageMultiplier;
            player.playerGunMultipliers.allGunsSpreadMultiplier += gunMultipliers.spreadMultiplier;
        }
        else
        {
            player.playerGunMultipliers.SetGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.Damage, gunMultipliers.damageMultiplier);
            player.playerGunMultipliers.SetGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.Spread, gunMultipliers.spreadMultiplier);
            player.playerGunMultipliers.SetGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.FireRate, gunMultipliers.firerateMultiplier);
            player.playerGunMultipliers.SetGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.ReloadTime, gunMultipliers.reloadTimeMultiplier);
        }

        player.SetGunsMultipliers(); // Atualiza os atributos das armas
    }

    public override void OnRemove()
    {
        player.playerHealth.damageMultiplier -= playerReductionDamage;
        player.SetMovementMultiplier(-playerSpeed);
        player.playerHealth.RemoveMaxHealthMultiplier(playerMaxHealthMultiplier);

        if (affectAllWeapons)
        {
            player.playerGunMultipliers.allGunsMultiplier -= gunMultipliers.damageMultiplier;
            player.playerGunMultipliers.allGunsSpreadMultiplier -= gunMultipliers.spreadMultiplier;
        }
        else
        {
            player.playerGunMultipliers.RemoveGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.Damage, gunMultipliers.damageMultiplier);
            player.playerGunMultipliers.RemoveGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.Spread, gunMultipliers.spreadMultiplier);
            player.playerGunMultipliers.RemoveGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.FireRate, gunMultipliers.firerateMultiplier);
            player.playerGunMultipliers.RemoveGunMultiplier(gunTypeToAffect, PlayerGunMultipliers.GunMultiplierType.ReloadTime, gunMultipliers.reloadTimeMultiplier);
        }
    }
}
