using UnityEngine;

public class MixingZone : MonoBehaviour
{
    [SerializeField] private bool playerInside = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            PlayerUI.Instance.ShowMixButton(false);
        }
    }

    private void Update()
    {
        // continuously check only if player is inside
        if (playerInside)
        {
            CheckAndShowMixButton();
        }
    }

    private void CheckAndShowMixButton()
    {
        var player = PlayerManager.Instance;
        if (player == null) return;

        bool hasGlass = PlayerUI.Instance.hasGlass;
        bool enoughIngredients = player.selectedIngredients.Count >= 2;

        if (playerInside && hasGlass && enoughIngredients)
        {
            PlayerUI.Instance.ShowMixButton(true);
            Debug.Log("Conditions true!");
        }
        else
        {
            PlayerUI.Instance.ShowMixButton(false);
            Debug.Log("Conditions false!");
        }
    }
}
