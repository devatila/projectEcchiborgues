using UnityEngine;

public abstract class EnemyAttack
{
    public abstract void ExecuteAttack(Transform enemyTransform);
    public abstract void CancelAttacks();

    public virtual void AnimationPerformTrigger(Animator anim, string stateName)
    {
        anim.SetTrigger(stateName);
    }

    public bool CanAttackWithProbabilites(int actualProb)
    {
        return Random.Range(0, 100) < actualProb;
    }
}
