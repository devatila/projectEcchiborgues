using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Day Raid Preset", menuName = "Project Classes/New Raid Preset Data")]
public class RaidPresetsSO : ScriptableObject
{
    [Header("Evento Antes da Raid")]
    public UnityEvent startEvent; // Evento que sera chamado ao inicio do dia | Seja ele cutscene ou qualquer coisa do tipo
    
    [Space()]
    
    [Header("Controle de Inimigos")]
    public GameObject[] enemiesToSpawn; // Inimigos que vão spawnar neste dia
    
    [Header("Quantidade de Raids do Dia")]
    public PerRaidPerformance[] subRaidsPerformance; //Famosa tatica pra reaproveitar cenario, "os fins não justificam os meios" - quem, não sei.
    
    [Space()]
    
    [Header("Evento Final Pós Raids")]
    public UnityEvent finishEvent; // Evento que sera chamado ao final do dia | Pode ser outra cutscene ou qualquer coisa do tipo
                                   // Mas isso não significa o Evento de "Passou de Fase", por exemplo. Isso será feito dentro do RaidManager.cs
}

[System.Serializable]
public class PerRaidPerformance
{
    public OnRaidPerformance[] onRaidPerformances;
}

[System.Serializable]
public class OnRaidPerformance
{
    public bool awaitsEveryoneIsDead;
    public float delayToSpawn;
    public int howMany = 4;
    public bool doesThemComeFromTheAir; //Tipo...eles vão "spawnar" vindo dos céus? 
    public EnemyTypeAndProbability[] desiredEnemies;
}

[System.Serializable]
public class EnemyTypeAndProbability
{
    [Range((int)1,(int)100)]
    public int probability; //probabilidade de spawn desse inimigo
    public int enemyIndex; // Tem que representar o index da array enemiesToSpawn do inimigo desejado
    
}
