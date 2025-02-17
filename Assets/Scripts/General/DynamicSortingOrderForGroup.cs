using System.Collections.Generic;
using UnityEngine;

public class DynamicSortingOrderForGroup : MonoBehaviour
{
    private static readonly int SortingOrderBase = 1000; // Base para evitar números negativos.
    [SerializeField] private int offset = 0; // Ajuste manual para prioridade do grupo.

    private SpriteRenderer[] spriteRenderers;
    private List<int> basicOffset = new List<int>();

    private void Awake()
    {
        // Captura todos os SpriteRenderers nos filhos.
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers == null)
        {
            SpriteRenderer[] newArray = { GetComponent<SpriteRenderer>() };
            spriteRenderers = newArray;
        }
        foreach (var renderer in spriteRenderers)
        {
            basicOffset.Add(renderer.sortingOrder);
        }
    }

    private void LateUpdate()
    {
        // Calcula o sorting order com base na posição Y do GameObject pai.
        int sortingOrder = (int)(SortingOrderBase - transform.position.y * 10) + offset;

        // Aplica o sorting order a todos os filhos.
        foreach (var renderer in spriteRenderers)
        {
            int localOffset = basicOffset[System.Array.IndexOf(spriteRenderers, renderer)];
            //Debug.Log(localOffset + " " + renderer.name);
            renderer.sortingOrder = sortingOrder + localOffset;
        }
    }
}
