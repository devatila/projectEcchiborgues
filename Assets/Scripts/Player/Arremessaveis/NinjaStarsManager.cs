using UnityEngine;

public class NinjaStarsManager : MonoBehaviour, IThrowableEffect
{
    public GameObject fragParent;
    public GameObject[] fragsObject;
    private void OnEnable()
    {
        
    }
    public void ApplyEffect(GameObject hitObject, int damage)
    {
        Debug.Log("S1");
        //fragParent.transform.parent = null;
        //fragParent.transform.position = transform.position;

        // Obt�m a dire��o para o mouse
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;

        float spreadAngle = 60f; // �ngulo total de dispers�o dos fragmentos
        int fragCount = fragsObject.Length;

        for (int i = 0; i < fragCount; i++)
        {
            fragsObject[i].SetActive(true);

            // Calcula o �ngulo de cada fragmento baseado na dire��o do mouse
            float angleOffset = spreadAngle * ((float)i / (fragCount - 1) - 0.5f);
            Vector2 fragDirection = Quaternion.Euler(0, 0, angleOffset) * direction;

            // Aplica a dire��o ao Rigidbody2D (se houver)
            Rigidbody2D rb = fragsObject[i].GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = fragDirection * 35f; // Ajuste a velocidade conforme necess�rio
                Debug.Log("S2." + i);
            }
        }
        Debug.Log("S3");


    }

    public void SetThrowableData(ThrowablesSO throwableData)
    {
        //throw new System.NotImplementedException();
    }
}
