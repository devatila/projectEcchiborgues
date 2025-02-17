using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;
using UnityEngine.Events;

public class TypewriterEffectTMP : MonoBehaviour
{
    public delegate void StopAllThings();
    public static StopAllThings stopAll;

    public delegate void ConinueAllThings();
    public static ConinueAllThings ContinueAll;



    [System.Serializable]
    public enum idiom
    {
        pt,
        eng,
        spa
    }
    public idiom language;

    [Header("Componetns")]
    public GameObject DialogueObj; //Janela do diálogo
    public Image profileSprite; //sprite do perfil (Se necessário)
    //Não necessário por enquanto// public Text speechText; //texto da fala
    public TMP_Text actorNameText; //nome do personagem que está falando


    public string text;
    public TMP_Text displayText; // TextMeshPro Text component
    public float defaultTypeSpeed = 0.25f;
    public float typeSpeed;

    // Variáveis para o efeito de tremulação
    private List<ShakeTag> shakingTags = new List<ShakeTag>();

    private bool isShowing; //se a janela de fala está visível
    private int index; //indexador dos textos
    private string[] sentences;
    private string[] actualActorName;
    private Sprite[] actualProfileSprite;

    public static TypewriterEffectTMP instance;

    private string hiddenFullText, commandspattern;

    private UnityEvent[] dialogueEvent;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        // Exemplo de string com comandos
        //text = "<color=#FF0000>normal</color> <ts=0,1>combinando velocidade </ts> com <sh>Texto tremendo.</sh> hehe Texto normal. <ts=0,02>Texto rápido.</ts> Texto normal. <sh>Texto tremendo.</sh> agora";
        //StartCoroutine(PlayText(sentences[index]));
        hiddenFullText = "";
    }

    private IEnumerator PlayText(string text)
    {
        typeSpeed = defaultTypeSpeed;
        displayText.text = "";
        commandspattern = @"<sh>|</sh>|<ts=[^>]*>|</ts>|<func=[^>]+>|<pause=[^>]+>";
        hiddenFullText = Regex.Replace(text, commandspattern, ""); //RemoveCustomTags(text);
        Debug.Log(hiddenFullText);
        shakingTags.Clear(); // Limpa a lista de tags a cada novo texto

        int shakingTagStartIndex = -1;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '<')
            {
                int commandLength = HandleTagCommand(text.Substring(i), ref shakingTagStartIndex);
                i += commandLength - 1; // Move o índice conforme o tamanho da tag
            }
            else
            {
                displayText.text += text[i];
                

                yield return new WaitForSeconds(typeSpeed);
            }
        }

        
    }
    public void NextSentence()
    {
        Debug.Log("Chegou aqui");
        if (displayText.text == hiddenFullText)
        {
            
            if(index < sentences.Length - 1)
            {
                index++;
                profileSprite.sprite = actualProfileSprite[index];
                actorNameText.text = actualActorName[index];
                displayText.text = "";
                StartCoroutine(PlayText(sentences[index]));
            }
            else
            {
                displayText.text = "";
                index = 0;
                actorNameText.text = "";
                DialogueObj.SetActive(false);
                sentences = null;
                isShowing = false;
                ContinueAll();
                if(dialogueEvent != null)
                {
                    for(int i = 0; i < dialogueEvent.Length; i++)
                    {
                        dialogueEvent[i].Invoke();
                    }
                    
                }
            }
        }
    }
    public void Speech(string[] txt, string[] actorName, Sprite[] actorSprite, UnityEvent[] dialoguePostEvent)
    {
        if (!isShowing) 
        {
            stopAll();
            DialogueObj.SetActive(true);
            sentences = txt;
            actualActorName = actorName;
            actualProfileSprite = actorSprite;
            profileSprite.sprite = actualProfileSprite[index];
            actorNameText.text = actualActorName[index];
            StartCoroutine(PlayText(sentences[index]));
            dialogueEvent = dialoguePostEvent;
            isShowing = true;
        }
    }


    private int HandleTagCommand(string text, ref int shakingTagStartIndex)
    {
        int commandLength = 0;

        if (text.StartsWith("<sh>"))
        {
            shakingTagStartIndex = displayText.text.Length;
            commandLength = 4;
        }
        else if (text.StartsWith("</sh>"))
        {
            int shakingTagEndIndex = displayText.text.Length - 1;
            shakingTags.Add(new ShakeTag(shakingTagStartIndex, shakingTagEndIndex));
            commandLength = 5;
        }
        else if (text.StartsWith("<ts="))
        {
            commandLength = HandleTypeSpeedCommand(text);
        }
        else if (text.StartsWith("</ts>"))
        {
            typeSpeed = defaultTypeSpeed;
            commandLength = 5;
        }
        else if (text.StartsWith("<func="))
        {
            // Chama o método para lidar com a execução da função
            int funcLength = HandleFunctionCommand(text);
            commandLength = funcLength;
            //i += funcLength - 1; // Move o índice conforme o tamanho do comando
        }
        else if (text.StartsWith("<pause="))
        {
            commandLength = HandlePauseCommand(text);
        }

        // Adiciona uma verificação para tags padrão do TMP
        else if (text.StartsWith("<color=") || text.StartsWith("</color>") || text.StartsWith("<b>") || text.StartsWith("</b>"))
        {
            int endOfTag = text.IndexOf('>') + 1;
            displayText.text += text.Substring(0, endOfTag);
            commandLength = endOfTag;
            
        }

        return commandLength;
    }

    private int HandleTypeSpeedCommand(string text)
    {
        int j = 4;
        while (j < text.Length && (char.IsDigit(text[j]) || text[j] == '.' || text[j] == ','))
        {
            j++;
        }

        string speedValue = text.Substring(4, j - 4);
        if (float.TryParse(speedValue, out float speed))
        {
            typeSpeed = speed;
        }

        return j + 1; // Inclui o '>'
    }

    private void Update()
    {
        // Agora, o mesh e o efeito de tremulação são atualizados no PlayText
        displayText.ForceMeshUpdate(); // Atualiza o mesh sempre que uma nova letra é adicionada
        if(isShowing)
            ApplyShakeEffect();
    }

    private void ApplyShakeEffect()
    {
        TMP_TextInfo textInfo = displayText.textInfo;
        Vector3[] vertices = displayText.mesh.vertices;

        // Adaptação para calcular o deslocamento de tags
        int tagOffset = CalculateTagOffset();

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textInfo.characterInfo[i];

            // Verifica se o caractere está visível
            if (!c.isVisible)
                continue;

            // Verifica se o caractere está dentro da tag <sh>
            if (IsCharacterWithinShakingTag(i - tagOffset))
            {
                int index = c.vertexIndex;
                Vector3 offset = Wobble(Time.time + i);

                vertices[index] += offset;
                vertices[index + 1] += offset;
                vertices[index + 2] += offset;
                vertices[index + 3] += offset;
            }
        }

        // Atualiza os vértices do mesh
        displayText.mesh.vertices = vertices;
        displayText.canvasRenderer.SetMesh(displayText.mesh);
    }


    private int CalculateTagOffset()
    {
        int offset = 0;
        string text = displayText.text;

        // Conta o deslocamento total devido às tags
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '<')
            {
                int tagEnd = text.IndexOf('>', i);
                if (tagEnd != -1)
                {
                    // Subtrai o comprimento da tag ao offset
                    offset -= (tagEnd - i + 1);
                    i = tagEnd;
                }
            }
        }

        return offset;
    }


    private bool IsCharacterWithinShakingTag(int charIndex)
    {
        foreach (var tag in shakingTags)
        {
            if (charIndex >= tag.startIndex && charIndex <= tag.endIndex)
            {
                return true;
            }
        }

        return false;
    }


    private Vector3 Wobble(float time)
    {
        return new Vector3(Mathf.Sin(time * 9.9f), Mathf.Cos(time * 7.5f), 0) * 1f;
    }

    // Classe auxiliar para armazenar os índices das tags de tremulação
    private class ShakeTag
    {
        public int startIndex;
        public int endIndex;

        public ShakeTag(int startIndex, int endIndex)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }
    }

    private string RemoveCustomTags(string text)
    {
        string pattern = @"<[^>]+>";
        return System.Text.RegularExpressions.Regex.Replace(text, pattern, string.Empty);
    }

    private int HandleFunctionCommand(string text)
    {
        // Encontra o início e o final da tag <func=nomeFuncao>
        int equalsIndex = text.IndexOf('=');
        int endOfTagIndex = text.IndexOf('>');

        if (equalsIndex != -1 && endOfTagIndex != -1)
        {
            // Extrai o nome da função entre o '=' e o '>'
            string functionName = text.Substring(equalsIndex + 1, endOfTagIndex - equalsIndex - 1);

            // Tenta chamar o método com o nome extraído
            //Debug.Log(functionName);
            InvokeFunction(functionName);

            // Retorna o comprimento total da tag para ser ignorada no texto mostrado
            return endOfTagIndex + 1; // Inclui o '>'
        }

        return 0; // Se não encontrar a tag ou algo estiver errado, não incrementa o índice
    }

    private void InvokeFunction(string functionName)
    {
        // Verifica se o método existe e o chama
        var method = this.GetType().GetMethod(functionName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (method != null)
        {
            method.Invoke(this, null);
        }
        else
        {
            Debug.LogWarning($"A função '{functionName}' não foi encontrada.");
        }
    }

    private int HandlePauseCommand(string text)
    {
        int j = 7; // Pula "<pause="
        while (j < text.Length && char.IsDigit(text[j]))
        {
            j++;
        }

        string pauseValue = text.Substring(7, j - 7);
        if (float.TryParse(pauseValue, out float pauseTime))
        {
            StartCoroutine(PauseText(pauseTime));
        }

        return j + 1; // Inclui o '>'
    }

    private IEnumerator PauseText(float pauseTime)
    {
        typeSpeed = pauseTime;
        yield return new WaitForSeconds(pauseTime);
        typeSpeed = defaultTypeSpeed;
    }


    void Teste()
    {
        Debug.Log("O Texto me Chamoou");
    }
}