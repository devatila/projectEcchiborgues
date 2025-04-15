using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : EnemyEffect
{
    
    public StunEffect(EnemyBase enemy, float duration) : base(enemy, duration) { stateType = NaturalStates.Eletric; }
    
    public override void OnApply()
    {
        enemy.SetStun();
    }

    protected override void OnEnd()
    {
        enemy.SetStun(false);
    }

    protected override void OnUpdate(float deltaTime)
    {
        // Isso não deve ser chamado pois...bem, o inimigo não pode tomar dano de stun...imagino eu
    }
}
