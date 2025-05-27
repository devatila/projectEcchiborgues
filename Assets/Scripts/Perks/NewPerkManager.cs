using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewPerkManager : MonoBehaviour
{
    public static NewPerkManager Instance;

    public List<PerkBase> perkList = new List<PerkBase>();
    public List<PerkSO> availablePerks = new List<PerkSO>();

    private Dictionary<string, PerkBase> perkInstances = new Dictionary<string, PerkBase>();
    private Dictionary<string, PerkBase> perkByName = new Dictionary<string, PerkBase>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SubscribingEvents();
    }
    void SubscribingEvents()
    {
        RaidManager instance = RaidManager.instance;
        if (instance != null)
        {
            instance.OnEndSubRaid += ShowPerkSelection;
            instance.OnEndSubRaid += UpdatePerksStats;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ShowPerkSelection();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            RemovePerkByName("SpeedBoost3x");
        }

        if (perkList.Count > 0)
        {
            float time = Time.deltaTime;
            foreach (var perk in perkList)
            {
                perk.Update(time);
            }
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
    public void ApplyPerk(PerkSO perk)
    {
        string perkName = perk.perkName;
        if (perkInstances.TryGetValue(perkName, out PerkBase existingPerk))
        {
            if (existingPerk.IsExpired)
            {
                existingPerk.OnApply();
                perkList.Add(existingPerk);
                perkByName[perkName] = existingPerk; // Atualiza a referência no dicionário por nome
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
            perkInstances[perkName] = newPerk;
            perkByName[perkName] = newPerk;
            Debug.Log("Novo perk aplicado.");
        }

    }

    // Remove ("Desaplica") o perk e o tira da lista de perks ativos
    // Utilizar este quando realmente for necessário e/ou ao fim de um dia (fase)
    public void RemovePerk(PerkBase perk)
    {
        perk.OnRemove();
        perkList.Remove(perk);
    }


    /// <summary>
    /// Retorna N perks aleatorios, excluindo perks ja selecionados a aparecer e os que já estão ativos
    /// </summary>
    /// <param name="ammount">Número de perks que ira retornar</param>
    public List<PerkSO> GetRandomPerks(int ammount) // Melhor deixar em 3 padrão né
    {
        var selected = new List<PerkSO>(); // Variavel responsavel para armazenar perks ja selecioandos a aparecer e previnir de o mesmo perk reaparecer
        var activePerks = new HashSet<string>(perkByName.Keys);

        // lista de candidatos que ainda não estão ativos
        var candidates = availablePerks
            .Where(p => !perkByName.ContainsKey(p.perkName))
            .ToList();

        for (int i = 0; i < ammount && candidates.Count > 0; i++)
        {
            int idx = UnityEngine.Random.Range(0, candidates.Count);
            selected.Add(candidates[idx]);
            Debug.Log(candidates[idx].perkName);
            candidates.RemoveAt(idx);
        }

       
        return selected;
    }

    void ShowPerkSelection()
    {
        var perks = GetRandomPerks(3);
        PerkOptionsManager.instance.SetupOptions(perks.ToArray());
    }

    void UpdatePerksStats()
    {
        for (int i = perkList.Count - 1; i >= 0; i--)
        {
            var perk = perkList[i];
            perk.UpdateWaveCount();

            if (perk.IsExpired)
                RemovePerk(perk);
        }
    }



}
