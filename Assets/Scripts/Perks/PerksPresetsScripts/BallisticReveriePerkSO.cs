using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ballistic Reverie Perk", menuName = "New Perk/Ballistic Reverie Perk")]
public class BallisticReveriePerkSO : PerkSO
{
    public bool shouldActivate = true;

    public override PerkBase CreatePerkInstance()
    {
        PlayerPerkManager player = FindObjectOfType<PlayerPerkManager>();
        return new BallisticReveriePerk(shouldActivate, this, player);
    }

    public override Type GetPerkType()
    {
        return null;
    }
}
