using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TurretBehavior : MonoBehaviour, IUpgradeable
{
    public   bool              isActive            = false;
    private  bool              isThereEnemyOnRange = false;
    public   bool              isFixedHead         = false; // A "cabeça" da Torreta é fixa ou ela acompanha o Inimigo?
    public   float             tRange;
    public   int               actualLevel;
    public   TurretSO          turretData;
    public   TurretLevel       turretLevel;
    public   LayerMask         maskToAttack;
    private  PlaceableObject   pObject;
    public   Transform         turretHead;
    [SerializeField] private List<GameObject> ammountOfEnemiesOnRange = new List<GameObject>();
    [SerializeField] private Transform[] shootPositions;
    private int shootPosIndex;
    public int projectileID;
    public bool UseShootPosInSequence; // Das posições de tiro, usar todas elas de uma vez na hora de atacar
    // O design disso vou deixar a cargo do JUUJ  // Ou usar sequencialmente? // APENAS SE shootPositions.Length > 1

    private bool canShoot;
    private bool isPlayerCloser = false;
    private bool canUpgrade = true;
    public GameObject obj;

    private UpgradeWindow upgradesShopMenu;
    private int aPlayerCash; //Actual Player Cash - Atualizado apenas quando o player Chega Perto

    void Start()
    {
        GetAllReferencesFromTurretSO(turretData);

        canUpgrade = false;
        upgradesShopMenu = FindObjectOfType<UpgradeWindow>();
        UnityEngine.Debug.Log("Feliz Aniversário JUUJ");
        canShoot = true;
        pObject = GetComponent<PlaceableObject>();
        pObject.OnPlaced += TurnitOnPlaced;

        TurretProjectilesPoolingManager.Instance.SetMoreProjectilesInPool(projectileID);

        // Começa ativado se na outra classe estiver marcado como já posicionado (Só para casos de teste ou bem específico)
        isActive = pObject.startPlaced;
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isThereEnemyOnRange)
        {
            LookToObject();
            if (canShoot)
            {
                Shoot();
                StartCoroutine(TurretFirerateManager(turretData.basicAttribute.turretCadency));
                canShoot = false;
            }
        }
        if (!canUpgrade) return;
        if(isPlayerCloser && Input.GetKeyDown(KeyCode.E))
        {
            upgradesShopMenu.ShowUpgradeShop(this); 
            //Evento que vai mostrar a janela de upgrade para a determinada torreta
        }
    }

    void FixedUpdate()
    {
        if (isActive)
        {
            TurretRangeChecker();
        }
    }

    void Shoot()
    {
        //Que oreguiça de fazer mais um object pooling ;-;
        // acho que vou fazer um crime
        if (turretLevel.burstAmmount == 0)
        {
            turretLevel.burstAmmount = 1; // Ajustado automaticamente para evitar erros
            UnityEngine.Debug.LogWarning("Favor, Reajustar o numero de Rajada por Tiros no SO");
        }
        StartCoroutine(TurretBurstFirerateController(turretLevel.burstFirerate, turretLevel.burstAmmount));
        // SpawnProjectiles();

    }

    private void SpawnProjectiles()
    {
        if (UseShootPosInSequence)
        {
            GetAndSpawnProjectile();
            shootPosIndex++;
            if (shootPosIndex > shootPositions.Length - 1) shootPosIndex = 0;
        }
        else
        {
            for (int i = 0; i < shootPositions.Length; i++)
            {
                GetAndSpawnMultiplesProjectiles(i);
            }
        }
    }

    private void GetAndSpawnProjectile()
    {
        obj = TurretProjectilesPoolingManager.Instance.GetDesiredProjectile(projectileID);
        obj.transform.position = shootPositions[shootPosIndex].position; // MDS MAS EU SOU BURRRO!!!
        obj.transform.rotation = shootPositions[shootPosIndex].rotation;
        obj.GetComponent<TurretProjectileBehavior>().GetDamageData(turretData.basicAttribute.turretDamage);
    }

    private void GetAndSpawnMultiplesProjectiles(int i)
    {
        obj = TurretProjectilesPoolingManager.Instance.GetDesiredProjectile(projectileID);
        obj.transform.position = shootPositions[i].position; // MDS MAS EU SOU BURRRO!!!
        obj.transform.rotation = shootPositions[i].rotation;
        obj.GetComponent<TurretProjectileBehavior>().GetDamageData(turretData.basicAttribute.turretDamage);
    }

    public void ShowInputCondition(PlayerInventory pInventory)
    {
        if (!isActive || !canUpgrade) return;
        UnityEngine.Debug.Log("Mostrando o Input Condition");
        aPlayerCash = pInventory.metalCash;
        isPlayerCloser = true;
    }

    public void HideInputCondition()
    {
        
        UnityEngine.Debug.Log("Escondendo o Input Condition");
        isPlayerCloser = false;
    }

    private void TurretRangeChecker()
    {
        // Obtém todos os colliders dentro do raio.
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, tRange, maskToAttack);

        // Limpa a lista atual para atualizá-la com os inimigos detectados.
        ammountOfEnemiesOnRange.Clear();

        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Adiciona o inimigo detectado à lista.
                ammountOfEnemiesOnRange.Add(hit.gameObject);
            }
        }

        // Atualiza a flag com base no número de inimigos detectados.
        isThereEnemyOnRange = ammountOfEnemiesOnRange.Count > 0;
    }

    private GameObject FindClosestEnemyOnRange()
    {
        GameObject closestEnemy = null;
        float shortestDistance = float.MaxValue;

        foreach (GameObject enemy in ammountOfEnemiesOnRange)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if(distance < shortestDistance)
            {
                shortestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    IEnumerator TurretFirerateManager(float timer)
    {
        yield return new WaitForSeconds(timer);
        canShoot = true;
    }

    IEnumerator TurretBurstFirerateController(float burstFirerate, int burstAmmount)
    {
        for (int i = 0; i < burstAmmount; i++)
        {
            SpawnProjectiles();
            yield return new WaitForSeconds(burstFirerate);
        }

        yield break;
    }

    void LookToObject()
    {
        GameObject closestEnemy = FindClosestEnemyOnRange();
        if (closestEnemy == null) return;

        // Acessa o "PontoCentral" do inimigo
        Transform enemyCenter = closestEnemy.GetComponent<EnemyBase>().centralPosition;

        Vector3 direction = enemyCenter.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        turretHead.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void TurnitOnPlaced()
    {
        isActive = true;
        canUpgrade = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, tRange);
    }

    void GetAllReferencesFromTurretSO(TurretSO turretDataSO)
    {
        turretLevel = turretDataSO.basicAttribute;
    }

    public void SetNextLevel()
    {
        turretLevel = turretData.turretUpgradeStats[actualLevel];
        actualLevel++;
        if(actualLevel >= turretData.turretUpgradeStats.Length)
        {
            canUpgrade = false;
            UnityEngine.Debug.Log("Chegou no Maximo de Upgrade para esta torreta");
        }
    }
}
