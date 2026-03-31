using UnityEngine;
using UnityEngine.SceneManagement;

public class BedInteraction : MonoBehaviour, IInteractable
{
    public static bool fromSleep = false;

    public void Interact()
    {
        if (TimeSystem.Instance == null)
        {
            Debug.LogWarning("TimeSystem not found! Cannot advance day.");
            return;
        }
        SaveSystem.SaveGame();
        TimeSystem.Instance.NextDay();
        fromSleep = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Level_Bar");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level_Bar")
        {
            TimeSystem.Instance.ForceDayRefresh();
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
