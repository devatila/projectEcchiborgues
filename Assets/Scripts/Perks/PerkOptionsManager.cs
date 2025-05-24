using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkOptionsManager : MonoBehaviour
{
    public static PerkOptionsManager instance;
    [SerializeField] private Image BackGround;

    public PerkOptionUI[] perkOptionUIs;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
        perkOptionUIs = GetComponentsInChildren<PerkOptionUI>();
        HideAllOptions();
    }

    public void SetupOptions(PerkSO[] perks)
    {
        BackGround.enabled = true;
        for (int i = 0; i < perks.Length; i++)
        {
            perkOptionUIs[i].gameObject.SetActive(true);
            perkOptionUIs[i].Setup(perks[i], LanguageCode.Pt);
        }
    }

    public void HideAllOptions()
    {
        BackGround.enabled = false;
        for (int i = 0; i < perkOptionUIs.Length; i++)
        {
            perkOptionUIs[i].gameObject.SetActive(false);
        }
    }
}
