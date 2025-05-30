using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RollSequence", menuName = "New Perk/Roll Sequence Perk")]
public class RollSequencePerkSO : PerkSO
{
    public enum PlayerStatsToChange
    {
        Damage,
        MovementSpeed,
    }
    public PlayerStatsToChange statToChange;
    public float valuePerKill;
    public float maxValue;
    public float duration;
    public override PerkBase CreatePerkInstance()
    {
        PlayerPerkManager player = FindObjectOfType<PlayerPerkManager>();
        return new RollSequencePerk(this, player, statToChange, valuePerKill, maxValue, duration);
    }

    public override Type GetPerkType()
    {
        throw new NotImplementedException();
    }
}
