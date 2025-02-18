using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Throwable Data", menuName = "Project Classes/New Throwable")]
public class ThrowablesSO : ScriptableObject
{
    // NÃO ESQUECER DE CHECAR OS RANGES NO OBJETOOOOO
    public ThroableObjects throwableType;

    public bool canBeShooted = true;
    public int damage;
    public float speed;
    public float rotationSpeed;
    public bool isContinuous;
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

    
}

[CustomEditor(typeof(ThrowablesSO))]
public class ThrowableDataEditor: Editor
{
    public override void OnInspectorGUI()
    {
        // Isso deve pegar as referencias da classe que eu quero
        ThrowablesSO data = (ThrowablesSO)target;

        // Mostrando as variáveis genéricas
        data.damage = EditorGUILayout.IntField("Damage", data.damage);
        data.speed = EditorGUILayout.FloatField("Damage", data.speed);
        data.canBeShooted  = EditorGUILayout.Toggle(new GUIContent("Can Be Shot", "Marcar se algo acontecer ao atirar no objeto"), data.canBeShooted);
        data.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", data.rotationSpeed);
        data.isContinuous = EditorGUILayout.Toggle(new GUIContent("Is Continuous?", "Marque se o objeto deve parar proximo a posição do mouse"), data.isContinuous);

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
                
                break;

            case ThroableObjects.PEM_Granade:
                data.stunDuration = EditorGUILayout.FloatField("Stun Duration", data.stunDuration);
                data.effectProbability = EditorGUILayout.FloatField("Eletricity Probability", data.effectProbability);
                data.explosionRadius = EditorGUILayout.FloatField("Effect Range", data.explosionRadius);
                break;
            // Continuar o resto amanhã
        }

    }
}
