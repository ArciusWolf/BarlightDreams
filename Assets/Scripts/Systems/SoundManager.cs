using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Player Sound")]
    public AudioClip[] playerWalkSound;

    [Header("Back Counter Sound")]
    public AudioClip[] iceSound;
    public AudioClip[] trashSound;
    public AudioClip[] mixingSound;
    public AudioClip[] ingredientsPickSound;
    public AudioClip[] glassPickUpSound;

    [Header("UI Sound")]
    public AudioClip[] recipeBookOpenSound;

    [Header("Background Music")]
    public AudioClip[] backgroundMusics;
    public AudioSource bgmSource;
    public AudioSource musicSource;

    [Header("Customer Sound")]
    public AudioClip[] customerSound;
    private AudioSource audioSource;

    [Header("Money")]
    public AudioClip[] moneyReceivedSound;

    [Header("Lose Life Sound")]
    public AudioClip[] loseLifeSound;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = 0.3f;
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning("AudioClip is null.");
        }
    }

    public void PlaySoundNormalized(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null.");
            return;
        }

        float[] data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);

        float sum = 0f;
        for (int i = 0; i < data.Length; i++)
            sum += data[i] * data[i];
        float rms = Mathf.Sqrt(sum / data.Length);

        float target = 0.1f;
        float volume = target / Mathf.Max(rms, 0.0001f);

        volume = Mathf.Clamp(volume, 0.2f, 1f);

        audioSource.PlayOneShot(clip, volume);
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusics.Length == 0 || bgmSource == null)
        {
            Debug.LogWarning("No background music clips or source assigned!");
            return;
        }

        int randomIndex = Random.Range(0, backgroundMusics.Length);
        bgmSource.clip = backgroundMusics[randomIndex];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }

    public void PlayRandomCustomerSound()
    {
        if (customerSound.Length > 0)
        {
            int index = Random.Range(0, customerSound.Length);
            PlaySoundNormalized(customerSound[index]);
        }
    }

    public void PlayMoneySound()
    {
        if (moneyReceivedSound.Length > 0)
        {
            int index = Random.Range(0, moneyReceivedSound.Length);
            PlaySoundNormalized(moneyReceivedSound[index]);
        }
    }

    public void PlayLoseLifeSound()
    {
        if (loseLifeSound.Length > 0)
        {
            int index = Random.Range(0, loseLifeSound.Length);
            PlaySoundNormalized(loseLifeSound[index]);
        }
    }
}
