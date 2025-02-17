using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public Gun_Shoot _gunShoot;
    public PlayerInventory _playerInventory;
    public GameObject player;
    public UI_PlayerManager UIplayerManager;

    // Start is called before the first frame update
    void Start()
    {
        _playerInventory = player.GetComponent<PlayerInventory>();
        try
        {
            _gunShoot = _playerInventory.gunEquipped.GetComponent<Gun_Shoot>();
            _gunShoot.OnShootUpdate += UIplayerManager.UpdateOnlyCurrentMagazine;
        }
        catch
        {
            _gunShoot = null;
        }
        _playerInventory.OnReloaded += UIplayerManager.UpdateBothCurrentMagAndAmmoInventory;
        _playerInventory.OnSwapWeapon += UIplayerManager.SwapWeaponUpdate;
        _playerInventory.OnSwapWeapon += onSwapGun;

        _playerInventory.OnGetWeapon += onSwapGun;
        _playerInventory.OnGetWeapon += UIplayerManager.SwapWeaponUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onSwapGun()
    {
        _gunShoot = _playerInventory.gunEquipped.GetComponent<Gun_Shoot>();
        _gunShoot.OnShootUpdate += UIplayerManager.UpdateOnlyCurrentMagazine;
    }
}
