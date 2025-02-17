using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_PlayerCash : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Text;

    private PlayerInventory m_Inventory;
    // Start is called before the first frame update

    private void Awake()
    {
        m_Inventory = FindObjectOfType<PlayerInventory>();
        m_Inventory.OnChangeCash += GetPlayerCash;
    }
    void Start()
    {
        m_Text.text = "<sprite=\"EmojiOne\" index=0> 0";

        UpdateNumberInText(m_Inventory.metalCash.ToString());
    }
    public void UpdateNumberInText(string newNumber)
    {
        // Verificar se o texto cont�m um sprite
        string currentText = m_Text.text;
        int spriteEndIndex = currentText.IndexOf(">") + 1; // �ndice logo ap�s o fim da tag <sprite>

        if (spriteEndIndex > 0)
        {
            // Substituir o n�mero ap�s a tag
            string updatedText = currentText.Substring(0, spriteEndIndex) + " " + newNumber;
            m_Text.text = updatedText;
        }
        else
        {
            Debug.LogWarning("O texto n�o cont�m uma tag <sprite> v�lida.");
        }
    }

    void GetPlayerCash(int cash)
    {
        UpdateNumberInText(cash.ToString());
    }

}
