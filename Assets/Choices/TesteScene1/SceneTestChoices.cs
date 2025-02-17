using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SceneTestChoices : MonoBehaviour
{
    [Header("NPC_01")]

    public bool Escolheu1;
    public bool Escolheu2;
    public DialogueSettings choiceBogas, choiceTomarBogas;

    [Header("NPC_02")]

    public DialogueSettings choice1;
    public DialogueSettings choice2;



    private List<string> sentences = new List<string>();
    private List<string> actorNames = new List<string>();
    private List<Sprite> profileSprite = new List<Sprite>();
    private UnityEvent[] postDialogueEvnt;


    public void TomarNoBogas(int index)
    {
        Debug.Log("Escolheu Tomar no Bógas");
        UI_ChoiceManager.instance.HideMenuChoice(index);
        GetText(choiceTomarBogas);
        TypewriterEffectTMP.instance.Speech(sentences.ToArray(), actorNames.ToArray(), profileSprite.ToArray(), postDialogueEvnt);
    }
    public void EsbagacadorDeBogas(int index)
    {
        Debug.Log("HHMM Você escolheu o esbaçador de bógas");
        UI_ChoiceManager.instance.HideMenuChoice(index);
        GetText(choiceBogas);
        TypewriterEffectTMP.instance.Speech(sentences.ToArray(), actorNames.ToArray(), profileSprite.ToArray(), postDialogueEvnt.ToArray());
    }

    public void Escolha1(int index)
    {
        Debug.Log("Escolheu o Primeiro!");
        UI_ChoiceManager.instance.HideMenuChoice(index);
    }
    public void Escolha2(int index)
    {
        Debug.Log("Escolheu o Segundo!!");
        UI_ChoiceManager.instance.HideMenuChoice(index);
    }


    void GetText(DialogueSettings ds)
    {
        sentences.Clear();
        profileSprite.Clear();
        actorNames.Clear();
        postDialogueEvnt = null;

        for (int i = 0; i < ds.dialogues.Count; i++)
        {
            switch (TypewriterEffectTMP.instance.language)
            {
                case TypewriterEffectTMP.idiom.pt:
                    sentences.Add(ds.dialogues[i].sentences.portuguese);
                    break;
                case TypewriterEffectTMP.idiom.eng:
                    sentences.Add(ds.dialogues[i].sentences.english);
                    break;
                case TypewriterEffectTMP.idiom.spa:
                    sentences.Add(ds.dialogues[i].sentences.spanish);
                    break;
            }
            actorNames.Add(ds.dialogues[i].actorName);
            profileSprite.Add(ds.dialogues[i].profile);
            
        }
        postDialogueEvnt = ds.postDialogueEvent;
    }

}