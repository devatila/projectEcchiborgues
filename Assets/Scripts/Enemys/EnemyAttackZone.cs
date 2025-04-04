using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyAttackZone : MonoBehaviour
{
    [SerializeField] private EnemyBase enemy;
    public EnemyBase.EnemyTypes currentType;

    public enum SoldierBotAttackTypes { Shoot, Punch }

    private Type currentAttackEnumType;

    public string selectedAttackName; // Salva o nome do ataque no Inspector
    public Enum selectedAttack;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyBase>();
        SetEnemyType(currentType);

        // Converte o nome salvo para o Enum correspondente
        if (!string.IsNullOrEmpty(selectedAttackName) && currentAttackEnumType != null)
        {
            selectedAttack = (Enum)Enum.Parse(currentAttackEnumType, selectedAttackName);
        }
        else
        {
            selectedAttack = GetFirstEnumValue(currentAttackEnumType);
        }

        Debug.Log("Selected Attack on Awake: " + selectedAttack);
    }

    public void SetEnemyType(EnemyBase.EnemyTypes type)
    {
        if (type == EnemyBase.EnemyTypes.DogBot)
            currentAttackEnumType = typeof(DogBotAttacks.AttackTypes);
        else if (type == EnemyBase.EnemyTypes.SoldierBot)
            currentAttackEnumType = typeof(SoldierBotAttackTypes);

        // Atualiza o nome salvo no Inspector
        if (selectedAttack != null)
        {
            selectedAttackName = selectedAttack.ToString();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemy != null)
        {
            enemy.isPlayerOnAttackRange = true;
            enemy.SetGenericAttackType(selectedAttack, 150);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (enemy != null)
        {
            enemy.isPlayerOnAttackRange = false;
        }
    }

    private Enum GetFirstEnumValue(Type enumType)
    {
        if (enumType == null) return null;
        Array values = Enum.GetValues(enumType);
        return values.Length > 0 ? (Enum)values.GetValue(0) : null;
    }

    private void OnValidate()
    {
        // Garante que o tipo de ataque é coerente no editor
        SetEnemyType(currentType);
    }

}



[CustomEditor(typeof(EnemyAttackZone))]
public class EnemyAttackZoneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EnemyAttackZone enemyAttackZone = (EnemyAttackZone)target;
        EnemyBase enemyTypeChoosen = enemyAttackZone.GetComponentInParent<EnemyBase>();

        if (enemyTypeChoosen != null)
        {
            enemyAttackZone.currentType = enemyTypeChoosen.currentTypeOfEnemy;
        }

        Type attackEnumType = GetAttackEnumType(enemyAttackZone.currentType);

        if (attackEnumType != null)
        {
            string[] attackNames = Enum.GetNames(attackEnumType);
            int selectedIndex = Array.IndexOf(attackNames, enemyAttackZone.selectedAttackName);

            if (selectedIndex < 0) selectedIndex = 0;

            selectedIndex = EditorGUILayout.Popup("Attack Type", selectedIndex, attackNames);

            // Salva a escolha no nome e no Enum
            enemyAttackZone.selectedAttackName = attackNames[selectedIndex];
            enemyAttackZone.selectedAttack = (Enum)Enum.Parse(attackEnumType, attackNames[selectedIndex]);
        }
        
        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(enemyAttackZone);
        }
    }

    private Type GetAttackEnumType(EnemyBase.EnemyTypes enemyType)
    {
        switch (enemyType)
        {
            case EnemyBase.EnemyTypes.DogBot:
                return typeof(DogBotAttacks.AttackTypes);
            case EnemyBase.EnemyTypes.SoldierBot:
                return typeof(EnemyAttackZone.SoldierBotAttackTypes);
            default:
                return null;
        }
    }
}

