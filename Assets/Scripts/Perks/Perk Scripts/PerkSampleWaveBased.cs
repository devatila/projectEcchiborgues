using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkSampleWaveBased : PerkBase
{
    private int waveCount;

    public PerkSampleWaveBased (PerkSO perkso,int maxWaves) : base (perkso)
    {
        this.maxWaves = maxWaves;
        if (this.maxWaves > 0) hasWaveDuration = true;
        else hasWaveDuration = false;
    }

    public override void OnApply()
    {
        waveCount = 0;
        Debug.Log("Perk com Validade de Turno ativado");
    }

    public override void OnRemove()
    {
        Debug.Log("Perk com Validade de Turno Removido");
    }

    public override void UpdateWaveCount()
    {
        if (hasWaveDuration)
        waveCount++;
    }

    public override bool IsExpired => waveCount >= maxWaves;
    
}
