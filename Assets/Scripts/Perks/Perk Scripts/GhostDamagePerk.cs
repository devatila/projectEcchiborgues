using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostDamagePerk : PerkBase
{
    // Isso é um ativador...n tem pq criar um desativador

    private float valueToAdd;
    PlayerPerkManager player;

    public GhostDamagePerk(PlayerPerkManager player, PerkSO so, float valueToAdd) : base(so)
    {
        this.player = player;
        this.valueToAdd = valueToAdd;
    }
    public override void OnApply()
    {
        player.playerHealth.AddGhostDamageProb(valueToAdd);
    }

    public override void OnRemove()
    {
        player.playerHealth.AddGhostDamageProb(-valueToAdd);
        // Eu AMO essa IDE
    }
}
