using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBasics : MonoBehaviour
{
    public int bossRNG; //Número que vai definir o que o boss vai fazer em seguida
    public float timeToResponse = 3f; //O tempo que vai levar para um novo RNG ser gerado
    private float actualTime;
    public enum animStates {idle, walking, running}
    public animStates bossAnimationStates;

    public enum states {Approaching, Retreating, HandAttack, ChargingShoot, ShootingChargedShoot, HealingItSelf, Stunned, shootingWhileWalkOrRun}
    public states bossStates;

    private void Start()
    {
        StartCoroutine(ChangeStateRandomly());
    }

    IEnumerator ChangeStateRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToResponse); // Espera 3 segundos
            ChangeState(); // Chama o método para alterar o estado aleatoriamente
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChangeState()
    {
        // Pega todos os valores do enum
        states[] states = (states[])System.Enum.GetValues(typeof(states));
        // Escolhe um valor aleatório do array de estados
        bossStates = states[Random.Range(0, states.Length)];
        Debug.Log("Novo estado do Boss: " + bossStates);
    }
}
