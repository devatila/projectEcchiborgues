using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashAdd : MonoBehaviour,ICollectable
{
    public int cashToAdd;
    public PlayerInventory Player;
    private Sprite cashSprite;
    private void Start()
    {
        Player = FindObjectOfType<PlayerInventory>();
        Player.AddCash(cashToAdd);
        PlayEffect();
        Invoke("AutoDeactivate", 3);
    }
    public void Collect()
    {
        Player.AddCash(cashToAdd);
    }
    public void ShowInfos(GameObject uiWindowManager, Gun_Attributes attributesToCompare)
    {
        GunWindowInfo g = uiWindowManager.GetComponent<GunWindowInfo>();
        g.UpdateAmmoInfoGUI(transform, cashToAdd, cashSprite);
        
    }

    void PlayEffect()
    {
        //Tocando Efeito...dps eu faço
    }

    void AutoDeactivate()
    {
        this.gameObject.SetActive(false);
    }
}
