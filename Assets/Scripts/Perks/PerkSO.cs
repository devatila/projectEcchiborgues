using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Se eu conseguir fazer isso funcionar eu vou me auto declarar o rei do codigo...mau escrito;-;
public abstract class PerkSO : ScriptableObject
{
    public string perkName;
    public Sprite perkSprite;

    [Header("Localized Texts")]
    public List<LocalizationPerkText> localizedTexts = new List<LocalizationPerkText>();

    public abstract PerkBase CreatePerkInstance();
    public abstract Type GetPerkType();

    public bool hasValidateTime;

    [Tooltip("Numero de Ordas que este perk irá durar. Para funcionar, a opção 'hasValidateTime' tem que estar ativada")]
    public int wavesDuration = 0;

    #region LocalizationMethods
    public string GetLocalizedName(LanguageCode languageCode)
    {
        LocalizationPerkText text = localizedTexts.Find(t => t.language == languageCode);
        return text != null ? text.LocalizedName : "Error: Name Not Found";
    }

    public string GetLocalizedDescription(LanguageCode languageCode)
    {
        LocalizationPerkText text = localizedTexts.Find(t => t.language == languageCode);
        return text != null ? text.LocalizedDescription : "Error: Description Not Found";
    }
    #endregion
}

[System.Serializable]
public class LocalizationPerkText
{
    public LanguageCode language;
    public string LocalizedName;                            // Nome Visivel para o jogador 
    [TextArea] public string LocalizedDescription;          // Descrição visivel para o jogador
}

public enum LanguageCode
{
    Pt, // Portugues
    En, // Ingles
    Es  // Espanhol
    
    // Adicionar mais idiomas depois...
}