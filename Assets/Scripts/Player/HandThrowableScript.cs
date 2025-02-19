using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Eu escrevo ruim demais mas é pra respresentar todos os tipos de arremessáveis presente no jogo
public enum ThroableObjects {
    Granade,
    ImpactGranade,
    FragGranade,
    PEM_Granade,
    Dynamite,
    Molotov,
    ThrowingKnife,
    NinjaStar
}
public class HandThrowableScript : MonoBehaviour
{
    private AnimPlayer animPlayer;

    [SerializeField] private Transform spawnPosition;
    [SerializeField] private ThrowableOrgnizer throwablesInventory;
    [SerializeField] private ThroableObjects thorwableEquipped;
    [SerializeField] private int equippedIndex;

    private Dictionary<ThroableObjects, System.Func<int>> GetAmmount;
    private Dictionary<ThroableObjects, System.Action<int>> SetAmmount;

    private Vector3 mousePosition;
    private Camera cam;
    private ThroableObjects[] throwableTypes;

    public int NinjaStarsAmmount = 10;

    private void Start()
    {
        cam = Camera.main; //Camera principal
        animPlayer = GetComponent<AnimPlayer>();

        GetAmmount = new Dictionary<ThroableObjects, System.Func<int>>
        {
            { ThroableObjects.Granade,          () => throwablesInventory.granadeAmmount },
            { ThroableObjects.ImpactGranade,    () => throwablesInventory.impactGranadeAmmount },
            { ThroableObjects.FragGranade,      () => throwablesInventory.fragGranadeAmmount },
            { ThroableObjects.PEM_Granade,      () => throwablesInventory.PEMammount },
            { ThroableObjects.Dynamite,         () => throwablesInventory.dynamiteAmmount },
            { ThroableObjects.Molotov,          () => throwablesInventory.molotovAmmount },
            { ThroableObjects.ThrowingKnife,    () => throwablesInventory.throwingKnifeAmmount },
            { ThroableObjects.NinjaStar,        () => throwablesInventory.ninjaStarAmmount }
        };

        // Responsável para setar os valores referenciaveis diretamente nas variaveis publicas
        SetAmmount = new Dictionary<ThroableObjects, System.Action<int>>
        {
            { ThroableObjects.Granade,          (value) => throwablesInventory.granadeAmmount       = value },
            { ThroableObjects.ImpactGranade,    (value) => throwablesInventory.impactGranadeAmmount = value },
            { ThroableObjects.FragGranade,      (value) => throwablesInventory.fragGranadeAmmount   = value },
            { ThroableObjects.PEM_Granade,      (value) => throwablesInventory.PEMammount           = value },
            { ThroableObjects.Dynamite,         (value) => throwablesInventory.dynamiteAmmount      = value },
            { ThroableObjects.Molotov,          (value) => throwablesInventory.molotovAmmount       = value },
            { ThroableObjects.ThrowingKnife,    (value) => throwablesInventory.throwingKnifeAmmount = value },
            { ThroableObjects.NinjaStar,        (value) => throwablesInventory.ninjaStarAmmount     = value }
        };

        // Obtém todos os valores do enum automaticamente
        throwableTypes = (ThroableObjects[])Enum.GetValues(typeof(ThroableObjects));

        equippedIndex = GetNextAvailableIndex(0);

        // Garante que o primeiro arremessável seja configurado corretamente
        UpdateThrowableEquipped();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Executa a logica de arremesso quando eu pressiono a tecla G (Me lembrar disso quando eu adaptar para o input system)
            Throw();
        }
        // Função que lê o scroll do mouse e atualiza o arremessavel equipado
        ScrollerHandler();
    }
    private void ScrollerHandler()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            equippedIndex = GetNextAvailableIndex(1);
        }
        else if (scroll < 0f)
        {
            equippedIndex = GetNextAvailableIndex(-1);
        }

        // Atualiza o tipo de arremessável equipado
        UpdateThrowableEquipped();
    }
    private void UpdateThrowableEquipped()
    {
        // Define o enum baseado no índice atual
        thorwableEquipped = throwableTypes[equippedIndex];
    }
    private int GetNextAvailableIndex(int direction)
    {
        int newIndex = equippedIndex;

        for (int i = 0; i < throwableTypes.Length; i++)
        {
            newIndex = (newIndex + direction + throwableTypes.Length) % throwableTypes.Length;

            if (GetAmmount[throwableTypes[newIndex]]() > 0) // Obtém a quantidade atual dinamicamente
            {
                return newIndex;
            }
        }

        return equippedIndex; // Retorna o mesmo índice se nenhum estiver disponível
    }

    private void Throw()
    {
        if (GetAmmount[thorwableEquipped]() <= 0)
        {
            Debug.LogWarning("Sem Arremessável disponivel ou Arremessável Equipado inválido");
            return;
        }

        // Puxa da pool de arremessaveis o objeto equipado
        GameObject granadeObj = HandThrowablePoolSystem.instance.GetObject(thorwableEquipped);
        IThrowable throwable = granadeObj.GetComponent<IThrowable>();
        
        // Execução das devidas funções da interface de arremessavel
        if (throwable != null)
        {
            throwable.ThrowObject(GetMousePosition(), spawnPosition.position);
            
        }

        SetAmmount[thorwableEquipped](GetAmmount[thorwableEquipped]() - 1);

        // Executa a animação de arremessar (Meio porco eu sei, dps eu penso se melhoro)
        animPlayer.PlayThrowableAnimation();

        
    }

    private void FormerWay()
    {
        //granadeAmmount--;
        animPlayer.PlayThrowableAnimation();

        mousePosition = GetMousePosition();

        //GameObject granadeObj = HandThrowablePoolSystem.instance.GetObject(); //Instantiate(granadePrefab, spawnPosition.position, Quaternion.identity);
        //granadeObj.transform.position = spawnPosition.position;

        //Vector3 dir = spawnPosition.position - mousePosition;
        //dir.Normalize();
        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        //granadeObj.transform.rotation = Quaternion.Euler(0, 0, angle);
        //granadeObj.GetComponent<GranadeObject>().SetTargetObject(mousePosition);
    }

    public Vector3 GetMousePosition()
    {
        Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        return pos;
    }
}

[System.Serializable]
public class ThrowableOrgnizer 
{
    public int granadeAmmount;
    public int impactGranadeAmmount;
    public int fragGranadeAmmount;
    public int PEMammount;
    public int dynamiteAmmount;
    public int molotovAmmount;
    public int throwingKnifeAmmount;
    public int ninjaStarAmmount;
}
