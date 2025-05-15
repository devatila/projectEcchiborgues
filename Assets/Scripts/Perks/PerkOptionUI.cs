using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PerkOptionUI : MonoBehaviour
{
    [Header("UI References")]
    public Image background;
    public Image icon;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Button selectButton;

    private PerkSO perkData;

    /// <summary>
    /// Inicializa este slot com os dados do PerkSO e configura o botão.
    /// </summary>
    public void Setup(PerkSO data, LanguageCode lang)
    {
        perkData = data;
        icon.sprite = data.perkSprite;
        titleText.text = data.GetLocalizedName(lang);
        descriptionText.text = data.GetLocalizedDescription(lang);

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // Aplica o perk e fecha a UI de seleção
        NewPerkManager.Instance.ApplyPerk(perkData);
        PerkOptionsManager.instance.HideAllOptions(); // esconde o painel inteiro
    }
}
