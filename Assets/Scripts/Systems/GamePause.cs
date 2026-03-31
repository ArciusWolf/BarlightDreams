using UnityEngine;
using System.Collections.Generic;

public class GamePause : MonoBehaviour
{
    [Tooltip("UI Active = Pause game")]
    public List<GameObject> uiList = new List<GameObject>();

    private bool isPaused = false;

    void Update()
    {
        bool anyUIOpen = false;

        foreach (GameObject ui in uiList)
        {
            if (ui != null && ui.activeInHierarchy)
            {
                anyUIOpen = true;
                break;
            }
        }

        if (anyUIOpen && !isPaused)
        {
            PauseGame();
        }
        else if (!anyUIOpen && isPaused)
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }
}
