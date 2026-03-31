using UnityEngine;

public class GlassInteraction : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (PlayerUI.Instance.hasGlass)
        {
            NoticeUI.Instance.ShowNotice("You can only hold one glass at a time!");
        }
        else
        {
            ShowGlass();
        }
    }

    private void ShowGlass()
    {
        if (PlayerUI.Instance != null)
        {
            PlayerUI.Instance.SetHasGlass(true);
            PlayerUI.Instance.SetDrinkName("Empty Cup");

            DrinkPopupUI popup = Object.FindFirstObjectByType<DrinkPopupUI>();
            popup?.Show("Empty Cup");

            if (SoundManager.Instance.glassPickUpSound.Length > 0)
            {
                int randomIndex = Random.Range(0, SoundManager.Instance.glassPickUpSound.Length);
                SoundManager.Instance.PlaySoundNormalized(SoundManager.Instance.glassPickUpSound[randomIndex]);
            }
            Debug.Log("[Glass] Glass picked up.");
        }
    }

    public void ClearGlass()
    {
        if (!PlayerUI.Instance.hasGlass)
        {
            Debug.LogWarning("[Glass] Tried to clear glass, but player has none.");
            return;
        }

        PlayerUI.Instance.SetHasGlass(false);
        PlayerUI.Instance.SetDrinkName("");
        Debug.Log("[Glass] Glass cleared.");
    }
}
