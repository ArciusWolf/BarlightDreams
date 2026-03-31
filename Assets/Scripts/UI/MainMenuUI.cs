using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TransitionsPlus {

    public class MainMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        public Button continueButton;
        public Button newGameButton;
        public Button exitGameButton;

        private bool isTransitioning = false;
        public static bool isNewGame = false;

        private void Start()
        {
            Time.timeScale = 1f;

            int savedDay = PlayerPrefs.GetInt("CurrentDay", 1);
            continueButton.interactable = savedDay > 1;
        }

        public void StartNewGameFromButton()
        {
            Debug.Log("[MainMenuUI] New Game button clicked.");
            if (isTransitioning) return;
            isTransitioning = true;

            SaveSystem.ResetSave();
            isNewGame = true;

            SceneManager.sceneLoaded += OnSceneLoadedLoadGame;
            PlayTransitionToBar();
        }

        public void ContinueGameFromButton()
        {
            if (isTransitioning) return;
            isTransitioning = true;

            isNewGame = false;

            SceneManager.sceneLoaded += OnSceneLoadedLoadGame;
            PlayTransitionToBar();
        }

        public void ExitGameFromButton()
        {
            Application.Quit();
        }

        private void PlayTransitionToBar()
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
            new GradientColorKey(new Color(0.8f, 0.2f, 1f), 0f),
            new GradientColorKey(new Color(0.1f, 0.4f, 1f), 1f)
                },
                new GradientAlphaKey[] {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 1f)
                }
            );

            TransitionAnimator.Start(
                type: TransitionType.SeaWaves,
                duration: 1.5f,
                gradient: gradient,
                rotation: 90f,
                splits: 2
            );

            SceneManager.LoadScene("Level_Bar");
        }


        private void OnSceneLoadedLoadGame(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Level_Bar")
            {
                Time.timeScale = 1f;

                if (isNewGame)
                {
                    TimeSystem.Instance.OverrideDay(1);
                    TimeSystem.Instance.ResetTime();
                    Debug.Log("[MainMenuUI] Started new game at Day 1.");
                }
                else
                {
                    SaveSystem.LoadGame();
                    Debug.Log("[MainMenuUI] Loaded saved game.");
                }

                isNewGame = false;
                SceneManager.sceneLoaded -= OnSceneLoadedLoadGame;
            }
        }

    }
}
