using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class InteractableOutline : MonoBehaviour
{
    public Sprite outlinedSprite;
    private SpriteRenderer spriteRenderer;
    private Sprite normalSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalSprite = spriteRenderer.sprite; // Save the original sprite
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.sprite = outlinedSprite; // Enable outline
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.sprite = normalSprite; // Disable outline
        }
    }
}