using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;

    public void closeUI()
    {
        pauseMenuUI.SetActive(false);
    }

    public void openUI()
    {
        pauseMenuUI.SetActive(true);
    }

    public void returnToMainMenu()
    {
        Time.timeScale = 1f;

        SaveSystem.SaveGame();

        SceneManager.LoadScene("Main Menu");
    }
}