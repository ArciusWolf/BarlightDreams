using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerInput inputActions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInput();
        inputActions.Enable();

        inputActions.Player.Interact.performed += ctx => TryInteract();
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }

    public void TryInteract()
    {
        if (RibbonUI.IsBannerAnimating)
        {
            Debug.Log("Interaction blocked: Banner is animating.");
            return;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Interactable"))
            {
                IInteractable interactable = hitCollider.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    interactable.Interact();
                    Debug.Log("Interacted with: " + hitCollider.name);
                    break;
                }
                else
                {
                    Debug.LogWarning("No IInteractable component found on: " + hitCollider.name);
                }
            }
        }
    }
}
