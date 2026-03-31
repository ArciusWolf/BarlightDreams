using UnityEngine;
using TMPro;

public class BarStatusUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI seatStatusText;

    private void Update()
    {
        if (ChairManager.Instance == null || ChairManager.Instance.chairs == null)
        {
            seatStatusText.text = "0 / ? Seats Taken";
            return;
        }

        int occupied = 0;
        int total = ChairManager.Instance.chairs.Count;

        foreach (var chair in ChairManager.Instance.chairs)
        {
            if (chair.isOccupied) occupied++;
        }

        seatStatusText.text = $"{occupied} / {total} Seats Taken";
    }
}
