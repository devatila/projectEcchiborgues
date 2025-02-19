using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PerkManager : MonoBehaviour
{
    public static PerkManager Instance;

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
        }
    }
    public class ThrowablePerks
    {
        public NinjaStar ninjaStar = new NinjaStar();
        public class NinjaStar
        {
            public bool isIncreasePerkActivated = false;
            public int increasedAmmount;
            public event Action<int> OnIncreaseAmmount;
            public void ActivateIncreaseAmmount(int newTotalAmmount)
            {
                OnIncreaseAmmount?.Invoke(newTotalAmmount);
                isIncreasePerkActivated = true;
                increasedAmmount = newTotalAmmount;
            }
        }
    }

    public ThrowablePerks throwablePerks = new ThrowablePerks();
}
