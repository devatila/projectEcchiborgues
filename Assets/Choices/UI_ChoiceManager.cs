using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_ChoiceManager : MonoBehaviour
{
    public GameObject[] choices;
    public static  UI_ChoiceManager instance;
    void Awake()
    {
        instance = this;
    }

    public void ShowMenoChoices(int indexMenu)
    {
        choices[indexMenu].SetActive(true);
    }

    public void HideMenuChoice(int indexMenu)
    {
        choices[indexMenu].SetActive(false);
    }
}
