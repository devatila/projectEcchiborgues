using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemyAttackZone : MonoBehaviour
{
    [SerializeField] private EnemyBase enemy;
    [SerializeField] private List<AttackConfig> attacks = new List<AttackConfig>();

    // Configuração de cada ataque associado a esta zona
    [System.Serializable]
    public class AttackConfig
    {
        public string attackType; // Nome do ataque (ex.: "Bite", "Dash")
        public int damage;
        public int probability; // 0-100
    }

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyBase>();
        if (enemy == null)
        {
            Debug.LogError("AttackZone não encontrou EnemyBase no pai!", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.PlayerEnteredZone(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.PlayerExitedZone(this);
        }
    }

    public List<AttackConfig> GetAttacks() => attacks;

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



