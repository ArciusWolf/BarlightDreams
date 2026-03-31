using UnityEngine;

public class TrashSystem : MonoBehaviour, IInteractable
{
    [SerializeField] private GlassInteraction glassInteraction;

    public void Interact()
    {
        if (PlayerUI.Instance != null)
        {
            PlayerManager.Instance.selectedIngredients.Clear();
            PlayerUI.Instance.ClearIcons();
            PlayerUI.Instance.SetHasGlass(false);
            PlayerUI.Instance.SetDrinkName("");
            glassInteraction.ClearGlass();
            DrinkPopupUI.Instance?.Hide();
            SoundManager.Instance.PlaySound(SoundManager.Instance.trashSound[0]);

            Debug.Log("[Trash] All ingredients and glass cleared.");
        }
        else
        {
            Debug.LogWarning("PlayerUI instance not found!");
        }
    }
}

