using System.Collections.Generic;
using UnityEngine;
public class EnemyIndicator : MonoBehaviour
{
    // Vou transformar isso em singleton, me perdoe os principios do encapsulamento mas esse crime vai acontecer.
    public static EnemyIndicator instance;

    public List<GameObject> enemies = new List<GameObject>(); // Lista dos inimigos a serem monitorados
    public RectTransform indicatorPrefab; // Prefab do indicador (seta) no Canvas
    public Canvas canvas; // O Canvas onde o indicador será exibido
    public Camera mainCamera; // A câmera principal do jogo

    public List<RectTransform> indicators;

    [SerializeField] private List<RectTransform> indicatorsOnWait = new List<RectTransform>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

        
    }

    void FixedUpdate()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null && enemies[i].gameObject.activeSelf == true)
            {
                UpdateIndicator(indicators[i], enemies[i].transform);
            }
        }
    }

    void UpdateIndicator(RectTransform indicator, Transform enemy)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(enemy.position);
        EnemyBase enemyControl = enemy.GetComponent<EnemyBase>();

        // Verifica se o inimigo está na tela
        bool isOffScreen = screenPos.x <= 0 || screenPos.x >= Screen.width || screenPos.y <= 0 || screenPos.y >= Screen.height;

        

        if (isOffScreen)
        {
            // Mostra o indicador
            indicator.gameObject.SetActive(true);

            // Mantém o indicador dentro dos limites da tela
            screenPos.x = Mathf.Clamp(screenPos.x, 50, Screen.width - 50);
            screenPos.y = Mathf.Clamp(screenPos.y, 50, Screen.height - 50);

            // Converte a posição da tela para coordenadas do Canvas
            Vector2 indicatorPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPos, canvas.worldCamera, out indicatorPos);
            indicator.anchoredPosition = indicatorPos;

            // Usa o "PontoCentral" ao invés da posição do inimigo
            Transform enemyCenter = enemyControl != null ? enemyControl.centralPosition : enemy.transform;
            Vector3 direction = enemyCenter.position - mainCamera.transform.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            indicator.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Esconde o indicador se o inimigo estiver na tela
            indicator.gameObject.SetActive(false);
        }
    }

    public void OnSpawningEnemies(GameObject enemy)
    {
        Debug.Log("Chamou");
        
        enemies.Add(enemy);
        RectTransform newIndicator;
        // Cria o um indicador por chamada de inimigo spawnado
        if (indicatorsOnWait.Count > 0)
        {
            newIndicator = indicatorsOnWait[0];
            indicatorsOnWait.RemoveAt(0);
        }
        else
        {
            newIndicator = Instantiate(indicatorPrefab, canvas.transform);
            newIndicator.gameObject.SetActive(false);
        }
        
        // Adiciona a lista o indicador gerado
        indicators.Add(newIndicator);
    }

    public void OnEnemyDeath(GameObject enemy)
    {
        int index = enemies.IndexOf(enemy);

        indicators[index].gameObject.SetActive(false);
        indicatorsOnWait.Add(indicators[index]);

        indicators.RemoveAt(index);
        enemies.Remove(enemy);
    }
}
