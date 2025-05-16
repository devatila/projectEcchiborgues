using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkSampleWaveBased : PerkBase
{
    private int waveCount;
    private int maxWaves;

    public PerkSampleWaveBased (int maxWaves)
    {
        this.maxWaves = maxWaves;
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

    public override void Update(float deltaTime = 0)
    {
        waveCount++;
    }

    public override bool IsExpired => waveCount >= maxWaves;
    
}
