using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixingSystem : MonoBehaviour
{
    [Header("Recipe References")]
    [SerializeField] private DrinkRecipeSO[] drinkRecipes;
    [SerializeField] private MysteryDrinkSO mysteryDrinkData;

    [Header("References")]
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private MixingMinigame mixingMinigame;

    [Header("Sound & Effects")]
    [SerializeField] private AudioClip[] mixSuccessSounds;
    [SerializeField] private AudioClip[] mixFailSounds;

    private bool isMixing = false;

    private void Start()
    {
        Debug.Log("[MIX] MixingSystem initialized for new minigame-based mixing flow.");
    }

    public void OnPressMixButton()
    {
        if (isMixing)
        {
            NoticeUI.Instance.ShowNotice("Mixing already in progress!");
            return;
        }

        if (!playerUI.hasGlass)
        {
            NoticeUI.Instance.ShowNotice("You need a glass before mixing!");
            return;
        }

        List<IngredientSO> selectedIngredients = PlayerManager.Instance.selectedIngredients;
        if (selectedIngredients == null || selectedIngredients.Count < 2)
        {
            NoticeUI.Instance.ShowNotice("You must select at least 2 ingredients!");
            return;
        }

        // minigame
        isMixing = true;
        mixingMinigame.StartMinigame(OnMinigameSuccess);
        Debug.Log("[MIX] Minigame started!");
    }

    private void OnMinigameSuccess()
    {
        Debug.Log("[MIX] Minigame success -> Creating drink!");
        StartCoroutine(FinalizeMixCoroutine());
    }

    private IEnumerator FinalizeMixCoroutine()
    {
        yield return new WaitForSeconds(0.3f); // delay nhỏ cho cảm giác "hoàn tất"
        List<IngredientSO> selectedIngredients = PlayerManager.Instance.selectedIngredients;
        bool isRecipeFound = false;

        foreach (var recipe in drinkRecipes)
        {
            if (IsMixMatch(selectedIngredients, recipe.ingredients))
            {
                ApplyRecipeResult(recipe);
                isRecipeFound = true;
                break;
            }
        }

        if (!isRecipeFound)
        {
            ApplyMysteryResult();
        }

        playerUI.ClearIcons();
        PlayerManager.Instance.selectedIngredients.Clear();
        DailySummaryManager.Instance.AddCupMixed();
        isMixing = false;
    }

    private void ApplyRecipeResult(DrinkRecipeSO recipe)
    {
        playerUI.SetGlassSprite(recipe.drinkSprite);
        playerUI.SetDrinkName(recipe.drinkName);
        PlayerManager.Instance.currentMix = recipe;

        DrinkPopupUI popup = FindFirstObjectByType<DrinkPopupUI>();
        if (popup != null)
        {
            popup.Show(recipe.drinkName);
        }

        PlaySound(mixSuccessSounds);
        Debug.Log($"[MIX] Successfully created {recipe.drinkName}");
    }

    private void ApplyMysteryResult()
    {
        playerUI.SetGlassSprite(mysteryDrinkData.drinkSprite);
        playerUI.SetDrinkName(mysteryDrinkData.drinkName);

        PlayerManager.Instance.currentMix = null;
        PlayerManager.Instance.mysteryDrinkReference = mysteryDrinkData;

        PlaySound(mixFailSounds);
        Debug.Log("[MIX] Unknown recipe -> Mystery Drink created");
    }

    private void PlaySound(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        int randomIndex = Random.Range(0, clips.Length);
        AudioSource.PlayClipAtPoint(clips[randomIndex], Camera.main.transform.position);
    }

    private bool IsMixMatch(List<IngredientSO> selectedIngredients, IngredientSO[] recipeIngredients)
    {
        if (selectedIngredients.Count != recipeIngredients.Length)
        {
            return false;
        }

        List<IngredientSO> selectedCopy = new List<IngredientSO>(selectedIngredients);
        List<IngredientSO> recipeCopy = new List<IngredientSO>(recipeIngredients);

        foreach (var ingredient in selectedCopy)
        {
            bool found = false;
            for (int i = 0; i < recipeCopy.Count; i++)
            {
                if (ingredient == recipeCopy[i])
                {
                    found = true;
                    recipeCopy.RemoveAt(i);
                    break;
                }
            }

            if (!found)
            {
                return false;
            }
        }

        return true;
    }
}
