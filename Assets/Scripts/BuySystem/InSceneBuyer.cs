using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSceneBuyer : MonoBehaviour,IBuyableInScene
{
    public int value; //Valor a ser cobrado do player
    public int discount; // em Porcentagem

    public void Buy(int playerCash)
    {
        playerCash -= value;
        Debug.Log("Comprou");
    }

    public void ShowStatsAndInfos()
    {
        return;
    }
}
