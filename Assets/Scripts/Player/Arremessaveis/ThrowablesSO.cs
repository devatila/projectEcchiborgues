using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Throwable Data", menuName = "Project Classes/New Throwable")]
public class ThrowablesSO : ScriptableObject
{
    public ThroableObjects throwableType;

    public bool canBeShooted = true;
    public int damage;
    public float speed;

    // Região de Explosivos
    public float delayToExplode; // Se for Impact Granade, é só por = 0

    // Aahhh eu vou delirar como o nego bamm heheheh
}
