using TransitionsPlus;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    // Singleton Pattern
    public static GameOverUI Instance { get; private set; }
    [SerializeField] private GameObject gameOverPanel;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Show()
    {
        UIAnimator.Show(gameOverPanel, 0.4f);
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        UIAnimator.Hide(gameOverPanel, 0.4f);
    }

    public void OnPlayAgain()
    {
        Time.timeScale = 1f;
        SaveSystem.ResetSave();
        MainMenuUI.isNewGame = true;

        SceneManager.sceneLoaded += OnSceneLoadedNewGame;
        SceneManager.LoadScene("Level_Bar");
    }
    private void OnSceneLoadedNewGame(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level_Bar")
        {
            TimeSystem.Instance.OverrideDay(1);
            TimeSystem.Instance.ResetTime();

            PlayerManager.Instance.RecalculateMaxLife();
            PlayerManager.Instance.moneyHeld = 0;
        }

        SceneManager.sceneLoaded -= OnSceneLoadedNewGame;
    }


    public void OnBackToMenu()
    {
        Time.timeScale = 1f;

        SaveSystem.ResetSave();
        MainMenuUI.isNewGame = true;

        SceneManager.LoadScene("Main Menu");
    }

}
