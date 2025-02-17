using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.PlayerLoop;

public class UI_PlayerManager : MonoBehaviour
{
    public delegate void AmmoDataUpdate(PlayerInventory.ammoTypeOfGunEquipped ammoType);
    public AmmoDataUpdate OnAmmoUpdate;

    public PlayerInventory playerInventory;
    public GameObject player;
    public TMP_Text currentMagazineText, currentAmmoInInventory;

    private void Start()
    {
        HideOrShowGUI_Content(false);
        playerInventory = player.GetComponent<PlayerInventory>();
        CheckGunEquipped();
        OnAmmoUpdate += UpdateOnlyAmmoInventory;
        
    }

    private void CheckGunEquipped()
    {
        if(playerInventory.gunEquipped != null)
        {
            HideOrShowGUI_Content(true);
            Debug.Log("Isso aqui não deveria ser chamado se eu não iniciasse o jogo sem uma arma equipada");
            currentMagazineText.text = playerInventory.gunEquipped.GetComponent<Gun_Attributes>().magazine.ToString();
            switch (playerInventory.AmmoType)
            {
                case PlayerInventory.ammoTypeOfGunEquipped.AR:
                    currentAmmoInInventory.text = playerInventory.ARammount.ToString();
                    break;
                case PlayerInventory.ammoTypeOfGunEquipped.Sub:
                    currentAmmoInInventory.text = playerInventory.subAmmount.ToString();
                    break;
                case PlayerInventory.ammoTypeOfGunEquipped.Shotgun:
                    currentAmmoInInventory.text = playerInventory.shotgunAmmount.ToString();
                    break;
                case PlayerInventory.ammoTypeOfGunEquipped.Pistol:
                    currentAmmoInInventory.text = playerInventory.pistolAmmount.ToString();
                    break;
                case PlayerInventory.ammoTypeOfGunEquipped.Granade_Launcher:
                    currentAmmoInInventory.text = playerInventory.granadeLauncherAmmount.ToString();
                    break;
                case PlayerInventory.ammoTypeOfGunEquipped.Rocket_Launcher:
                    currentAmmoInInventory.text = playerInventory.rocketLauncherAmmount.ToString();
                    break;
                case PlayerInventory.ammoTypeOfGunEquipped.FlameThrower:
                    currentAmmoInInventory.text = playerInventory.flamethrowerAmmount.ToString();
                    break;

            }
        }
        
    }

    public void UpdateOnlyCurrentMagazine(int mag)
    {
        HideOrShowGUI_Content(true);
        currentMagazineText.text = mag.ToString();
    }
    public void UpdateOnlyAmmoInventory(PlayerInventory.ammoTypeOfGunEquipped ammoType)
    {
        if (playerInventory.gunEquipped == null) return;
        HideOrShowGUI_Content(true);
        if (playerInventory.gunEquipped.GetComponent<Gun_Attributes>().typeOfAmmo == ammoType)
        {
            switch (ammoType)
            {
                case PlayerInventory.ammoTypeOfGunEquipped.Pistol:
                    currentAmmoInInventory.text = playerInventory.pistolAmmount.ToString();
                    break;
                case PlayerInventory.ammoTypeOfGunEquipped.AR:
                    currentAmmoInInventory.text = playerInventory.ARammount.ToString();
                    break;
                case PlayerInventory.ammoTypeOfGunEquipped.Sub:
                    currentAmmoInInventory.text = playerInventory.subAmmount.ToString();
                    break;
                case PlayerInventory.ammoTypeOfGunEquipped.Shotgun:
                    currentAmmoInInventory.text = playerInventory.shotgunAmmount.ToString();
                    break;
                case PlayerInventory.ammoTypeOfGunEquipped.Granade_Launcher:
                    currentAmmoInInventory.text = playerInventory.granadeLauncherAmmount.ToString();
                    break;
                case PlayerInventory.ammoTypeOfGunEquipped.Rocket_Launcher:
                    currentAmmoInInventory.text = playerInventory.rocketLauncherAmmount.ToString();
                    break;
                case PlayerInventory.ammoTypeOfGunEquipped.FlameThrower:
                    currentAmmoInInventory.text = playerInventory.flamethrowerAmmount.ToString();
                    break;
            }
        }
        
    }
    public void UpdateBothCurrentMagAndAmmoInventory(int mag, int ammo) 
    {
        HideOrShowGUI_Content(true);
        currentMagazineText.text = mag.ToString();
        currentAmmoInInventory.text = ammo.ToString();
    }

    public void SwapWeaponUpdate()
    {
        playerInventory = player.GetComponent<PlayerInventory>();
        currentMagazineText.text = playerInventory.gunEquipped.GetComponent<Gun_Attributes>().actual_magazine.ToString();
        

        switch (playerInventory.AmmoType)
        {
            case PlayerInventory.ammoTypeOfGunEquipped.AR:
                currentAmmoInInventory.text = playerInventory.ARammount.ToString();
                break;
            case PlayerInventory.ammoTypeOfGunEquipped.Sub:
                currentAmmoInInventory.text = playerInventory.subAmmount.ToString();
                break;
            case PlayerInventory.ammoTypeOfGunEquipped.Shotgun:
                currentAmmoInInventory.text = playerInventory.shotgunAmmount.ToString();
                break;
            case PlayerInventory.ammoTypeOfGunEquipped.Pistol:
                currentAmmoInInventory.text = playerInventory.pistolAmmount.ToString();
                break;
            case PlayerInventory.ammoTypeOfGunEquipped.Granade_Launcher:
                currentAmmoInInventory.text = playerInventory.granadeLauncherAmmount.ToString();
                break;
            case PlayerInventory.ammoTypeOfGunEquipped.Rocket_Launcher:
                currentAmmoInInventory.text = playerInventory.rocketLauncherAmmount.ToString();
                break;
            case PlayerInventory.ammoTypeOfGunEquipped.FlameThrower:
                currentAmmoInInventory.text = playerInventory.flamethrowerAmmount.ToString();
                break;

        }
        UpdateOnlyCurrentMagazine(playerInventory.gunEquipped.GetComponent<Gun_Attributes>().actual_magazine);
    }

    void HideOrShowGUI_Content(bool ActiveState)
    {
        currentAmmoInInventory.enabled = ActiveState;
        currentMagazineText.enabled = ActiveState;
    }
}
