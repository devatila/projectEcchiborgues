using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Attack Config", fileName = "NewEnemyAttackConfig")]
public class EnemyAttackConfigSO : ScriptableObject
{
    public EnemyBase.EnemyTypes enemyType;
    public float defaultSpeed;
    [Serializable]
    public class AttackInfo
    {
        public string attackName;
        public int damage;
        public int probability;
        public float cooldown;
    }

    public List<AttackInfo> attacks = new();
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyAttackConfigSO))]
public class EnemyAttackConfigSOEditor : Editor
{
    private EnemyAttackConfigSO config;

    public override void OnInspectorGUI()
    {
        config = (EnemyAttackConfigSO)target;
        serializedObject.Update();
        config.defaultSpeed = EditorGUILayout.FloatField("Default Speed", config.defaultSpeed);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("enemyType"));

        Type enumType = EnemyAttackEnumResolver.GetAttackEnumType(config.enemyType);

        if (enumType != null)
        {
            string[] enumNames = Enum.GetNames(enumType);

            foreach (string enumName in enumNames)
            {
                var existing = config.attacks.FirstOrDefault(a => a.attackName == enumName);
                if (existing == null)
                {
                    existing = new EnemyAttackConfigSO.AttackInfo { attackName = enumName };
                    config.attacks.Add(existing);
                }

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField(enumName, EditorStyles.boldLabel);
                existing.damage = EditorGUILayout.IntField("Damage", existing.damage);
                existing.probability = EditorGUILayout.IntSlider("Probability", existing.probability, 0, 100);
                existing.cooldown = EditorGUILayout.FloatField("Cooldown", existing.cooldown);
                EditorGUILayout.EndVertical();
            }

            // Remove ataques obsoletos
            config.attacks.RemoveAll(a => !enumNames.Contains(a.attackName));
        }
        else
        {
            EditorGUILayout.HelpBox("Não foi possível detectar o enum para esse tipo de inimigo.", MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(config);
        }
    }
}
#endif