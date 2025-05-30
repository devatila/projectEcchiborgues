using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStatType
{
    Life,
    Armor
}
[CreateAssetMenu(fileName = "WhenStatPlayerDoStuff", menuName = "New Perk/WhenPlayerStatDoStuff")]
public class WhenPlayerStatDoStuffPerkSO : PerkSO
{
    [Header("Quando um Atributo do Player chegar em...")]
    public PlayerStatType whatStatShouldWatch;
    [Tooltip("In porcentage")]
    public int valuePointToTrigger;

    [Header("O que deverá ser feito?")]
    // Valores em porcentagem
    public float gainHealthValue;
    public float gainArmorValue;
    public float gainDamageValue;

    [Space()]
    public bool oncePerWave;
    public bool removeEffectWhenValueIsOverThanMinimum;
    // "Chuvas de arroz e tudo depoissssh"


    public override PerkBase CreatePerkInstance()
    {
        PlayerPerkManager player = FindObjectOfType<PlayerPerkManager>();
        return new WhenPlayerStatDoStuffPerk(this, whatStatShouldWatch, valuePointToTrigger, gainHealthValue, gainArmorValue, gainDamageValue, player,
            oncePerWave, removeEffectWhenValueIsOverThanMinimum);
    }

    public override Type GetPerkType()
    {
        throw new NotImplementedException();
    }

    

}
