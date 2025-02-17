using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Turret Attributes", menuName = "Project Classes/ New Turret")]
public class TurretSO : ScriptableObject
{
    public TurretLevel basicAttribute;
    public TurretLevel[] turretUpgradeStats;

    
}

[System.Serializable]
public class TurretLevel
{
    [Header("Basics Stats")]
    public int levelCost;
    public int turretHealth;
    public int turretDamage;
    public float turretCadency;

    [Space()]
    [Header("Burst Manager")]
    [Tooltip("Defina a quantidade de Projeteis liberados a cada vez que a torreta for Atirar, Sempre maior que 1")]
    public int burstAmmount = 1;
    public float burstFirerate = 0;

    public void SetLevel(TurretSO turretData, int levelToSet)
    {
        if (levelToSet < 0 || levelToSet >= turretData.turretUpgradeStats.Length)
        {
            Debug.LogError("Level fora do intervalo válido.");
            return;
        }

        TurretLevel levelData = turretData.turretUpgradeStats[levelToSet];

        Debug.Log($"Setando Nivel para {levelToSet}");

        // Atualiza as propriedades do objeto atual com os valores do nível selecionado
        this.levelCost = levelData.levelCost;
        this.turretHealth = levelData.turretHealth;
        this.turretDamage = levelData.turretDamage;
        this.turretCadency = levelData.turretCadency;
        this.burstAmmount = levelData.burstAmmount;
        this.burstFirerate = levelData.burstFirerate;
    }

}
