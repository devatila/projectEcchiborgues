using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PerkManager : MonoBehaviour
{
    // JÁ ADIANTO QUE ESTE CÓDIGO PODE E VAI FICAR ENORME QUANDO EU ACABAR COM TUDOO
    public static PerkManager Instance;
    public int ammount = 10;
    private void Awake()
    {
        Instance = this;
    }


    public ThrowablePerks   throwablePerks  = new ThrowablePerks();
    public PlayerPerks      playerPerks     = new PlayerPerks();
    public EnemyPerks       enemyPerks      = new EnemyPerks();
    public class ThrowablePerks
    {
        public GenenericThrowables genenericThrowables  = new GenenericThrowables();
        public NinjaStar ninjaStar                      = new NinjaStar();
        public ThrowableKnife throwableKnife            = new ThrowableKnife();
        public Molotov molotov                          = new Molotov();
        public Granade granade                          = new Granade();
        public ImpactGranade impactGranade              = new ImpactGranade();
        public FragGranade fragGranade                  = new FragGranade();
        public PEMGranade pemGranade                    = new PEMGranade();
        public Dinamyte dinamyte                        = new Dinamyte();

        public class GenenericThrowables
        {

        }
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
                isBounceActivated = canRicochete; // Errei mas já ajeitei o nome dessa variavel,ATENÇÃO ATILA DO FUTURO: LER AS VARIAVEIS QUE TA COLOCANDO
                OnNinjaStarRicocheteAllow?.Invoke(canRicochete);
            }

        }

        public class ThrowableKnife
        {

        }

        public class Molotov
        {

        }

        public class Granade
        {

        }

        public class FragGranade
        {

        }

        public class ImpactGranade
        {

        }

        public class PEMGranade
        {

        }

        public class Dinamyte

        {

        }
    }
    
    public class PlayerPerks
    {

    }

    public class EnemyPerks // Não perks que buffem inimigos, apenas os custo beneficios heheh
    {

    }
    
}
