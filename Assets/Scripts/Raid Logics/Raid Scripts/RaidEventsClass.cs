using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidEventsClass : ScriptableObject 
{
    public virtual void OnRaidStart()
    {
        Debug.LogWarning("Evento de inicio n�o configurado para essa raid");
    }

    public virtual void OnRaidUpdate()
    {
        // Tenho medo da possivel utilidade dessa fun��o
    }

    public virtual void OnRaidEnd()
    {
        Debug.LogWarning("Evento de termino n�o configurado para essa raid");
    }
}
