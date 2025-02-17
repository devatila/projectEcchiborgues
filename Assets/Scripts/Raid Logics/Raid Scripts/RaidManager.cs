using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidManager : MonoBehaviour
{
    public static RaidManager instance;

    public int day; // Dia atual da Raid
    private int dayIndex; // �ndice ajustado (day - 1)
    public int raidIndex; // �ndice da Raid atual

    [Header("Raid Configuration")]
    public RaidPresetsSO[] raidPresetsSO; // Array de Scriptable Objects de Raid
    public Transform[] spawnPoints; // Pontos de spawn fixos para essa fase

    [Header("Runtime Data")]
    public List<GameObject> EnemiesAlive = new List<GameObject>(); // Lista de inimigos atualmente vivos
    public float perkTime = 30f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Inicializa o �ndice de dia para garantir que est� correto com base no array
        dayIndex = Mathf.Max(day - 1, 0);
        if (raidPresetsSO[dayIndex].startEvent.GetPersistentEventCount() > 0) //Esse IF n�o deve ser chamado UNITY pls ;-; apenas se definido algo
        {
            raidPresetsSO[dayIndex].startEvent.Invoke();
            return;
        }
        StartCoroutine(ExecuteRaid(dayIndex));
    }

    private IEnumerator ExecuteRaid(int dayIndex)
    {
        // Verifica se o dia e o �ndice s�o v�lidos para evitar erros de refer�ncia
        if (dayIndex >= raidPresetsSO.Length)
        {
            Debug.LogWarning("Day index out of bounds! Make sure you have set up the raids correctly.");
            yield break;
        }

        // Percorre cada `PerRaidPerformance` na lista de raids para o dia atual
        PerRaidPerformance[] dayRaids = raidPresetsSO[dayIndex].raidsOfTheDay;
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
                SpawnSubRaid(subRaid); // Chama o m�todo para spawnar os inimigos
            }


            // Espera at� que todos os inimigos dessa Raid estejam mortos
            yield return new WaitUntil(() => EnemiesAlive.Count == 0);
            Debug.Log($"Raid {raidIndex + 1} completed!");
            Debug.Log("Select your desired Perk");
            // PearkSelecter() // Aqui seria a fun��o que chamaria a janelinha de pearks, essa fun��o ja far� aleat�riamente a sele��o
            if (raidIndex < dayRaids.Length - 1)
            {
                float timer = perkTime;
                while (timer > 0)
                {
                    Debug.Log($"Next raid starting in: {timer} seconds, the actual raidIndex is: {raidIndex}.");
                    timer -= Time.deltaTime; // Decrementa o timer de acordo com o tempo real de jogo
                    yield return null; // Aguarda o pr�ximo frame
                }
            }
        }

        Debug.Log("All raids for the day completed!");
    }
    private void SpawnSubRaid(OnRaidPerformance subRaid)
    {
        
        for (int i = 0; i < subRaid.howMany; i++)
        {
            // Escolhe um inimigo com base na probabilidade definida
            GameObject chosenEnemy = ChooseEnemy(subRaid.desiredEnemies);
            
            // Seleciona um ponto de spawn aleat�rio entre os pontos fixos definidos
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Spawna o inimigo no local escolhido
            GameObject spawnedEnemy = Instantiate(chosenEnemy, spawnPoint.position, Quaternion.identity);
            EnemiesAlive.Add(spawnedEnemy);
            EnemyIndicator.instance.OnSpawningEnemies(spawnedEnemy);

            Debug.Log($"Spawned {chosenEnemy.name} at {spawnPoint.position}");

             // blz vou s� fechar as coisas okidoki
        }
        
    }
    public GameObject GetEnemy()
    {
        return null;
    }
    public void RemoveEnemyOnDeath(GameObject enemy)
    {
        EnemiesAlive.Remove(enemy);
    }

    private GameObject ChooseEnemy(EnemyTypeAndProbability[] enemies)
    {
        // Calcula a soma total das probabilidades
        int totalProbability = 0;
        foreach (var enemy in enemies)
        {
            totalProbability += enemy.probability;
        }

        // Escolhe um valor aleat�rio dentro do intervalo total de probabilidades
        int randomValue = Random.Range(0, totalProbability);

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

        // Caso algo d� errado, retorna o primeiro inimigo como fallback
        return raidPresetsSO[dayIndex].enemiesToSpawn[0];
    }
}
