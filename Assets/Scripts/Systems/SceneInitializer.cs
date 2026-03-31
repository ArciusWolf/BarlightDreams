using UnityEngine;

public class BarSceneLoader : MonoBehaviour
{
    void Start()
    {
        if (!BedInteraction.fromSleep)
        {
            SaveSystem.LoadGame();
        }
        else
        {
            BedInteraction.fromSleep = false;
        }
    }

}
