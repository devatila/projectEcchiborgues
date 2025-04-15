using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyEffect
{
    // Classe abstrata que cuidará da parte dos efeitos de estado do inimigo
    // Para os tipos Eletric, Fire e Slow (Ice Cold baby)

    protected EnemyBase enemy;
    protected float duration; // Duração, se houver
    protected float timer;

    public bool isFinished => timer >= duration; // de zero para cima

    public NaturalStates stateType;

    public EnemyEffect(EnemyBase enemy, float duration)
    {
        this.enemy = enemy;
        this.duration = duration;
    }

    public void UpdateEffect(float deltaTime)
    {
        timer += deltaTime;
        if (!isFinished)
        {
            OnUpdate(deltaTime);
        }
        else
        {
            OnEnd();

        }
    }

    public virtual void ResetDuration(float newDuration)
    {
        duration = newDuration;
        timer = 0;

        Debug.Log("Efeito já em execução, resetando o tempo...");
    }

    public abstract void OnApply();
    protected abstract void OnUpdate(float deltaTime);
    protected abstract void OnEnd();
}