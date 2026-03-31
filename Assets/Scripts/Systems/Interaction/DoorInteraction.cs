using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TransitionsPlus
{
    public class DoorInteraction : MonoBehaviour, IInteractable
    {
        [SerializeField] private RibbonUI ribbonUI;
        [SerializeField] private DailySummaryUI summaryUI;

        private bool reminderTriggered = false;
        private float reminderDelay = 60f;
        private bool isTransitioning = false;

        private void Start()
        {
            StartCoroutine(CheckForLateReminder());
        }

        public void Interact()
        {
            if (SceneManager.GetActiveScene().name != "Level_Bar") return;

            if (isTransitioning) return;

            if (!CanLeaveBar())
            {
                NoticeUI.Instance.ShowNotice("Bar still open or customers inside!");
                return;
            }

            Debug.Log("Door opened!");
            SaveSystem.SaveGame();
            summaryUI.Show(OnSummaryComplete);
        }

        private bool CanLeaveBar()
        {
            int hour = TimeSystem.Instance.GetHour();

            if ((hour >= 18 && hour <= 23) || (hour >= 0 && hour < 2))
                return false;

            foreach (var customer in CustomerManager.Instance.GetAllCustomers())
            {
                if (customer.gameObject.activeInHierarchy)
                    return false;
            }

            return true;
        }

        private bool alreadyTriggered = false;

        public void OnSummaryComplete()
        {
            if (isTransitioning || alreadyTriggered || SceneManager.GetActiveScene().name != "Level_Bar") return;
            isTransitioning = true;
            alreadyTriggered = true;

            StartCoroutine(PlayEndingAndTransition());
        }

        private IEnumerator PlayEndingAndTransition()
        {
            yield return ribbonUI.PlayEndingShiftRoutine();

            summaryUI = null;
            ribbonUI = null;

            // Cài đặt hiệu ứng chuyển cảnh
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

            yield return new WaitForSecondsRealtime(1.5f);

            SceneManager.LoadScene("Level_Home");
        }
        private IEnumerator CheckForLateReminder()
        {
            while (!reminderTriggered && SceneManager.GetActiveScene().name == "Level_Bar")
            {
                int hour = TimeSystem.Instance.GetHour();
                int minute = TimeSystem.Instance.GetMinute();

                if (hour == 2 && minute == 0)
                {
                    reminderTriggered = true;
                    yield return new WaitForSecondsRealtime(reminderDelay);

                    if (SceneManager.GetActiveScene().name == "Level_Bar" && !isTransitioning)
                    {
                        NoticeUI.Instance.ShowNotice("Maybe you should head home...");
                    }
                }

                yield return new WaitForSecondsRealtime(5f);
            }
        }



    }
}
