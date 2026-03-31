using UnityEngine;

public class MiniBarInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject upgradeUI;

    public void Interact()
    {
        if (upgradeUI == null)
        {
            Debug.Log("Can't get Upgrade Menu");
        }
        else
        {
            UIAnimator.Show(upgradeUI, 0.4f);
            StartCoroutine(DelayedRefresh());
        }
    }

    public void CloseUI()
    {
        if (upgradeUI != null)
        {
            UIAnimator.Hide(upgradeUI, 0.4f);
        }
    }

    private System.Collections.IEnumerator DelayedRefresh()
    {
        yield return null;
        foreach (var card in upgradeUI.GetComponentsInChildren<UpgradeCardUI>())
        {
            card.Refresh();
        }
    }
}
