using UnityEngine;

public class IceContainer : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientSO iceIngredient;

    public void Interact()
    {
        if (iceIngredient == null)
        {
            Debug.LogError("Ice ingredient is not assigned.");
            return;
        }

        if (PlayerManager.Instance.selectedIngredients.Count >= 4)
        {
            NoticeUI.Instance.ShowNotice("You can only hold 4 ingredients!");
            return;
        }

        PlayerManager.Instance.selectedIngredients.Add(iceIngredient);
        PlayerUI.Instance.UpdateIngredientIcons(PlayerManager.Instance.selectedIngredients);

        Debug.Log("Ice added to drink.");
        SoundManager.Instance.PlaySound(SoundManager.Instance.iceSound[0]);
    }

    
}
