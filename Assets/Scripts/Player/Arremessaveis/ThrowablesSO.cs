using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Throwable Data", menuName = "Project Classes/New Throwable")]
public class ThrowablesSO : ScriptableObject
{
    // NÃO ESQUECER DE CHECAR OS RANGES NO OBJETOOOOO
    public ThroableObjects throwableType;

    public bool canBeShooted;
    public int damage;
    public float speed;
    public float rotationSpeed;
    public bool isContinuous;
    public bool isBounceable;
    public bool instaActivate;
    public bool autoDeactivate;
    public LayerMask cullingMask;

    // Região de Explosivos
    public float delayToExplode; // Se for Impact Granade, é só por = 0
    public float explosionForce;
    public float explosionRadius;

    // Região dos Frags
    public int howManyFrags;
    public bool canBounce; // Reutilizarei nos arremessaveis
    public int maxBounces; // e isso também

    // Região do PEM
    public float stunDuration;
    public float effectProbability;

    // Região de Outros Arremessaveis
    public bool canApplySomeEffectOnHit;
    public int ninjaStarQuantities;

    
}

#if UNITY_EDITOR

[CustomEditor(typeof(ThrowablesSO))]
public class ThrowableDataEditor: Editor
{
    public override void OnInspectorGUI()
    {
        // Isso deve pegar as referencias da classe que eu quero
        ThrowablesSO data = (ThrowablesSO)target;

        EditorGUILayout.LabelField("Basics Settings", EditorStyles.boldLabel);

        // Mostrando as variáveis genéricas
        data.damage = EditorGUILayout.IntField("Damage", data.damage);
        data.speed = EditorGUILayout.FloatField("Speed", data.speed);
        data.canBeShooted  = EditorGUILayout.Toggle(new GUIContent("Can Be Shot", "Marcar se algo acontecer ao atirar no objeto"), data.canBeShooted);
        data.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", data.rotationSpeed);
        data.isContinuous = EditorGUILayout.Toggle(new GUIContent("Is Continuous?", "Marque se o objeto deve parar proximo a posição do mouse"), data.isContinuous);
        if(data.isContinuous) data.isBounceable = EditorGUILayout.Toggle(new GUIContent("Is Bounceable?", "Marque se o objeto deve ricochetear em paredes"), data.isBounceable);
        data.instaActivate = EditorGUILayout.Toggle(new GUIContent("Insta Activate?", "Marque se o arremessavel deve aplicar seu efeito instantaneamente"), data.instaActivate);
        data.autoDeactivate = EditorGUILayout.Toggle(new GUIContent("Auto Deactivate?", "Marque se o objeto deve parar proximo a posição do mouse"), data.autoDeactivate);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specifics Settings", EditorStyles.boldLabel);
        // Hora da verdadeira brincadeira
        data.throwableType = (ThroableObjects)EditorGUILayout.EnumPopup("Throwable Type", data.throwableType);

        // Esse switch vai serializar as variaveis determinadas para tal tipo de arremessável
        switch (data.throwableType)
        {
            case ThroableObjects.Granade:
                data.explosionForce = EditorGUILayout.FloatField("Explosion Force", data.explosionForce);
                data.explosionRadius = EditorGUILayout.FloatField("Explosion Radius", data.explosionRadius);
                data.delayToExplode = EditorGUILayout.FloatField("Delay To Explode", data.delayToExplode);
                break;
            case ThroableObjects.FragGranade:
                data.howManyFrags = EditorGUILayout.IntField("How Many Frags", data.howManyFrags);

                data.canBounce = EditorGUILayout.Toggle("Can Bounce", data.canBounce);
                if (data.canBounce)
                {
                    EditorGUI.indentLevel++;
                    data.maxBounces = EditorGUILayout.IntField("Max Bounces", data.maxBounces);
                    EditorGUI.indentLevel--;
                }

                data.explosionForce = EditorGUILayout.FloatField("Explosion Force", data.explosionForce);
                data.explosionRadius = EditorGUILayout.FloatField("Explosion Radius", data.explosionRadius);
                data.delayToExplode = EditorGUILayout.FloatField("Delay To Explode", data.delayToExplode);
                break;

            case ThroableObjects.ImpactGranade:
                data.explosionForce = EditorGUILayout.FloatField("Explosion Force", data.explosionForce);
                data.explosionRadius = EditorGUILayout.FloatField("Explosion Radius", data.explosionRadius);
                data.delayToExplode = 0f;
                data.isContinuous = false;
                data.instaActivate = false;
                
                break;

            case ThroableObjects.PEM_Granade:
                data.stunDuration = EditorGUILayout.FloatField("Stun Duration", data.stunDuration);
                data.effectProbability = EditorGUILayout.FloatField("Eletricity Probability", data.effectProbability);
                data.explosionRadius = EditorGUILayout.FloatField("Effect Range", data.explosionRadius);
                break;
            case ThroableObjects.Molotov:
                data.delayToExplode = 0f;
                data.effectProbability = EditorGUILayout.FloatField("Fire Probability", data.effectProbability);
                data.explosionRadius = EditorGUILayout.FloatField("Effect Range", data.explosionRadius);

                break;

            case ThroableObjects.ThrowingKnife:
                data.canBounce = EditorGUILayout.Toggle("Can Bounce", data.canBounce);
                if (data.canBounce)
                {
                    EditorGUI.indentLevel++;
                    data.maxBounces = EditorGUILayout.IntField("Max Bounces", data.maxBounces);
                    EditorGUI.indentLevel--;
                }

                break;
            case ThroableObjects.Dynamite:
                data.explosionForce = EditorGUILayout.FloatField("Explosion Force", data.explosionForce);
                data.explosionRadius = EditorGUILayout.FloatField("Explosion Radius", data.explosionRadius);
                data.delayToExplode = EditorGUILayout.FloatField("Delay To Explode", data.delayToExplode);
                break;

            case ThroableObjects.NinjaStar:
                data.instaActivate = true;
                data.isContinuous = false;
                data.ninjaStarQuantities = EditorGUILayout.IntField("Ninja Stars Ammount", data.ninjaStarQuantities);
                data.canBounce = EditorGUILayout.Toggle("Can Bounce", data.canBounce);
                if (data.canBounce)
                {
                    EditorGUI.indentLevel++;
                    data.maxBounces = EditorGUILayout.IntField("Max Bounces", data.maxBounces);
                    EditorGUI.indentLevel--;
                }
                break;

        }

        if(EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(data);

    }
}
#endif