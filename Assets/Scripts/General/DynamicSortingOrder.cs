using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private static readonly int SortingOrderBase = 1000; // Base para evitar números negativos.
    [SerializeField] private int offset = 0; // Offset para ajustes manuais (opcional).

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        // Ordena com base no eixo Y (quanto menor o Y, mais à frente o sprite aparece).
        spriteRenderer.sortingOrder = (int)(SortingOrderBase - transform.position.y * 10) + offset;
    }
}
