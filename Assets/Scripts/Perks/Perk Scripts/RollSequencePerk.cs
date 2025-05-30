using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollSequencePerk : PerkBase
{
    private RollSequencePerkSO.PlayerStatsToChange choosenStat;
    private float valuePerKill;
    private float maxValue;
    private float currentValue;
    private float duration;
    private float timer;

    private PlayerPerkManager player;

    public RollSequencePerk(
        PerkSO perkso,
        PlayerPerkManager player,
        RollSequencePerkSO.PlayerStatsToChange choosenStat,
        float valuePerKill,
        float maxValue,
        float duration
        ) : base(perkso)
    {
        this.player = player;
        this.choosenStat = choosenStat;
        this.valuePerKill = valuePerKill;
        this.maxValue = maxValue;
        this.duration = duration;
    }
    public override void OnApply()
    {
        if(RaidManager.instance != null) RaidManager.instance.OnEnemyDeath += ApplySequenceBuffs;
    }

    public override void OnRemove()
    {
        if (RaidManager.instance != null) RaidManager.instance.OnEnemyDeath -= ApplySequenceBuffs;
    }

    public override void Update(float deltaTime = 0)
    {
        if (currentValue > 0)
        {
            timer -= deltaTime;
            if (timer <= 0)
            {
                ResetSequenceBuffs();
            }
        }
    }

    private void ApplySequenceBuffs()
    {
        currentValue = Mathf.Min(currentValue + valuePerKill, maxValue);
        switch (choosenStat)
        {
            case RollSequencePerkSO.PlayerStatsToChange.Damage:
                float previousValue = currentValue > 0 ? currentValue - valuePerKill : 0f;

                player.SetGeneralDamageMultiplier(-previousValue); // Remove o valor anterior
                player.SetGeneralDamageMultiplier(currentValue);   // adiciona o novo valor
                player.SetGunsMultipliers();
                break;
            case RollSequencePerkSO.PlayerStatsToChange.MovementSpeed:
                    player.SetMovementMultiplier(currentValue);
                break;
        }
        timer = duration;
    }

    private void ResetSequenceBuffs()
    {
        switch (choosenStat)
        {
            case RollSequencePerkSO.PlayerStatsToChange.Damage:
                player.SetGeneralDamageMultiplier(-currentValue);
                player.SetGunsMultipliers();
                break;
            case RollSequencePerkSO.PlayerStatsToChange.MovementSpeed:
                player.SetMovementMultiplier(-currentValue);
                break;
        }
        
        currentValue = 0f;
    }
}
