using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewPerkManager : MonoBehaviour
{
    public List<PerkBase> perkList = new List<PerkBase>();
    public List<PerkSO> availablePerks = new List<PerkSO>();

    private Dictionary<Type, PerkBase> perkInstances = new Dictionary<Type, PerkBase>();
    private Dictionary<string, PerkBase> perkByName = new Dictionary<string, PerkBase>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ApplyPerkByName("SpeedBoost3x");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            RemovePerkByName("SpeedBoost3x");
        }
    }

    public void ApplyPerkByName(string perkName)
    {
        PerkSO perk = availablePerks.Find(p => p.perkName == perkName);
        if (perk != null)
        {
            ApplyPerk(perk);
        }
        else
        {
            Debug.LogWarning("Nome do Perk informado não está referenciado corretamente");
        }
    }

    public void RemovePerkByName(string perkName = "")
    {
        if (perkByName.TryGetValue(perkName, out PerkBase perk))
        {
            perk.OnRemove();
            perkByName.Remove(perkName);
            Debug.Log($"Perk '{perkName}' removido com sucesso.");
        }
        else
        {
            Debug.LogWarning($"Perk '{perkName}' não encontrado.");
        }
    }

    // Aplica e adiciona o perk na lista de perk ativos
    void ApplyPerk(PerkSO perk)
    {
        Type perkType = perk.GetPerkType();
        if (perkInstances.TryGetValue(perkType, out PerkBase existingPerk))
        {
            if (existingPerk.IsExpired)
            {
                existingPerk.OnApply();
                perkList.Add(existingPerk);
                perkByName.Add(perk.perkName, existingPerk);
                Debug.Log("Perk Previamente Instanciado Foi Reaplicado");
            }
            else
            {
                Debug.Log("Perk Ja Existente Na Lista");
            }
        }
        else
        {
            PerkBase newPerk = perk.CreatePerkInstance();
            newPerk.OnApply();
            perkList.Add(newPerk);
            perkInstances.Add(perkType, newPerk);
            perkByName.Add(perk.perkName, newPerk);
            Debug.Log("Novo perk aplicado.");
        }

    }

    // Remove ("Desaplica") o perk e o tira da lista de perks ativos
    // Utilizar este quando realmente for necessário e/ou ao fim de um dia (fase)
    void RemovePerk(PerkBase perk)
    {
        perk.OnRemove();
        perkList.Remove(perk);
    }
}
