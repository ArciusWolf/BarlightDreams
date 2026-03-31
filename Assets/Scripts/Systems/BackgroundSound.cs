using UnityEngine;

public class AudioBootstrap : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayBackgroundMusic();
    }
}
