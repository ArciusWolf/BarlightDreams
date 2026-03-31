using UnityEngine;

public enum ChairDirection { Up, Left, Right }

public class Chair : MonoBehaviour
{
    public ChairDirection direction;
    public Transform sitPoint;
    public bool isOccupied = false;

    public bool ReserveChair()
    {
        if (isOccupied) return false;
        isOccupied = true;
        return true;
    }

    public void FreeChair()
    {
        isOccupied = false;
    }
}
