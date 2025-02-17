using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPlayerCollecter : MonoBehaviour, ICollectable
{
    public GameObject gunPrefab;
    public GameObject gunInstancied;
    private PlayerInventory playerInventory;
    [SerializeField]private GunInfos windowInfo;
    private Gun_Attributes gunAttributes;
    public SpriteRenderer spriteRenderer;
    private bool collected = false;
    private GunWindowInfo g;
    private bool alreadyCollected;
    
    void Start()
    {
        alreadyCollected = false;
        gunInstancied = Instantiate(gunPrefab);
        
        playerInventory = FindObjectOfType<PlayerInventory>();
        gunAttributes = gunInstancied.GetComponent<Gun_Attributes>();

        
        windowInfo.gunSprite = gunAttributes.gunSprite;
        windowInfo.damageInfo = gunAttributes.gunDamage;
        windowInfo.cadencyInfo = gunAttributes.cadency;

        gunInstancied.SetActive(false);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = windowInfo.gunSprite;
    }

    public void Collect()
    {
        if (collected) return;
        GameObject previusGun = playerInventory.gunEquipped;
        collected = true;
        if (playerInventory.weaponsOnHold.Count < playerInventory.numeroMaximoDeArmasEmMao) 
        {
            
            gameObject.SetActive(false);
        }
        else
        {
            previusGun = playerInventory.gunEquipped;
            collected = false;
        }
        playerInventory.GetOrChangeWeaponSelected(gunInstancied, alreadyCollected);
        if (previusGun != null) ChangeWeaponGiver(previusGun);

    }

    private void ChangeWeaponGiver(GameObject PreviusGun)
    {
        alreadyCollected = true;
        gunInstancied = PreviusGun;
        gunAttributes = gunInstancied.GetComponent<Gun_Attributes>();


        windowInfo.gunSprite = gunAttributes.gunSprite;
        windowInfo.damageInfo = gunAttributes.gunDamage;
        windowInfo.cadencyInfo = gunAttributes.cadency;
        spriteRenderer.sprite = windowInfo.gunSprite;
        g?.UpdateInfo(transform, playerInventory.gunEquipped.GetComponent<Gun_Attributes>(), gunAttributes);
    }

    public void ShowInfos(GameObject uiWindowManager, Gun_Attributes onHandGunAttributes)
    {
        g = uiWindowManager.GetComponent<GunWindowInfo>();
        g.gunSprite = windowInfo.gunSprite;
        g.damageInfo = windowInfo.damageInfo;
        g.cadencyInfo = windowInfo.cadencyInfo;
        if (onHandGunAttributes == null)
        {
            g.UpdateInfo(transform, null, gunAttributes);
        }
        else
        {
            g.UpdateInfo(transform, onHandGunAttributes, gunAttributes);
        }
        
    }
}

[System.Serializable]
public class GunInfos
{
    public Sprite gunSprite;
    public int damageInfo;
    public float cadencyInfo;
}
