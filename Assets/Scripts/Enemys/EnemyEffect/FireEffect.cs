using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffect : EnemyEffect
{
    public FireEffect(EnemyBase enemy, float duration) : base(enemy, duration)
    {
        stateType = NaturalStates.Fire;
    }

    public override void OnApply()
    {
        enemy.SetFireState();
    }

    protected override void OnEnd()
    {
        enemy.CancelFireState();
    }

    public override void ResetDuration(float newDurantion)
    {
        base.ResetDuration(newDurantion);
        enemy.SetFireState();
    }

    protected override void OnUpdate(float deltaTime)
    {
        //throw new System.NotImplementedException();
    }
}
