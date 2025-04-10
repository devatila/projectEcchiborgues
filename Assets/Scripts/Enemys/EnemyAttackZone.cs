using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemyAttackZone : MonoBehaviour
{
    [SerializeField] private EnemyBase enemy;
    [SerializeField] internal bool lastZone;
    public EnemyBase.EnemyTypes currentType;
    [HideInInspector] public int probability;

    public enum SoldierBotAttackTypes { Shoot, Punch }

    private Type currentAttackEnumType;

    public string selectedAttackName; // Salva o nome do ataque no Inspector
    public Enum selectedAttack;
    public int damage;

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

        //Debug.Log("Selected Attack on Awake: " + selectedAttack);
    }

    public void SetEnemyType(EnemyBase.EnemyTypes type)
    {
        currentAttackEnumType = EnemyAttackEnumResolver.GetAttackEnumType(type);
        

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
            enemy.SetGenericAttackType(selectedAttack, damage, probability);
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

}

public static class EnemyAttackEnumResolver
{
    private static Dictionary<EnemyBase.EnemyTypes, Type> _attackEnumCache;

    public static Type GetAttackEnumType(EnemyBase.EnemyTypes enemyType)
    {
        if (_attackEnumCache == null)
        {
            _attackEnumCache = new Dictionary<EnemyBase.EnemyTypes, Type>();

            var typesWithAttribute = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.GetCustomAttributes(typeof(EnemyAttackEnumAttribute), false).Length > 0);

            foreach (var type in typesWithAttribute)
            {
                var attr = (EnemyAttackEnumAttribute)type.GetCustomAttributes(typeof(EnemyAttackEnumAttribute), false)[0];
                if (!_attackEnumCache.ContainsKey(attr.enemyType))
                {
                    _attackEnumCache[attr.enemyType] = attr.enumType;
                }
            }
        }

        _attackEnumCache.TryGetValue(enemyType, out var result);
        return result;
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
        enemyAttackZone.lastZone = EditorGUILayout.Toggle("Is Last Zone?", enemyAttackZone.lastZone);
        Type attackEnumType = EnemyAttackEnumResolver.GetAttackEnumType(enemyAttackZone.currentType);

        if (attackEnumType != null)
        {
            // Pega todos os nomes do enum
            string[] allAttackNames = Enum.GetNames(attackEnumType);

            // Coleta todos os AttackZones do mesmo inimigo
            var allZones = enemyAttackZone.GetComponentInParent<EnemyBase>()
                .GetComponentsInChildren<EnemyAttackZone>();

            // Lista de ataques já escolhidos por outros AttackZones
            var alreadyUsed = allZones
                .Where(z => z != enemyAttackZone && !string.IsNullOrEmpty(z.selectedAttackName))
                .Select(z => z.selectedAttackName)
                .ToHashSet();

            // Filtra para mostrar apenas os ataques ainda não usados
            var filteredNames = allAttackNames
                .Where(name => !alreadyUsed.Contains(name))
                .ToList();

            // Garante que o valor atual apareça mesmo se já estiver usado
            if (!string.IsNullOrEmpty(enemyAttackZone.selectedAttackName) &&
                !filteredNames.Contains(enemyAttackZone.selectedAttackName))
            {
                filteredNames.Add(enemyAttackZone.selectedAttackName);
            }

            // Organiza alfabeticamente para não bagunçar visual
            filteredNames.Sort();

            // Define índice do selecionado
            int selectedIndex = filteredNames.IndexOf(enemyAttackZone.selectedAttackName);
            if (selectedIndex < 0) selectedIndex = 0;

            selectedIndex = EditorGUILayout.Popup("Attack Type", selectedIndex, filteredNames.ToArray());

            // Atualiza campos
            enemyAttackZone.selectedAttackName = filteredNames[selectedIndex];
            enemyAttackZone.selectedAttack = (Enum)Enum.Parse(attackEnumType, filteredNames[selectedIndex]);

        }

        // Slidinho da chance (probability)
        enemyAttackZone.probability = EditorGUILayout.IntSlider(new GUIContent("Probability", "Valor definido em porcentagem"),
                    enemyAttackZone.probability, 0, 100);

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(enemyAttackZone);
        }
    }

    
}


