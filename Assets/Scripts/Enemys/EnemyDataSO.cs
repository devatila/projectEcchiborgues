using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data Core", menuName = "Project Classes/Enemy Basics Data")]
public class EnemyDataSO : ScriptableObject
{
    // Atributos do Inimigo
    public int health = 100;
    public float speed = 4f;
    public bool canHeEvade = true; //Só se quiser que o inimigo não recue do player caso ele chegue perto
    public AudioClip stepsSound_SFX; // Som de Passos

    // Escolhas de definição
    public enum difficulties {Easy, Normal, Hard} // Escolhe a dificuldade do inimigo
    public difficulties enemyDifficult;           // A ideia é que quanto mais facil ele seja, maior seja a demora na movimentação e escolha
                                                  // ...de ataque

    public enum attackTypes {FireGun, HandPunch, Melee, Investida} // Firegun = Armas de Fogo | HandPunch = O bot sair no soco |
                                                          // Melee = o bot usar armas brancas ou qualquer coisa que não atire
    public attackTypes attackTypeOfTheEnemy;              // Investida = Não sei o nome em ingles, mas ele só vai tacar a cabeçada e fds

    //Caso seja FireGun
    public Sprite GunSprite;
    public GameObject GunPrefab;
    public int gunDamage = 25;
    public float FireRate = 1f;
    public enum HoldTypes {OneHand, TwoHand, RPG, LargeGun} //Incrementar mais se for necessário
    public HoldTypes howToHoldTheChoicedGun;
    public AudioClip gunShootSound_SFX; // Som de disparo
    
    public bool doesHeMakesSoundsWhileAttack; // Ele vai fazer algum barulho amendrontado enquanto ataca ou persegue?
    public AudioClip enemySound_SFX; // Caso seja verdadeiro, essa variavel aparece

    public Transform rightHandGunPosition;

    // FUI TOMAR UM CAFÉZINHO DA NOITE EU VOLTO JÁ 

    // Caso seja HandPunch
    public int punchDamage = 60;
    public float PunchRate = 2f;
    public AudioClip punchSound_SFX;

    public bool hasCombo = false; // Ele tem um combo de ataque?
    public int[] punchAttackDamages; // danos subsequentes após o primeiro ataque
    private int attackIndex = 0;

    // Caso seja Melee
    public Sprite meleeSprite;
    public int meleeDamage = 45;
    public float attackMeleeRate = 2f;
    public bool hasMeleeCombo = false;
    public int[] meleeAttackDamages;
    private int meleeAttackIndex;
    public AudioClip meleeAttackSound_SFX;

    // Caso seja Investida
    public int attackDamage = 60;
    public float attackRate = 4f;
    public AudioClip attackSound_SFX;

    public void FireGunAttack()
    {
        int attack = attackIndex;
    }
    public void PunchAttack()
    {

    }
    public void MeleeAttack()
    {

    }
    public void InvestidaAttack()
    {

    }



}

#region EditorChanges
#if UNITY_EDITOR

[CustomEditor(typeof(EnemyDataSO))]
public class EnemyDataSOEditor : Editor
{
    SerializedProperty health, speed, canHeEvade, stepsSound_SFX, enemyDifficult;
    SerializedProperty attackTypeOfTheEnemy, doesHeMakesSoundsWhileAttack, enemySound_SFX;
    SerializedProperty GunSprite, GunPrefab, gunDamage, FireRate, howToHoldTheChoicedGun, gunShootSound_SFX, rightHandGunPosition;
    SerializedProperty punchDamage, PunchRate, punchSound_SFX, hasCombo, punchAttackDamages;
    SerializedProperty meleeSprite, meleeDamage, attackMeleeRate, hasMeleeCombo, meleeAttackDamages, meleeAttackSound_SFX;
    SerializedProperty attackDamage, attackRate, attackSound_SFX;

    private void OnEnable()
    {
        // Atributos Básicos
        health = serializedObject.FindProperty("health");
        speed = serializedObject.FindProperty("speed");
        canHeEvade = serializedObject.FindProperty("canHeEvade");
        stepsSound_SFX = serializedObject.FindProperty("stepsSound_SFX");
        enemyDifficult = serializedObject.FindProperty("enemyDifficult");

        // AttackType e FireGun
        attackTypeOfTheEnemy = serializedObject.FindProperty("attackTypeOfTheEnemy");
        GunSprite = serializedObject.FindProperty("GunSprite");
        GunPrefab = serializedObject.FindProperty("GunPrefab");
        gunDamage = serializedObject.FindProperty("gunDamage");
        FireRate = serializedObject.FindProperty("FireRate");
        howToHoldTheChoicedGun = serializedObject.FindProperty("howToHoldTheChoicedGun");
        gunShootSound_SFX = serializedObject.FindProperty("gunShootSound_SFX");
        doesHeMakesSoundsWhileAttack = serializedObject.FindProperty("doesHeMakesSoundsWhileAttack");
        enemySound_SFX = serializedObject.FindProperty("enemySound_SFX");
        rightHandGunPosition = serializedObject.FindProperty("rightHandGunPosition");

        // HandPunch
        punchDamage = serializedObject.FindProperty("punchDamage");
        PunchRate = serializedObject.FindProperty("PunchRate");
        punchSound_SFX = serializedObject.FindProperty("punchSound_SFX");
        hasCombo = serializedObject.FindProperty("hasCombo");
        punchAttackDamages = serializedObject.FindProperty("punchAttackDamages");

        // Melee
        meleeSprite = serializedObject.FindProperty("meleeSprite");
        meleeDamage = serializedObject.FindProperty("meleeDamage");
        attackMeleeRate = serializedObject.FindProperty("attackMeleeRate");
        hasMeleeCombo = serializedObject.FindProperty("hasMeleeCombo");
        meleeAttackDamages = serializedObject.FindProperty("meleeAttackDamages");
        meleeAttackSound_SFX = serializedObject.FindProperty("meleeAttackSound_SFX");

        // Investida
        attackDamage = serializedObject.FindProperty("attackDamage");
        attackRate = serializedObject.FindProperty("attackRate");
        attackSound_SFX = serializedObject.FindProperty("attackSound_SFX");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Atributos Básicos do Inimigo", EditorStyles.boldLabel);

        // Atributos Básicos
        EditorGUILayout.PropertyField(health);
        EditorGUILayout.PropertyField(speed);
        EditorGUILayout.PropertyField(canHeEvade);
        EditorGUILayout.PropertyField(stepsSound_SFX);
        EditorGUILayout.PropertyField(enemyDifficult);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Escolha o Tipo de Ataque Dele", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(attackTypeOfTheEnemy);

        // Controle da exibição baseado no tipo de ataque
        EnemyDataSO.attackTypes currentAttackType = (EnemyDataSO.attackTypes)attackTypeOfTheEnemy.enumValueIndex;

        switch (currentAttackType)
        {
            case EnemyDataSO.attackTypes.FireGun:
                EditorGUILayout.PropertyField(GunSprite);
                EditorGUILayout.PropertyField(GunPrefab);
                EditorGUILayout.PropertyField(gunDamage);
                EditorGUILayout.PropertyField(FireRate);
                EditorGUILayout.PropertyField(howToHoldTheChoicedGun);
                EditorGUILayout.PropertyField(gunShootSound_SFX);
                EditorGUILayout.PropertyField(doesHeMakesSoundsWhileAttack);
                if (doesHeMakesSoundsWhileAttack.boolValue)
                {
                    EditorGUILayout.PropertyField(enemySound_SFX);
                }
                EditorGUILayout.PropertyField(rightHandGunPosition);
                break;

            case EnemyDataSO.attackTypes.HandPunch:
                EditorGUILayout.PropertyField(punchDamage);
                EditorGUILayout.PropertyField(PunchRate);
                EditorGUILayout.PropertyField(punchSound_SFX);
                EditorGUILayout.PropertyField(hasCombo);
                if (hasCombo.boolValue)
                {
                    EditorGUILayout.PropertyField(punchAttackDamages, true);
                }
                break;

            case EnemyDataSO.attackTypes.Melee:
                EditorGUILayout.PropertyField(meleeSprite);
                EditorGUILayout.PropertyField(meleeDamage);
                EditorGUILayout.PropertyField(attackMeleeRate);
                EditorGUILayout.PropertyField(meleeAttackSound_SFX);
                EditorGUILayout.PropertyField(hasMeleeCombo);
                if (hasMeleeCombo.boolValue)
                {
                    EditorGUILayout.PropertyField(meleeAttackDamages, true);
                }
                break;

            case EnemyDataSO.attackTypes.Investida:
                EditorGUILayout.PropertyField(attackDamage);
                EditorGUILayout.PropertyField(attackRate);
                EditorGUILayout.PropertyField(attackSound_SFX);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
#endregion
