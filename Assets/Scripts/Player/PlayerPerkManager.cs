using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static PlayerGunMultipliers;
using static PlayerInventory;

public class PlayerPerkManager : MonoBehaviour
{
    // Variaveis de Controles Gerais
    [Header("Multiplicadores Gerais")]
    public float playerMovementMultiplier           = 1f; // Multiplicador de velocidade de movimento do player
    public float playerTakeableDamagerMultiplier    = 1f; // Multiplicador geral de dano recebido pelo player
    public float playerThrowableGeneralMultiplier   = 1f; // Multiplicador geral do dano causado por todos os arremessáveis

    [Space()]
    // Variaveis de Controle Especificos - De armas e arremessaveis
    [Header("Multiplicadores Específicos")]
    //No quesito Armas
    public PlayerGunMultipliers playerGunMultipliers;
    public Projectile.StatesPercentage effectStatesProbabilites;
    [Space()]
    //No Quesito Arremessaveis


    // Referencias do player [MANTER PRIVADO NO INSPECTOR]
    private PlayerInventory     playerInventory;
    private Player_Movement     playerMovement;
    private HandThrowableScript playerThrowableScript;
    private PlayerHealth        playerHealth;

    // Incrementar uma func que aumenta ou diminui a vida total do player

    private void Start()
    {
        GetBasicsReferences();
        playerGunMultipliers.BuildGunStructMap();
        playerGunMultipliers.BuildMultiplierSetters();
    }

    private void GetBasicsReferences()
    {
        // Inicializando Referencias...
        playerInventory = GetComponent<PlayerInventory>();
        playerMovement = GetComponent<Player_Movement>();
        playerThrowableScript = GetComponent<HandThrowableScript>();
        playerHealth = GetComponent<PlayerHealth>();
    } // O nome ja é autoexplicativo...certo?

    #region PlayerBasicsMultipliersForApplyingInInspector

    public void SetMovementMultiplier(float multiplier)             { playerMovementMultiplier *= (1 + multiplier); UpdatePlayerMovement(); }
    public void ResetMovementMultiplier() { playerMovementMultiplier = 1f; UpdatePlayerMovement(); }
    public void RemoveMovementMultiplier(float multiplier) { playerMovementMultiplier /= (1 + multiplier); UpdatePlayerMovement(); }
    private void UpdatePlayerMovement() => playerMovement.SetMultiplierMovement(playerMovementMultiplier);

    public void SetGeneralDamageMultiplier(float multiplier)        => playerGunMultipliers.allGunsMultiplier *= (1 + multiplier);
    public void ResetGeneralDamageMultiplier() => playerGunMultipliers.allGunsMultiplier = 1f;

    public void SetTakeableDamageMultiplier(float multiplier)       => playerTakeableDamagerMultiplier *= (1 + multiplier);
    public void ResetTakeableDamageMultiplier() => playerTakeableDamagerMultiplier = 1f;


    public void SetGeneralThrowablesDamagerMultiplier(float multiplier) => playerThrowableGeneralMultiplier *= (1 + multiplier);
    public void ResetGeneralThrowablesDamagerMultiplier() => playerThrowableGeneralMultiplier = 1f;

    #endregion

    // Basicamente é como um Atualizador dos atributos das armas
    public void SetGunsMultipliers()
    {
        //UpdateBulletSpecialEffects();
        playerInventory.SetGunMultipliersByTypeOfGun(playerGunMultipliers, effectStatesProbabilites); // acho que não ta fun
    }

    public void UpdateBulletSpecialEffects()
    {
        playerInventory.GeneralStatesBulletPercentages = effectStatesProbabilites;
    }

    public void ResetGunsEquipped()
    {
        playerGunMultipliers.ResetValues();
        playerInventory.ResetGunEquippedGunAttributes();
        playerInventory.GeneralGunsDamageMultiplier = 1f;
    }

    public void ResetAllMultipliers() // Não esquecer de atualizar
    {
        ResetMovementMultiplier();
        ResetGeneralDamageMultiplier();
        ResetTakeableDamageMultiplier();
        ResetGeneralThrowablesDamagerMultiplier();
        ResetGunsEquipped();
    }

    public void ApplyAllMultipliers()
    {
        UpdatePlayerMovement();
        //SetMovementMultiplier(playerMovementMultiplier);
        //SetGeneralDamageMultiplier(playerGunMultipliers.allGunsMultiplier);
        //SetTakeableDamageMultiplier(playerTakeableDamagerMultiplier);
        //SetSpreadEffectorMultiplier(playerGunSpreadGeneralMultiplier);
        //SetGeneralThrowablesDamagerMultiplier(playerThrowableGeneralMultiplier);
        SetGunsMultipliers();
    }

}

[CustomEditor(typeof(PlayerPerkManager))]
public class PlayerPerkManagerEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        PlayerPerkManager playerPerkManager = (PlayerPerkManager)target;

        if (GUILayout.Button("Apply Changes"))
        {
            playerPerkManager.ApplyAllMultipliers();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Reset Changes"))
        {
            playerPerkManager.ResetAllMultipliers();
        }
    }
}

[System.Serializable]
public class PlayerGunMultipliers
{
    public float allGunsMultiplier = 1f;
    public float allGunsSpreadMultiplier = 1f;

    // inicializa todos
    public GunMultipliers ArMultipliers                 = new GunMultipliers();
    public GunMultipliers SubMultipliers                = new GunMultipliers();
    public GunMultipliers ShotgunMultipliers            = new GunMultipliers();
    public GunMultipliers PistolMultipliers             = new GunMultipliers();
    public GunMultipliers GranadeLauncherMultipliers    = new GunMultipliers();
    public GunMultipliers RocketLauncherMultipliers     = new GunMultipliers();
    public GunMultipliers FlameThrowerMultipliers       = new GunMultipliers();

    private Dictionary<ammoTypeOfGunEquipped, GunMultipliers> gunStructMap;
    private Dictionary<GunMultiplierType, Action<GunMultipliers, float>> multiplierSetters;
    private Dictionary<GunMultiplierType, Action<GunMultipliers, float>> divisorSetters;

    public enum GunMultiplierType
    {
        Damage,
        Spread,
        ReloadTime,
        FireRate
    }
    public void ResetValues()
    {
        ArMultipliers.ResetAllMultipliers(); SubMultipliers.ResetAllMultipliers(); ShotgunMultipliers.ResetAllMultipliers(); PistolMultipliers.ResetAllMultipliers();
        GranadeLauncherMultipliers.ResetAllMultipliers(); RocketLauncherMultipliers.ResetAllMultipliers(); FlameThrowerMultipliers.ResetAllMultipliers();
    }
    public void BuildGunStructMap()
    {
        gunStructMap = new Dictionary<ammoTypeOfGunEquipped, GunMultipliers>
        {
            { ammoTypeOfGunEquipped.AR,                ArMultipliers },
            { ammoTypeOfGunEquipped.Sub,               SubMultipliers },
            { ammoTypeOfGunEquipped.Shotgun,           ShotgunMultipliers },
            { ammoTypeOfGunEquipped.Pistol,            PistolMultipliers },
            { ammoTypeOfGunEquipped.Granade_Launcher,  GranadeLauncherMultipliers },
            { ammoTypeOfGunEquipped.FlameThrower,      FlameThrowerMultipliers },
            { ammoTypeOfGunEquipped.Rocket_Launcher,   RocketLauncherMultipliers },
        };

    }

    public void BuildMultiplierSetters()
    {
        multiplierSetters = new Dictionary<GunMultiplierType, Action<PlayerGunMultipliers.GunMultipliers, float>>()
        {
            { GunMultiplierType.Damage,     (gm, v) => gm.damageMultiplier      *= (1 + v) },
            { GunMultiplierType.Spread,     (gm, v) => gm.spreadMultiplier      *= (1 + v) },
            { GunMultiplierType.ReloadTime, (gm, v) => gm.reloadTimeMultiplier  *= (1 + v) },
            { GunMultiplierType.FireRate,   (gm, v) => gm.firerateMultiplier    *= (1 + v) },
        };

        divisorSetters = new Dictionary<GunMultiplierType, Action<GunMultipliers, float>>()
        {
            {GunMultiplierType.Damage,      (gm, v) => gm.damageMultiplier      /= (1 + v) },
            {GunMultiplierType.Spread,      (gm, v) => gm.spreadMultiplier      /= (1 + v) },
            {GunMultiplierType.ReloadTime,  (gm, v) => gm.reloadTimeMultiplier  /= (1 + v) },
            {GunMultiplierType.FireRate,    (gm, v) => gm.firerateMultiplier    /= (1 + v) },
        };
    }

    /// <summary>
    /// Define o valor de um multiplicador específico para uma arma.
    /// </summary>
    /// <param name="ammo">Tipo de arma (enum).</param>
    /// <param name="which">Qual multiplicador (enum).</param>
    /// <param name="value">O novo valor a ser aplicado.</param>
    public void SetGunMultiplier(
        ammoTypeOfGunEquipped ammo,
        GunMultiplierType which,
        float value
    )
    {
        // 1) Encontra o struct de multiplicadores daquela arma
        if (!gunStructMap.TryGetValue(ammo, out var gm))
        {
            Debug.LogError($"[PerkManager] arma '{ammo}' não configurada no gunStructMap!");
            return;
        }
        
        // 2) Encontra o “setter” correspondente
        if (!multiplierSetters.TryGetValue(which, out var setter))
        {
            Debug.LogError($"[PerkManager] não há setter para {which}!");
            return;
        }

        // 3) Aplica!
        setter(gm, value);
        Debug.Log($"[{ammo}] {which} multipliers set to {value}");
    }

    /// <summary>
    /// Remove o valor de um multiplicador em porcentagem específico para uma arma.
    /// </summary>
    /// <param name="ammo">Tipo de arma (enum).</param>
    /// <param name="which">Qual multiplicador (enum).</param>
    /// <param name="value">O valor a ser removido.</param>
    public void RemoveGunMultiplier(
        ammoTypeOfGunEquipped ammo,
        GunMultiplierType which,
        float value
    )
    {
        // 1) Encontra o struct de multiplicadores daquela arma
        if (!gunStructMap.TryGetValue(ammo, out var gm))
        {
            Debug.LogError($"[PerkManager] arma '{ammo}' não configurada no gunStructMap!");
            return;
        }

        // 2) Encontra o “setter” correspondente
        if (!divisorSetters.TryGetValue(which, out var setter))
        {
            Debug.LogError($"[PerkManager] não há setter para {which}!");
            return;
        }

        // 3) Aplica!
        setter(gm, value);
        Debug.Log($"[{ammo}] {which} multipliers set to {value}");
    }


    [System.Serializable]
    public class GunMultipliers
    {
        public float damageMultiplier = 1f;
        public float spreadMultiplier = 1f;
        public float reloadTimeMultiplier = 1f;
        public float firerateMultiplier = 1f;

        public void ResetAllMultipliers()
        {
            damageMultiplier = 1f;
            spreadMultiplier = 1f;
            reloadTimeMultiplier = 1f;
            firerateMultiplier = 1f;
        }
    }



    /*
    public float arDamageMultiplier = 1f;
    public float subDamageMultiplier = 1f;
    public float shotgunDamageMultiplier = 1f;
    public float pistolDamageMultiplier = 1f;
    public float flamethrowerDamageMultiplier = 1f;
    public float granadeLauncherDamageMultiplier = 1f;

    public void ResetValues()
    {
        allGunsMultiplier = 1f;
        arDamageMultiplier = 1f;
        subDamageMultiplier = 1f;
        shotgunDamageMultiplier = 1f;
        pistolDamageMultiplier = 1f;
        flamethrowerDamageMultiplier = 1f;
        granadeLauncherDamageMultiplier = 1f;
    }
     * */

}
