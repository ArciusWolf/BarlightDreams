using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DrinkRecipeSO))]
public class DrinkRecipeSOEditor : Editor
{
    private DrinkRecipeSO drink;

    private void OnEnable()
    {
        drink = (DrinkRecipeSO)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (drink.drinkSprite == null)
            return;

        Texture2D previewTexture = AssetPreview.GetAssetPreview(drink.drinkSprite);

        if (previewTexture == null)
            return;

        GUILayout.Space(10);
        GUILayout.Label("Sprite Preview", EditorStyles.boldLabel);
        GUILayout.Label("", GUILayout.Height(80), GUILayout.Width(80));
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), previewTexture, ScaleMode.ScaleToFit);
    }
}
