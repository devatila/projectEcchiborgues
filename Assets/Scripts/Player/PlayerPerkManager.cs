using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerPerkManager : MonoBehaviour
{
    // Variaveis de Controles Gerais
    [Header("Multiplicadores Gerais")]
    public float playerGeneralDamageMultiplier      = 1f; // Multiplicador de dano geral causado pelo player
    public float playerMovementMultiplier           = 1f; // Multiplicador de velocidade de movimento do player
    public float playerTakeableDamagerMultiplier    = 1f; // Multiplicador geral de dano recebido pelo player
    public float playerGunSpreadGeneralMultiplier   = 1f; // Multiplicador geral de spread causado de armas do player
    public float playerThrowableGeneralMultiplier   = 1f; // Multiplicador geral do dano causado por todos os arremessáveis

    [Space()]
    // Variaveis de Controle Especificos - De armas e arremessaveis
    [Header("Multiplicadores Específicos")]
    //No quesito Armas
    public float arDamageMultiplier             = 1f;
    public float subDamageMultiplier            = 1f;
    public float shotgunDamageMultiplier        = 1f;
    public float pistolDamageMultiplier         = 1f;
    public float flamethrowerDamageMultiplier   = 1f;

    [Space()]
    //No Quesito Arremessaveis


    // Referencias do player [MANTER PRIVADO NO INSPECTOR]
    private PlayerInventory     playerInventory;
    private Player_Movement     playerMovement;
    private HandThrowableScript playerThrowableScript;
    private PlayerHealth        playerHealth;

    // Incrementar variaveis booleanas para roubo de vida e afins

    // Incrementar uma func que aumenta ou diminui a vida total do player

    private void Start()
    {
        GetBasicsReferences();
    }

    private void GetBasicsReferences()
    {
        // Inicializando Referencias...
        playerInventory = GetComponent<PlayerInventory>();
        playerMovement = GetComponent<Player_Movement>();
        playerThrowableScript = GetComponent<HandThrowableScript>();
        playerHealth = GetComponent<PlayerHealth>();
    } // O nome ja é autoexplicativo...certo?

    public void SetMovementMultiplier(float multiplier) { playerMovementMultiplier = multiplier; UpdatePlayerMovement(); }
    public void ResetMovementMultiplier() { playerMovementMultiplier = 1f; UpdatePlayerMovement(); }
    private void UpdatePlayerMovement() => playerMovement.SetMultiplierMovement(playerMovementMultiplier);

    public void SetGeneralDamageMultiplier(float multiplier) => playerGeneralDamageMultiplier = multiplier;
    public void ResetGeneralDamageMultiplier() => playerGeneralDamageMultiplier = 1f;

    public void SetTakeableDamageMultiplier(float multiplier) => playerTakeableDamagerMultiplier = multiplier;
    public void ResetTakeableDamageMultiplier() => playerTakeableDamagerMultiplier = 1f;

    public void SetSpreadEffectorMultiplier(float multiplier) => playerGunSpreadGeneralMultiplier = multiplier;
    public void ResetSpreadEffectorMultiplier() => playerGunSpreadGeneralMultiplier = 1f;

    public void SetGeneralThrowablesDamagerMultiplier(float multiplier) => playerThrowableGeneralMultiplier = multiplier;
    public void ResetGeneralThrowablesDamagerMultiplier() => playerThrowableGeneralMultiplier = 1f;

    public void ResetAllMultipliers() // Não esquecer de atualizar
    {
        ResetMovementMultiplier();
        ResetGeneralDamageMultiplier();
        ResetTakeableDamageMultiplier();
        ResetSpreadEffectorMultiplier();
        ResetGeneralThrowablesDamagerMultiplier();
    }

    public void ApplyAllMultipliers()
    {
        SetMovementMultiplier(playerMovementMultiplier);
        SetGeneralDamageMultiplier(playerGeneralDamageMultiplier);
        SetTakeableDamageMultiplier(playerTakeableDamagerMultiplier);
        SetSpreadEffectorMultiplier(playerGunSpreadGeneralMultiplier);
        SetGeneralThrowablesDamagerMultiplier(playerThrowableGeneralMultiplier);
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
