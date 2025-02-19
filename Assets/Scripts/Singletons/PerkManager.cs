using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PerkManager : MonoBehaviour
{
    // JÁ ADIANTO QUE ESTE CÓDIGO PODE E VAI FICAR ENORME QUANDO EU ACABAR COM TUDOO
    public static PerkManager Instance;
    public bool debugActivate;
    public int testNewAmmountNinjaStar = 3;
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            throwablePerks.ninjaStar.ActivateIncreaseAmmount(testNewAmmountNinjaStar);
            throwablePerks.ninjaStar.ActivateRicocheteOnNinjaStar(true);
        }

        debugActivate = throwablePerks.ninjaStar.isBounceActivated;
    }
    public ThrowablePerks throwablePerks = new ThrowablePerks();
    public class ThrowablePerks
    {
        public NinjaStar ninjaStar = new NinjaStar();

        public class NinjaStar
        {
            // Região De Increase Perks
            public bool isIncreasePerkActivated = false;
            public int increasedAmmount;
            public event Action<int> OnIncreaseAmmount;
            public void ActivateIncreaseAmmount(int newTotalAmmount)
            {
                OnIncreaseAmmount?.Invoke(newTotalAmmount);
                isIncreasePerkActivated = true;
                increasedAmmount = newTotalAmmount;
            }

            // Região de Ricochete
            public bool isBounceActivated = false;
            public event Action<bool> OnNinjaStarRicocheteAllow;
            public void ActivateRicocheteOnNinjaStar(bool canRicochete)
            {
                isBounceActivated = canRicochete; // VAI TOMA NO RABO DE MIM MESMO NAMORALLL
                OnNinjaStarRicocheteAllow?.Invoke(canRicochete);
            }

        }
    }

    
}
