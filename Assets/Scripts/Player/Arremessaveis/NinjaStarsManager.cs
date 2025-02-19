using System.Collections.Generic;
using UnityEngine;

public class NinjaStarsManager : MonoBehaviour, IThrowableEffect
{
    public int ninjaStarsAmmount = 3;
    public GameObject fragParent;
    public List<GameObject> fragsObject = new List<GameObject>();
    private NSBehavior[] nsBehaviors;
    [SerializeField] private bool canRicochete;
    private void OnEnable()
    {
        CheckIfHasLess();
    }

    void ResizeNSBehaviors()
    {
        nsBehaviors = new NSBehavior[fragsObject.Count];
        for (int i = 0; i < fragsObject.Count; i++)
        {
            nsBehaviors[i] = fragsObject[i].GetComponent<NSBehavior>();
            nsBehaviors[i].canBounce = canRicochete;
        }
    }
    private void Start()
    {
        SubscribingInEventsAndCheckingSomePerksEnabled();
        ResizeNSBehaviors();
    }

    private void SubscribingInEventsAndCheckingSomePerksEnabled()
    {
        var perks = PerkManager.Instance.throwablePerks.ninjaStar;
        perks.OnIncreaseAmmount += ApplyNewAmmount;
        perks.OnNinjaStarRicocheteAllow += ApplyRicocheteValue;

        if (perks.isIncreasePerkActivated)
        {
            ninjaStarsAmmount = perks.increasedAmmount;
            CheckIfHasLess();
        }
        Debug.Log(perks.isBounceActivated);
        if (perks.isBounceActivated)
        {
            Debug.Log("Chegou nesse If e o valor é" + perks.isBounceActivated);
            canRicochete = perks.isBounceActivated;
        }
    }

    void ApplyNewAmmount(int newTotalAmmount)
    {
        ninjaStarsAmmount = newTotalAmmount;
        
    }
    void ApplyRicocheteValue(bool CanRicochete)
    {
        canRicochete = CanRicochete;
        Debug.Log("Bounce foi setado para " + canRicochete);
    }
    void CheckIfHasLess()
    {
        if (fragsObject.Count < ninjaStarsAmmount)
        {
            int restantes = ninjaStarsAmmount - fragsObject.Count;
            for (int i = 0; i < restantes; i++)
            {
                GameObject newObj = Instantiate(fragsObject[0], transform.position, Quaternion.identity, transform);
                fragsObject.Add(newObj);
            }
            ResizeNSBehaviors();
        }
        else if (fragsObject.Count > ninjaStarsAmmount)
        {
            int excesso = fragsObject.Count - ninjaStarsAmmount;
            for (int i = 0; i < excesso; i++)
            {
                GameObject objToRemove = fragsObject[fragsObject.Count - 1]; // Pega o último da lista
                fragsObject.RemoveAt(fragsObject.Count - 1);
                Destroy(objToRemove);
            }
            ResizeNSBehaviors();
        }
        
    }
    public void ApplyEffect(GameObject hitObject, int damage)
    {

        // Obtém a direção para o mouse
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

        float spreadAngle = 60f; // Ângulo total de dispersão dos fragmentos
        int fragCount = fragsObject.Count;

        for (int i = 0; i < fragCount; i++)
        {
            fragsObject[i].SetActive(true);
            nsBehaviors[i].canBounce = canRicochete;
            // Calcula o ângulo de cada fragmento baseado na direção do mouse
            float angleOffset = spreadAngle * ((float)i / (fragCount - 1) - 0.5f);
            Vector2 fragDirection = Quaternion.Euler(0, 0, angleOffset) * direction;

            // Aplica a direção ao Rigidbody2D (se houver)
            Rigidbody2D rb = fragsObject[i].GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = fragDirection * 35f; // Ajuste a velocidade conforme necessário
                
            }
        }



    }

    public void SetThrowableData(ThrowablesSO throwableData)
    {
        //throw new System.NotImplementedException();
    }
}
