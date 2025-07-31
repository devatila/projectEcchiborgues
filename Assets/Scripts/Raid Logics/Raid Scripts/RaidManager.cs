using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidManager : MonoBehaviour
{
    public static RaidManager instance;

    public event Action OnEndSubRaid;
    public event Action OnEndAllRaids;
    public event Action OnEnemyDeath;

    public int day; // Dia atual da Raid
    private int dayIndex; // Índice ajustado (day - 1)
    public int raidIndex; // Índice da Raid atual

    [Header("Raid Configuration")]
    public RaidPresetsSO[] raidPresetsSO; // Array de Scriptable Objects de Raid
    public Transform[] spawnPoints; // Pontos de spawn fixos para essa raid

    [Header("Runtime Data")]
    public List<GameObject> EnemiesAlive = new List<GameObject>(); // Lista de inimigos atualmente vivos
    public float perkTime = 30f;

    [SerializeField] private CinemachineConfiner2D cinemachineConfiner2D;
    private Collider2D defaultConfinerArea;

    private Coroutine RaidCoroutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        defaultConfinerArea = cinemachineConfiner2D?.m_BoundingShape2D;
        OnEndAllRaids += SetDefaultBoundArea;
        #region [Descontinuado]
        // Inicializa o índice de dia para garantir que está correto com base no array
        //dayIndex = Mathf.Max(day - 1, 0);
        //if (raidPresetsSO[dayIndex].startEvent.GetPersistentEventCount() > 0) //Esse IF não deve ser chamado UNITY pls ;-; apenas se definido algo
        //{
        //    raidPresetsSO[dayIndex].startEvent.Invoke();
        //    return;
        //}
        //StartCoroutine(ExecuteRaid(dayIndex));
        #endregion

    }

    /// <summary>
    /// Inicia uma raid determinada
    /// </summary>
    /// <param name="raidPreset">ScriptableObject das informações basicas de raid</param>
    /// <param name="spawnPoints">Pontos de Spawn para esta raid</param>
    /// <param name="colliderArea">Limites de visualização da camera do player</param>
    public void StartRaid(RaidPresetsSO raidPreset, Transform[] spawnPoints = null, PolygonCollider2D colliderArea = null)
    {
        // Se houver alguma raid acontecendo por algum motivo e outra se iniciar, vai forçar a parar e começar do zero
        if (RaidCoroutine != null) StopExecutingRaid();
        if (colliderArea != null) cinemachineConfiner2D.m_BoundingShape2D = colliderArea;
        else Debug.LogWarning("colliderArea de camera não atribuido");


        RaidCoroutine = StartCoroutine(ExecuteRaid(raidPreset));
    }

    public void StopExecutingRaid()
    {
        if (RaidCoroutine != null) StopCoroutine(RaidCoroutine);
        RaidCoroutine = null;
    }

    private IEnumerator ExecuteRaid(RaidPresetsSO raidPreset)
    {
        // Percorre cada `PerRaidPerformance` na lista de raids para o dia atual
        PerRaidPerformance[] allRaidsPresets = raidPreset.subRaidsPerformance;
        for (raidIndex = 0; raidIndex < allRaidsPresets.Length; raidIndex++)
        {
            Debug.Log($"Starting Raid {raidIndex + 1} on Day {day}");
            PerRaidPerformance currentRaid = allRaidsPresets[raidIndex];

            // Executa cada `OnRaidPerformance` (sub-horda) dentro dessa raid
            foreach (OnRaidPerformance subRaid in currentRaid.onRaidPerformances)
            {
                if (subRaid.awaitsEveryoneIsDead)
                {
                    yield return new WaitUntil(() => EnemiesAlive.Count == 0);
                }
                yield return new WaitForSeconds(subRaid.delayToSpawn); // Aguarda o delay especificado
                SpawnSubRaid(subRaid, raidPreset); // Chama o método para spawnar os inimigos
            }


            // Espera até que todos os inimigos dessa Raid estejam mortos
            yield return new WaitUntil(() => EnemiesAlive.Count == 0);

            if (raidIndex < allRaidsPresets.Length - 1)
            {
                Debug.Log($"Raid {raidIndex + 1} completed!");
                Debug.Log("Select your desired Perk");
                OnEndSubRaid?.Invoke(); // Chamado para atualizar perks por duração de wave e outros bonus

                float timer = perkTime;
                while (timer > 0)
                {
                    Debug.Log($"Next raid starting in: {timer} seconds, the actual raidIndex is: {raidIndex}.");
                    timer -= 1; // Decrementa o timer de acordo com o tempo real de jogo
                    yield return new WaitForSeconds(1); // Aguarda o próximo segundo
                }
            }
        }

        Debug.Log("All Raids of the Region has been executed!");
        OnEndAllRaids?.Invoke();
        RaidCoroutine = null;
    }

    private IEnumerator ExecuteDayRaid(int dayIndex)
    {
        // Verifica se o dia e o índice são válidos para evitar erros de referência
        if (dayIndex >= raidPresetsSO.Length)
        {
            Debug.LogWarning("Day index out of bounds! Make sure you have set up the raids correctly.");
            yield break;
        }

        // Percorre cada `PerRaidPerformance` na lista de raids para o dia atual
        PerRaidPerformance[] dayRaids = raidPresetsSO[dayIndex].subRaidsPerformance;
        for (raidIndex = 0; raidIndex < dayRaids.Length; raidIndex++)
        {
            Debug.Log($"Starting Raid {raidIndex + 1} on Day {day}");
            PerRaidPerformance currentRaid = dayRaids[raidIndex];

            // Executa cada `OnRaidPerformance` (sub-horda) dentro dessa raid
            foreach (OnRaidPerformance subRaid in currentRaid.onRaidPerformances)
            {
                if (subRaid.awaitsEveryoneIsDead)
                {
                    yield return new WaitUntil(() => EnemiesAlive.Count == 0);
                }
                yield return new WaitForSeconds(subRaid.delayToSpawn); // Aguarda o delay especificado
                SpawnSubRaid(subRaid); // Chama o método para spawnar os inimigos
            }


            // Espera até que todos os inimigos dessa Raid estejam mortos
            yield return new WaitUntil(() => EnemiesAlive.Count == 0);
            
            if (raidIndex < dayRaids.Length - 1)
            {
                Debug.Log($"Raid {raidIndex + 1} completed!");
                Debug.Log("Select your desired Perk");
                OnEndSubRaid?.Invoke(); // Chamado para atualizar perks por duração de wave e outros bonus

                float timer = perkTime;
                while (timer > 0)
                {
                    Debug.Log($"Next raid starting in: {timer} seconds, the actual raidIndex is: {raidIndex}.");
                    timer -= 1; // Decrementa o timer de acordo com o tempo real de jogo
                    yield return new WaitForSeconds(1); // Aguarda o próximo frame
                }
            }
        }

        Debug.Log("All raids for the day completed!");
        OnEndAllRaids?.Invoke();
    }
    private void SpawnSubRaid(OnRaidPerformance subRaid, RaidPresetsSO raidPreset = null)
    {
        
        for (int i = 0; i < subRaid.howMany; i++)
        {
            // Escolhe um inimigo com base na probabilidade definida
            GameObject chosenEnemy = ChooseEnemyByPreset(subRaid.desiredEnemies, raidPreset);
            
            // Seleciona um ponto de spawn aleatório entre os pontos fixos definidos
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

            // Spawna o inimigo no local escolhido
            GameObject spawnedEnemy = EnemyOnSceneManager.instance.GetEnemy(chosenEnemy); 
            //Instantiate(chosenEnemy, spawnPoint.position, Quaternion.identity); // Metodo Antigo
            EnemiesAlive.Add(spawnedEnemy);
            EnemyIndicator.instance?.OnSpawningEnemies(spawnedEnemy);

            Debug.Log($"Spawned {chosenEnemy.name} at {spawnPoint.position}");

        }
        
    }
    public GameObject GetEnemy()
    {
        return null;
    }
    public void RemoveEnemyOnDeath(GameObject enemy)
    {
        EnemiesAlive.Remove(enemy);
        OnEnemyDeath?.Invoke();
    }

    private GameObject ChooseEnemy(EnemyTypeAndProbability[] enemies)
    {
        // Calcula a soma total das probabilidades
        int totalProbability = 0;
        foreach (var enemy in enemies)
        {
            totalProbability += enemy.probability;
        }

        // Escolhe um valor aleatório dentro do intervalo total de probabilidades
        int randomValue = UnityEngine.Random.Range(0, totalProbability);

        // Percorre os inimigos para determinar qual foi selecionado com base na probabilidade
        int cumulative = 0;
        foreach (var enemy in enemies)
        {
            cumulative += enemy.probability;
            if (randomValue < cumulative)
            {
                return raidPresetsSO[dayIndex].enemiesToSpawn[enemy.enemyIndex];
            }
        }

        // Caso algo dê errado, retorna o primeiro inimigo como fallback
        return raidPresetsSO[dayIndex].enemiesToSpawn[0];
    }
    private GameObject ChooseEnemyByPreset(EnemyTypeAndProbability[] enemies, RaidPresetsSO raidPreset)
    {
        // Calcula a soma total das probabilidades
        int totalProbability = 0;
        foreach (var enemy in enemies)
        {
            totalProbability += enemy.probability;
        }

        // Escolhe um valor aleatório dentro do intervalo total de probabilidades
        int randomValue = UnityEngine.Random.Range(0, totalProbability);

        // Percorre os inimigos para determinar qual foi selecionado com base na probabilidade
        int cumulative = 0;
        foreach (var enemy in enemies)
        {
            cumulative += enemy.probability;
            if (randomValue < cumulative)
            {
                return raidPreset.enemiesToSpawn[enemy.enemyIndex];
            }
        }

        // Caso algo dê errado, retorna o primeiro inimigo como fallback
        return raidPreset.enemiesToSpawn[0];
    }

    void SetDefaultBoundArea()
    {
        cinemachineConfiner2D.m_BoundingShape2D = defaultConfinerArea;
    }
}
