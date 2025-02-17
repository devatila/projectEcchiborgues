using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Project Classes/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [Tooltip("Usar este campo para definir o sprite usado na arma")]
    public Sprite gunSprite;
    public string gunName = "Nome da Arma Aqui Pls"; //Por favor não esquecer pelo nosso bem

    [Tooltip("Defina 1 para arma de uma mão, 2 para arma de duas mãos")]
    public int gunId = 1; //0 = Quando o player está sem arma; gunId > 0 = quando o player tem alguma arma na mão
    // ENTRE 1 E 2 POIS ISSO DEFINIRA SE A ARMA É DE UMA MÃO OU DUAS MÃOS

    public int gunDamage;
    public float cadency;


    public float minSpread;
    public float maxSpread;
    public float decreaseSpreadSpeed, SpreadForce;
    [HideInInspector] public float actualSpread;


    public bool reloadable;
    public int magazine = 10;

    private int actual_magazine;
    public float reloadTime = 3f;
    private bool isReloading;

    public bool isShotgun;
    public int shotgunFragmentsQuantitative = 10;
    public float MaxDistance;
    public float damageReductionRate = 0.5f;

    public bool isSpecialProjectile;
    public GameObject customProjectile;
    public bool instantiatedProjectile; // O Projetil ja está instanciado? 

    public int AmmoCostPerTick;

    public PlayerInventory.ammoTypeOfGunEquipped ammoType;
}

#if UNITY_EDITOR

[CustomEditor(typeof(WeaponDataSO))]
public class WeaponDataSOEditor : Editor
{
    // Variáveis para controlar os foldouts
    private bool spreadFoldout = true;
    private bool shotgunFoldout = true;
    private bool specialProjectileFoldout = true;

    public override void OnInspectorGUI()
    {
        // Referência ao objeto sendo editado
        WeaponDataSO weaponData = (WeaponDataSO)target;

        // Adicionando cabeçalhos
        EditorGUILayout.LabelField("Weapon Settings", EditorStyles.boldLabel);

        // Exibindo variáveis com Tooltip e Slider
        weaponData.gunSprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Gun Sprite", "Usar este campo para definir o sprite usado na arma"), weaponData.gunSprite, typeof(Sprite), false);
        weaponData.gunName = EditorGUILayout.TextField("Gun Name", weaponData.gunName);
        weaponData.gunId = EditorGUILayout.IntField("Gun ID", weaponData.gunId);
        weaponData.gunDamage = EditorGUILayout.IntField("Gun Damage", weaponData.gunDamage);
        weaponData.cadency = EditorGUILayout.FloatField("Cadency", weaponData.cadency);
        weaponData.ammoType = (PlayerInventory.ammoTypeOfGunEquipped)EditorGUILayout.EnumPopup("Ammo Type",weaponData.ammoType);

        if (weaponData.ammoType == PlayerInventory.ammoTypeOfGunEquipped.FlameThrower)
        {
            weaponData.AmmoCostPerTick = EditorGUILayout.IntField("Ammo Cost Per Tick", weaponData.AmmoCostPerTick);
        }

        // Exemplo de variáveis em linha horizontal
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Min Spread", GUILayout.Width(80)); // Ajusta a largura do label
        weaponData.minSpread = EditorGUILayout.FloatField(weaponData.minSpread);

        EditorGUILayout.LabelField("Max Spread", GUILayout.Width(80)); // Ajusta a largura do label
        weaponData.maxSpread = EditorGUILayout.FloatField(weaponData.maxSpread);
        EditorGUILayout.EndHorizontal();

        // Foldout para agrupar os spreads
        spreadFoldout = EditorGUILayout.Foldout(spreadFoldout, "Spread Settings");
        if (spreadFoldout)
        {
            // Slider para campos com Range
            weaponData.decreaseSpreadSpeed = EditorGUILayout.Slider("Decrease Spread Speed", weaponData.decreaseSpreadSpeed, 0f, 10f);
            weaponData.SpreadForce = EditorGUILayout.FloatField("Spread Force", weaponData.SpreadForce);
        }

        // Campo para arma com recarga
        weaponData.reloadable = EditorGUILayout.Toggle("Reloadable", weaponData.reloadable);
        if (weaponData.reloadable)
        {
            weaponData.magazine = EditorGUILayout.IntField("Magazine Capacity", weaponData.magazine);
            weaponData.reloadTime = EditorGUILayout.FloatField("Reload Time", weaponData.reloadTime);
        }

        // Campo para verificar se é Shotgun
        weaponData.isShotgun = EditorGUILayout.Toggle("Is Shotgun", weaponData.isShotgun);
        if (weaponData.isShotgun)
        {
            shotgunFoldout = EditorGUILayout.Foldout(shotgunFoldout, "Shotgun Settings");
            if (shotgunFoldout)
            {
                // Mostrar apenas se for shotgun
                weaponData.shotgunFragmentsQuantitative = EditorGUILayout.IntField("Shotgun Fragments", weaponData.shotgunFragmentsQuantitative);
                weaponData.MaxDistance = EditorGUILayout.FloatField("Max Distance", weaponData.MaxDistance);
                weaponData.damageReductionRate = EditorGUILayout.FloatField("Damage Reduction Rate", weaponData.damageReductionRate);
                
            }
        }

        weaponData.isSpecialProjectile = EditorGUILayout.Toggle("Is Special Projectile", weaponData.isSpecialProjectile);
        if (weaponData.isSpecialProjectile)
        {
            specialProjectileFoldout = EditorGUILayout.Foldout(specialProjectileFoldout, "Projectile Custom Set");
            if (specialProjectileFoldout)
            {
                weaponData.customProjectile = (GameObject)EditorGUILayout.ObjectField("Projectile Prefab", weaponData.customProjectile, typeof(GameObject), false);
                weaponData.instantiatedProjectile = EditorGUILayout.Toggle(new GUIContent("Instantiated Projectile", "Marque se o projetil a ser utilizado ja estiver instanciado"), weaponData.instantiatedProjectile);
            }
        }

        // Sempre necessário para salvar as alterações no ScriptableObject
        EditorUtility.SetDirty(weaponData);
    }
}
#endif
