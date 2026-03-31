using System.Collections;
using UnityEngine;

public class ServeSystem : MonoBehaviour, IInteractable
{
    [Header("Component References")]
    public GlassInteraction glass;

    private bool isServing = false;
    private bool justOrdered = false;

    public void Start()
    {
        if (glass == null)
        {
            glass = FindAnyObjectByType<GlassInteraction>();
            if (glass == null)
            {
                Debug.LogError("GlassInteraction component not found on ServeSystem.");
            }
        }
    }

    public void Interact()
    {
        if (isServing || justOrdered) return;

        CustomerOrder customer = DetectCustomer();
        if (customer == null) return;

        if (!customer.active)
        {
            if (customer.questionMark != null && customer.questionMark.activeSelf)
            {
                customer.InitializeOrder();
                Debug.Log("Customer has now ordered.");

                justOrdered = true;
                StartCoroutine(ResetJustOrdered());
            }
            return;
        }
        if (PlayerManager.Instance.currentMix == null && PlayerManager.Instance.mysteryDrinkReference == null)
        {
            NoticeUI.Instance.ShowNotice("No drink to serve...");
            Debug.Log("No drink to serve.");
            return;
        }

        if (!PlayerUI.Instance.hasGlass)
        {
            Debug.LogWarning("Tried to serve without holding a glass.");
            NoticeUI.Instance.ShowNotice("You need a glass to serve!");
            return;
        }

        isServing = true;
        ServeDrink(customer);
        StartCoroutine(ResetServeCooldown());
    }

    private IEnumerator ResetJustOrdered()
    {
        yield return new WaitForSeconds(0.2f);
        justOrdered = false;
    }


    private IEnumerator ResetServeCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        isServing = false;
    }

    private CustomerOrder DetectCustomer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 0f);
        if (hit.collider != null)
        {
            return hit.collider.GetComponent<CustomerOrder>();
        }
        return null;
    }

    public void ServeDrink(CustomerOrder order)
    {
        var mix = PlayerManager.Instance.currentMix;

        if (mix == null && PlayerManager.Instance.mysteryDrinkReference == null)
        {
            Debug.Log("No drink prepared.");
            return;
        }

        if (!order.active)
        {
            Debug.Log("Tried to serve inactive customer.");
            return;
        }

        if (mix != null)
        {
            order.Serve(mix);
        }
        else
        {
            order.ServeMysteryDrink();
        }

        PlayerManager.Instance.currentMix = null;
        PlayerManager.Instance.mysteryDrinkReference = null;
        PlayerUI.Instance.UpdateIngredientIcons(PlayerManager.Instance.selectedIngredients);

        PlayerUI.Instance.SetDrinkName("");
        DrinkPopupUI.Instance?.Hide();
        Debug.Log("[Serve] Attempting to clear glass...");
        glass.ClearGlass();
        Debug.Log("[Serve] Glass clear requested.");

        Debug.Log("[Serve] Drink served successfully. Glass cleared.");
    }
}
