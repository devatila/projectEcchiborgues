using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Day Raid Preset", menuName = "Project Classes/New Raid Preset Data")]
public class RaidPresetsSO : ScriptableObject
{
    [Header("Eventos da Raid")]
    public RaidEventsClass raidEvents; // Todas os eventos da raid devem estar aqui
    
    [Space()]
    
    [Header("Controle de Inimigos")]
    public GameObject[] enemiesToSpawn; // Inimigos que v�o spawnar neste dia
    
    [Header("Quantidade de Raids do Dia")]
    public PerRaidPerformance[] subRaidsPerformance; //Famosa tatica pra reaproveitar cenario, "os fins n�o justificam os meios" - quem, n�o sei.
    
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
    public bool doesThemComeFromTheAir; //Tipo...eles v�o "spawnar" vindo dos c�us? 
    public EnemyTypeAndProbability[] desiredEnemies;
}

[System.Serializable]
public class EnemyTypeAndProbability
{
    [Range((int)1,(int)100)]
    public int probability; //probabilidade de spawn desse inimigo
    public int enemyIndex; // Tem que representar o index da array enemiesToSpawn do inimigo desejado
    
}
