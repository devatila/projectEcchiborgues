using System;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public event Action OnPlaced;

    public bool startPlaced;

    private bool isPlaced = false;
    public bool _isPlaced { get { return isPlaced; }
        set {
            isPlaced = value;
        } 
    }

    private bool canBePlaced = true;
    public bool _canBePlaced { get { return canBePlaced; } set { canBePlaced = value; } }

    private SpriteRenderer spriteRenderer;
    private Color nonPlacedColor;

    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        if (startPlaced) isPlaced = true;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        nonPlacedColor = spriteRenderer.color;
        if (!isPlaced)
        {
            nonPlacedColor.a = 0.5f;
            spriteRenderer.color = nonPlacedColor;
        }
    }
    
    public void SetColorOnPlaced()
    {
        nonPlacedColor.a = 1f;
        spriteRenderer.color = nonPlacedColor;
        isPlaced = true;
        OnPlaced?.Invoke();
        if (rb != null) Destroy(rb);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPlaced) return;
        canBePlaced = false;
        nonPlacedColor = Color.red;
        nonPlacedColor.a = 0.5f;
        spriteRenderer.color = nonPlacedColor;
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(isPlaced) return;
        canBePlaced = true;
        nonPlacedColor = Color.white;
        nonPlacedColor.a = 0.5f;
        spriteRenderer.color = nonPlacedColor;
        
    }
}
