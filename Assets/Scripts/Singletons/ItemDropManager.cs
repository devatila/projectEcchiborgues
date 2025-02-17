using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    public static ItemDropManager Instance;
    // Já que o dinheiro ja vai diretamente para o player, apenas um efeito basico
    public GameObject cashEffectPrefab;
    public GameObject ammoDropPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void OnEnemyDeath(Vector3 deathPosition)
    {
        // O sistema do jogo escolherá aleatóriamente entre dropar munição ou cash
        int i = Random.Range(0, 3); //0 = Cash | 1 = Ammo | 2 = Nothing | o maximo é exclusivo

        switch (i)
        {
            case 0:
                GameObject cashObj = Instantiate(cashEffectPrefab, deathPosition, Quaternion.identity);
                break;

            case 1:
                GameObject ammoObj = Instantiate(ammoDropPrefab, deathPosition, Quaternion.identity);
                break;

            case 2:
                return; // kkkk nada e nada q porra eh essa
                

        }
        
    }

    
}
