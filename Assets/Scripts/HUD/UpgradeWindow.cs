using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeWindow : MonoBehaviour
{
    public event Action OnOpenMenu;

    [Header("Current Turret Attributes")]
    [SerializeField] private TMP_Text actualLevelText;
    [SerializeField] private GameObject turretUpgradeWindow;

    [SerializeField] private TurretLevelShopUI currentLevelStats = new TurretLevelShopUI();
    [SerializeField] private TurretLevelShopUI nextLevelStats = new TurretLevelShopUI();
    [SerializeField] private Button upgradeButton;

    private TurretLevel actualLevelState;
    private TurretSO actualTurretData;
    private int actualLevelNumber;
    private UI_BuySystem bSystem;

    [Space()]
    [SerializeField]private PlayerInventory pInventory;
    private TurretBehavior desiredTurretBehavior;

    [Header("UI Icons")]
    public Image dmgIcon;
    public Image firerateIcon;
    public Image burstIcon;
    private void Start()
    {
        if (turretUpgradeWindow.activeSelf) turretUpgradeWindow.SetActive(false);
        //pInventory = FindObjectOfType<PlayerInventory>();
        OnOpenMenu += pInventory.StopAllPlayerMovement;
        bSystem = GetComponentInParent<UI_BuySystem>();
    }
    public void ShowUpgradeShop(TurretBehavior turret)
    {
        if (bSystem.IsShopOpen() || turretUpgradeWindow.activeSelf) return;

        bSystem.BlockShopWhileBuilding(0);
        turretUpgradeWindow.SetActive(true);
        OnOpenMenu?.Invoke();

        TurretLevel turretLevel = turret.turretLevel;
        TurretSO turretSO = turret.turretData;

        bool hasCondition = CheckPlayerHasMoney(turretSO, turret.actualLevel);
        upgradeButton.interactable = hasCondition;
        
        TMP_Text btnText = upgradeButton.GetComponentInChildren<TMP_Text>();
        btnText.text = turretSO.turretUpgradeStats[turret.actualLevel].levelCost.ToString();

        if (!hasCondition) btnText.color = Color.red;
        else btnText.color = Color.black;

        currentLevelStats.GetDataFromTurretStats(turret.actualLevel, turretLevel);
        nextLevelStats.GetDataFromTurretStats(turret.actualLevel + 1, turretSO.turretUpgradeStats[turret.actualLevel]);
        CompareUpgradeStats(currentLevelStats, nextLevelStats);
        //
        desiredTurretBehavior = turret;

    }

    void CompareUpgradeStats(TurretLevelShopUI currentStats, TurretLevelShopUI nextStats)
    {
        bool hasMoreFirerate = nextLevelStats.tFirerate < currentLevelStats.tFirerate;
        bool hasMoreBurst = nextLevelStats.tBurst > currentLevelStats.tBurst;
        bool hasMoreDamage = nextLevelStats.tDamage > currentLevelStats.tDamage;

        firerateIcon.enabled = hasMoreFirerate;
        burstIcon.enabled = hasMoreBurst;
        dmgIcon.enabled = hasMoreDamage;
    }
    public void SetNextTurretLevel()
    {
        bSystem.AllowShopAfterBuild();
        desiredTurretBehavior.SetNextLevel();

        pInventory.ContinueAllPlayerMovement();
        pInventory.DecreaseCash(desiredTurretBehavior.turretLevel.levelCost);

        turretUpgradeWindow.SetActive(false);
    }

    public void CloseMenu()
    {
        bSystem.AllowShopAfterBuild();
        SetBackPlayerMovement();
        turretUpgradeWindow.SetActive(false);
    }

    void SetBackPlayerMovement()
    {
        pInventory.ContinueAllPlayerMovement();
    }
    bool CheckPlayerHasMoney(TurretSO turretDataParam, int levelAtual)
    {
        int playerCash = pInventory.metalCash;
        int upgradeCost = turretDataParam.turretUpgradeStats[levelAtual].levelCost;
        Debug.Log(upgradeCost);
        return playerCash >= upgradeCost;
    }

    [System.Serializable]
    private class TurretLevelShopUI
    {
        public TMP_Text actualLevelText;
        public TMP_Text turretFirerate;
        public TMP_Text turretBurst;
        public TMP_Text turretDamage;

        public float tFirerate { get; set; }
        public int tBurst { get; set; }
        public int tDamage { get; set; }
        public void GetDataFromTurretStats(int actualLevel, TurretLevel level)
        {
            actualLevelText.text    = actualLevel.ToString();
            turretFirerate.text     = level.turretCadency.ToString();
            turretBurst.text        = level.burstAmmount.ToString();
            turretDamage.text       = level.turretDamage.ToString();

            tFirerate = level.turretCadency;
            tBurst = level.burstAmmount;
            tDamage = level.turretDamage;
        }
    }

    
}
