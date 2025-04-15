using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPerkManager : MonoBehaviour
{
    public List<PerkBase> perkList = new List<PerkBase>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ApplyPerk(new PlayerSpeedBoostPerk(FindObjectOfType<Player_Movement>(), 3f));
        }
    }

    void ApplyPerk(PerkBase perk)
    {
        perk.OnApply();
        perkList.Add(perk);
    }
}
