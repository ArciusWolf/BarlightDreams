using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MixingMinigame : MonoBehaviour
{
    [Header("UI References")]
    public Transform gridParent;
    public GameObject upPrefab;
    public GameObject downPrefab;
    public GameObject leftPrefab;
    public GameObject rightPrefab;

    [Header("Sprites - Normal")]
    public Sprite upNormal;
    public Sprite downNormal;
    public Sprite leftNormal;
    public Sprite rightNormal;

    [Header("Sprites - Correct (Green)")]
    public Sprite upGreen;
    public Sprite downGreen;
    public Sprite leftGreen;
    public Sprite rightGreen;

    [Header("Sprites - Wrong (Red)")]
    public Sprite upRed;
    public Sprite downRed;
    public Sprite leftRed;
    public Sprite rightRed;

    [Header("Settings")]
    public float resetDelay = 1f;

    private List<string> sequence = new List<string>();
    private List<Image> arrowImages = new List<Image>();
    private int currentIndex = 0;
    private System.Action onComplete;
    private bool isActive = false;

    public void StartMinigame(System.Action onSuccess)
    {
        onComplete = onSuccess;
        gameObject.SetActive(true);
        GenerateSequence();
        DisplaySequence();
        currentIndex = 0;
        isActive = true;
    }

    private void GenerateSequence()
    {
        sequence.Clear();

        int currentDay = 1;
        if (TimeSystem.Instance != null)
        {
            currentDay = TimeSystem.Instance.CurrentDay;
        }

        // Difficulty scaling by day
        // Day 1 → min 3 max 5
        // Day 5+ → min 5 max 8
        float t = Mathf.Clamp01((currentDay - 1) / 4f); // normalized 0 → 1
        int minLength = Mathf.RoundToInt(Mathf.Lerp(3, 5, t));
        int maxLength = Mathf.RoundToInt(Mathf.Lerp(5, 8, t));

        int length = Random.Range(minLength, maxLength + 1);

        string[] dirs = { "Up", "Down", "Left", "Right" };
        for (int i = 0; i < length; i++)
        {
            sequence.Add(dirs[Random.Range(0, dirs.Length)]);
        }

        Debug.Log($"[MIX-MINIGAME] Day {currentDay} generated sequence length {length}");
    }

    private void DisplaySequence()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        arrowImages.Clear();

        foreach (string dir in sequence)
        {
            GameObject prefab = null;
            if (dir == "Up") prefab = upPrefab;
            else if (dir == "Down") prefab = downPrefab;
            else if (dir == "Left") prefab = leftPrefab;
            else if (dir == "Right") prefab = rightPrefab;

            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab, gridParent);
                Image img = obj.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = GetSprite(dir, "normal");
                    arrowImages.Add(img);
                }
            }
        }
    }

    public void OnPressDirection(string direction)
    {
        if (!isActive || sequence.Count == 0) return;

        if (direction == sequence[currentIndex])
        {
            // ✅ Correct input
            UpdateArrowColor(currentIndex, "green");
            currentIndex++;

            if (currentIndex >= sequence.Count)
            {
                StartCoroutine(CompleteMinigame());
            }
        }
        else
        {
            // ❌ Wrong input
            StartCoroutine(ShowWrongFeedback());
        }
    }

    private void UpdateArrowColor(int index, string colorType)
    {
        if (index < 0 || index >= arrowImages.Count) return;

        string dir = sequence[index];
        arrowImages[index].sprite = GetSprite(dir, colorType);
    }

    private IEnumerator ShowWrongFeedback()
    {
        // turn all to red
        for (int i = 0; i < arrowImages.Count; i++)
        {
            string dir = sequence[i];
            arrowImages[i].sprite = GetSprite(dir, "red");
        }

        yield return new WaitForSeconds(resetDelay);

        // reset all to normal
        for (int i = 0; i < arrowImages.Count; i++)
        {
            string dir = sequence[i];
            arrowImages[i].sprite = GetSprite(dir, "normal");
        }

        currentIndex = 0;
    }

    private IEnumerator CompleteMinigame()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
        isActive = false;
        onComplete?.Invoke();
    }

    private Sprite GetSprite(string dir, string type)
    {
        switch (type)
        {
            case "green":
                if (dir == "Up") return upGreen;
                if (dir == "Down") return downGreen;
                if (dir == "Left") return leftGreen;
                if (dir == "Right") return rightGreen;
                break;

            case "red":
                if (dir == "Up") return upRed;
                if (dir == "Down") return downRed;
                if (dir == "Left") return leftRed;
                if (dir == "Right") return rightRed;
                break;

            default:
                if (dir == "Up") return upNormal;
                if (dir == "Down") return downNormal;
                if (dir == "Left") return leftNormal;
                if (dir == "Right") return rightNormal;
                break;
        }
        return null;
    }
}
