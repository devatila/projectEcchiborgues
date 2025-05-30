using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WaveBasedPerk", menuName = "New Perk/Wave Based Test Perk")]
public class WaveBasedSamplePerkSO : PerkSO
{
    public override PerkBase CreatePerkInstance()
    {
        return new PerkSampleWaveBased(this, wavesDuration);
    }

    public override Type GetPerkType()
    {
        throw new NotImplementedException();
    }
}
