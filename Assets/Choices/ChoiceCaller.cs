using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceCaller : MonoBehaviour
{
    public void CallerChoice(int index)
    {
        GameObject obj = GameObject.FindGameObjectWithTag("UI_Manager");
        obj.GetComponent<UI_ChoiceManager>().ShowMenoChoices(index);
    }
}
