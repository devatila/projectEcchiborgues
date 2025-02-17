using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPC_Dialogue : MonoBehaviour
{
    public int dialogueIndex;
    public DialogueSettings[] dialogue;
    private List<string> sentences = new List<string>();
    private List<string> actorNames = new List<string>();
    private List<Sprite> profileSprite = new List<Sprite>();
    private UnityEvent[] postDialogueEvnt;

    public bool initiateOnStart;
    public float dialogueRange;
    public LayerMask playerLayer;
    private bool isOnRange;

    void Start()
    {
        GetText(dialogueIndex);
    }

    private void FixedUpdate()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, dialogueRange, playerLayer);
        if (hit != null)
        {
            isOnRange = true;
        }
        else
        {
            isOnRange = false;
        }
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F) && isOnRange)
        {
            TypewriterEffectTMP.instance.Speech(sentences.ToArray(), actorNames.ToArray(), profileSprite.ToArray(), postDialogueEvnt);
            if (dialogueIndex < dialogue.Length - 1)
            {
                dialogueIndex++;
                ClearText();
                GetText(dialogueIndex);
            }
            
            
        }
    }
    void GetText(int index)
    {
        for (int i = 0; i < dialogue[index].dialogues.Count; i++)
        {
            switch (TypewriterEffectTMP.instance.language)
            {
                case TypewriterEffectTMP.idiom.pt:
                    sentences.Add(dialogue[index].dialogues[i].sentences.portuguese);
                    break;
                case TypewriterEffectTMP.idiom.eng:
                    sentences.Add(dialogue[index].dialogues[i].sentences.english);
                    break;
                case TypewriterEffectTMP.idiom.spa:
                    sentences.Add(dialogue[index].dialogues[i].sentences.spanish);
                    break;
            }
            actorNames.Add(dialogue[index].dialogues[i].actorName);
            profileSprite.Add(dialogue[index].dialogues[i].profile);
            
        }
        postDialogueEvnt = dialogue[index].postDialogueEvent;
    }
    void ClearText()
    {
        sentences.Clear();
        profileSprite.Clear();
        actorNames.Clear();
        postDialogueEvnt = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, dialogueRange);
    }
}
