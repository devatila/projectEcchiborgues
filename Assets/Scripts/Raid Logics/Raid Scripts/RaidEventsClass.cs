using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidEventsClass : ScriptableObject 
{
    public virtual void OnRaidStart()
    {
        Debug.LogWarning("Evento de inicio não configurado para essa raid");
    }

    public virtual void OnRaidUpdate()
    {
        // Tenho medo da possivel utilidade dessa função
    }

    public virtual void OnRaidEnd()
    {
        Debug.LogWarning("Evento de termino não configurado para essa raid");
    }
}
