using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using TMPro;

public class CutsceneHandler : MonoBehaviour
{
    private bool alreadyActivated = false; //QUANDO A CUTSCENE ROLAR, NÃO É MAIS PARA CHAMAR A CUTSCENE UMA VEZ QUE JA TIVER OCORRIDO
    //E ESSA VARIAVEL DE CIMA VAI CONTROLAR ISSO //EU PODERIA ATÉ POR UM SETACTIVE MAS ACHO QUE PODE DAR RUIM...VOU FAZER AQUI DE UM JEITO E VOU VER NO QUE DÁ

    [SerializeField] public ActorInCutscene[] actors;
    public DialogueSettings[] dialogue;  //Por Mais de um SOMENTE se houver multiplos pontos para o NPC ir
    [SerializeField] private int indexDialogue;

    //Região de Controle de Camera durante a Cutscene
    public CinemachineVirtualCamera virtualCamera;
    private Transform originalFollowTarget;
    [SerializeField] private Transform initialCameraPosition;
    [SerializeField] private Transform initialCameraPositionInCutscene;
    [SerializeField] private Transform[] MultipleCameraPositionsInCutscenes;
    private int cameraIndex = 0;
    public float duration = 2f;
    private bool isTransitioning = false;


    //Região dos dialogos da Cutscene
    private List<string> sentences = new List<string>();
    private List<string> actorNames = new List<string>();
    private List<Sprite> profileSprite = new List<Sprite>();
    private UnityEvent[] postDialogueEvnt;

    void Start()
    {
        if (actors[0].startPoint == null) 
            actors[0].startPoint = actors[0].actor.transform;

        originalFollowTarget = virtualCamera.Follow;
    }

    public void StartCameraTransition(int positionCameraIndex)
    {
        Vector3 targetPosition = Vector3.zero;
        bool canTrasition = false;
        if (positionCameraIndex == 0)
        {
            if (initialCameraPositionInCutscene != null)
            {
                targetPosition = initialCameraPositionInCutscene.position;
                canTrasition = true;
            }
            cameraIndex++;
        }
        else
        {
            if(MultipleCameraPositionsInCutscenes != null)
            {
                targetPosition = MultipleCameraPositionsInCutscenes[cameraIndex - 1].position;
                canTrasition = true;
            }
        }
        if (!isTransitioning && canTrasition)
        {
            Debug.Log("Chegou nesse If");
            StartCoroutine(MoveCameraToPosition(targetPosition, false));
        }
    }
    IEnumerator MoveCameraToPosition (Vector3 targetPos, bool isTheEnd)
    {
        isTransitioning = true;
        float elapsedTime = 0f;
        virtualCamera.Follow = null;
        Vector3 startingPos = virtualCamera.transform.position;
        Vector3 desiredPos = new Vector3(targetPos.x,targetPos.y, Camera.main.transform.position.z);

        while(elapsedTime < duration)
        {
            virtualCamera.transform.position = Vector3.Lerp(startingPos, desiredPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        virtualCamera.transform.position = desiredPos;
        if (isTheEnd)
        {
            virtualCamera.Follow = originalFollowTarget;
        }
        isTransitioning = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (alreadyActivated) return;
            TypewriterEffectTMP.stopAll(); // vai pausar toda a movimentação do player. Tenho que me lembrar de mudar de onde chamar esse evento.
            StartCoroutine(MoveToPoint(actors[0].startPoint.position, FindObjectOfType<PlayerInventory>().transform.position, actors[0], 0.5f, true));
            alreadyActivated = true;
            StartCameraTransition(0);
        }
        
    }
    
    public void CallerMoveToPoint(int actorIndex)
    {
        TypewriterEffectTMP.stopAll();
        if (indexDialogue >= dialogue.Length)
        {
            Debug.Log("Termina");
            StartCoroutine(MoveToPoint(actors[actorIndex].actor.transform.position, actors[actorIndex].exitPoint.position, actors[actorIndex], 0.1f, false));
            StartCoroutine(MoveCameraToPosition(originalFollowTarget.position, true));
        }
        else
        {
            if (actors[actorIndex].multiplesPoints.Length != 0)
            {
                Debug.Log("Continua");
                StartCoroutine(MoveToPoint(actors[actorIndex].actor.transform.position, actors[actorIndex].multiplesPoints[indexDialogue - 1].position, actors[actorIndex], 0.1f, true));
            }
        }
        
    }
    public void CameraChangePosition()
    {

    }

    IEnumerator MoveToPoint(Vector3 initialPos, Vector3 targetPosition, ActorInCutscene actor, float distance, bool resume)
    {
        float npc_speed = actor.speed;
        // Enquanto o NPC não chegar ao ponto B
        while (Vector3.Distance(actor.actor.transform.position, targetPosition) > distance)
        {
            // Move o NPC na direção do ponto B
            actor.actor.transform.position = Vector3.MoveTowards(actor.actor.transform.position, targetPosition, npc_speed * Time.deltaTime);

            // Espera até o próximo frame antes de continuar
            yield return null;
        }

        // Quando o NPC chegar ao ponto B, chama a função OnReachedDestination
        if (resume)
            OnReachedDestination();
        else
            TypewriterEffectTMP.ContinueAll();

    }

    void OnReachedDestination()
    {
        Debug.Log("Chegou");
        //para fazer amanhã...ou mais tarde
        ClearText();
        GetText(indexDialogue);
        indexDialogue++;
        TypewriterEffectTMP.instance.Speech(sentences.ToArray(), actorNames.ToArray(), profileSprite.ToArray(), postDialogueEvnt);
        

        
    }
    void ClearText()
    {
        sentences.Clear();
        profileSprite.Clear();
        actorNames.Clear();
        postDialogueEvnt = null;
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
}

[System.Serializable]
public class ActorInCutscene
{
    public GameObject actor;             //NPC que provavelmente irá na direção do player ou não
    public float speed;                  //Velocidade
    public Transform startPoint;         //O ponto inicial de onde o npc irá sair
    public Transform endPoint;           //O ponto final de onde o NPC ira parar (Geralmente, proximo ao player)
    public Transform[] multiplesPoints;  //Caso queira que o NPC se mova apos algum dialogo, preencher esta variavel
    public Transform exitPoint;          //O ponto para onde o NPC irá após a conversa acabar
}
