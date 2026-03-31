using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixingSystem_Backup : MonoBehaviour, IInteractable
{
    [SerializeField] private DrinkRecipeSO[] drinkRecipes;
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private SpriteRenderer loadingBarRenderer;
    [SerializeField] private Sprite[] loadingBarStages;
    [SerializeField] private GameObject loadingBarContainer;
    [SerializeField] private MysteryDrinkSO mysteryDrinkData;

    public float mixDuration = 3f;

    private void Start()
    {
        mixDuration = PlayerUpgradeManager.Instance.cachedMixDuration;
        Debug.Log($"[MIX] Mix duration set to {mixDuration} from upgrade");
    }

    public void Interact()
    {
        // Player must have a glass to mix ingredients
        if (!playerUI.hasGlass)
        {
            NoticeUI.Instance.ShowNotice("You need to get a glass before mixing!");
            return;
        }

        // Player must select ingredients before mixing too
        var selectedIngredients = PlayerManager.Instance.selectedIngredients;
        if (selectedIngredients.Count >= 2)
        {
            MixIngredients(selectedIngredients);
        }
        else
        {
            NoticeUI.Instance.ShowNotice("You must select more than 2 ingredients!");
        }
    }

    public void MixIngredients(List<IngredientSO> selectedIngredients)
    {
        StartCoroutine(MixWithDelay(selectedIngredients));
    }

    private IEnumerator MixWithDelay(List<IngredientSO> selectedIngredients)
    {
        float actualMixTime = mixDuration;
        float timePassed = 0f;
        int stageCount = loadingBarStages.Length;

        loadingBarContainer.SetActive(true);
        loadingBarRenderer.gameObject.SetActive(true);

        if (SoundManager.Instance.mixingSound.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, SoundManager.Instance.mixingSound.Length);
            SoundManager.Instance.PlaySoundNormalized(SoundManager.Instance.mixingSound[randomIndex]);
        }

        while (timePassed < actualMixTime)
        {
            timePassed += Time.deltaTime;

            int stageIndex = Mathf.FloorToInt((1 - timePassed / actualMixTime) * stageCount);
            stageIndex = Mathf.Clamp(stageIndex, 0, stageCount - 1);
            loadingBarRenderer.sprite = loadingBarStages[stageIndex];

            yield return null;
        }

        // Hide everything after done mixing
        loadingBarRenderer.gameObject.SetActive(false);
        loadingBarContainer.SetActive(false);

        bool isRecipeFound = false;
        foreach (var recipe in drinkRecipes)
        {
            if (IsMixMatch(selectedIngredients, recipe.ingredients))
            {
                playerUI.SetGlassSprite(recipe.drinkSprite);
                playerUI.SetDrinkName(recipe.drinkName);
                isRecipeFound = true;
                PlayerManager.Instance.currentMix = recipe;

                DrinkPopupUI popup = UnityEngine.Object.FindFirstObjectByType<DrinkPopupUI>();
                popup?.Show(recipe.drinkName);

                break;
            }
        }

        if (!isRecipeFound)
        {
            playerUI.SetGlassSprite(mysteryDrinkData.drinkSprite);
            playerUI.SetDrinkName(mysteryDrinkData.drinkName);

            PlayerManager.Instance.currentMix = null;
            PlayerManager.Instance.mysteryDrinkReference = mysteryDrinkData;
        }

        playerUI.ClearIcons();
        PlayerManager.Instance.selectedIngredients.Clear();
        DailySummaryManager.Instance.AddCupMixed();
    }

    public bool IsMixMatch(List<IngredientSO> selectedIngredients, IngredientSO[] recipeIngredients)
    {
        if (selectedIngredients.Count != recipeIngredients.Length)
        {
            return false;
        }

        var SelectedIngredientsCopy = new List<IngredientSO>(selectedIngredients);
        var RecipeIngredientsCopy = new List<IngredientSO>(recipeIngredients);

        foreach (var ingredient in SelectedIngredientsCopy)
        {
            bool isFound = false;
            for (int i = 0; i < RecipeIngredientsCopy.Count; i++)
            {
                if (ingredient == RecipeIngredientsCopy[i])
                {
                    isFound = true;
                    RecipeIngredientsCopy.RemoveAt(i);
                    break;
                }
            }

            if (!isFound)
            {
                return false;
            }
        }

        return true;
    }
}
