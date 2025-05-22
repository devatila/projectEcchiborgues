using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForPlayerCollectable : MonoBehaviour, ICollectable
{
    public bool isRandom;
    private GameObject Player;
    private UI_PlayerManager PlayerManager;
    public Sprite ammoSprite;
    public PlayerInventory.ammoTypeOfGunEquipped ammoType;
    private PlayerInventory pInventory;
    public int ammoAmount = 30;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        pInventory = Player.GetComponent<PlayerInventory>();

        PlayerManager = GameObject.FindObjectOfType<UI_PlayerManager>();
        StartRandom();
    }

    public void Collect()
    {
        Debug.Log("Pegou " + ammoType + " Tipo de Munição");
        pInventory = Player.GetComponent<PlayerInventory>();
        switch (ammoType)
        {
            case PlayerInventory.ammoTypeOfGunEquipped.AR:
                pInventory.ARammount += ammoAmount;
                break;
            case PlayerInventory.ammoTypeOfGunEquipped.Sub:
                pInventory.subAmmount += ammoAmount;
                break;
            case PlayerInventory.ammoTypeOfGunEquipped.Shotgun:
                pInventory.shotgunAmmount += ammoAmount;
                break;
            case PlayerInventory.ammoTypeOfGunEquipped.Pistol:
                pInventory.pistolAmmount += ammoAmount;
                break;
            case PlayerInventory.ammoTypeOfGunEquipped.FlameThrower:
                pInventory.flamethrowerAmmount += ammoAmount;
                break;
            case PlayerInventory.ammoTypeOfGunEquipped.Granade_Launcher:
                pInventory.granadeLauncherAmmount += ammoAmount;
                break;
            
        }
        PlayerManager.OnAmmoUpdate(ammoType);
        
        gameObject.SetActive(false);
        
    }

    public void ShowInfos(GameObject uiManager, Gun_Attributes gunAt)
    {
        GunWindowInfo g = uiManager.GetComponent<GunWindowInfo>();
        g.UpdateAmmoInfoGUI(transform, ammoAmount, ammoSprite);
    }

    private T GetValueRandom<T>(Dictionary<T, int> probabilities) where T : System.Enum
    {
        List<T> weightedList = new List<T>();

        foreach (var entry in probabilities)
        {
            for (int i = 0; i < entry.Value; i++)
            {
                weightedList.Add(entry.Key);
            }
        }

        return weightedList[Random.Range(0, weightedList.Count)];
    }

    void StartRandom()
    {
        if (isRandom)
        {
            // Obter o tipo de munição da arma equipada
            PlayerInventory.ammoTypeOfGunEquipped equippedAmmoType =
                pInventory.equippedGunAttributes != null ? pInventory.equippedGunAttributes.typeOfAmmo : PlayerInventory.ammoTypeOfGunEquipped.AR;

            // Definir pesos dinamicamente
            var probabilities = new Dictionary<PlayerInventory.ammoTypeOfGunEquipped, int>();

            foreach (PlayerInventory.ammoTypeOfGunEquipped ammoType in System.Enum.GetValues(typeof(PlayerInventory.ammoTypeOfGunEquipped)))
            {
                probabilities[ammoType] = (ammoType == equippedAmmoType) ? 75 : 10;
            }

            // Obter um valor aleatório baseado nos pesos dinâmicos
            ammoType = GetValueRandom(probabilities);

            // Definir a quantidade de munição de forma randômica
            ammoAmount = (Random.Range(0, 2) == 0) ? 25 : 40;
        }
    }

}
