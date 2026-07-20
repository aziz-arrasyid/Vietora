using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource sfxSource;
    public AudioSource bgmSource;
    public AudioSource narratorSource;

    [Header("Audio Clip")]
    public AudioClip clickSound;
    public AudioClip notifSound;
    public AudioClip quizzTimeSound;
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip bgmHome;
    public AudioClip bgmGameplay;
    public AudioClip bgmQuizz;

    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameManager.instance.settings.OnNarratorChanged += OnChangeNarrator;
    }

    public void ChangeBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    private void OnChangeNarrator()
    {
        if (narratorSource == null) return;

        narratorSource.mute = !GameManager.instance.settings.Narrator;
    }

    public void PlaySFXSound(AudioClip clip) => sfxSource.PlayOneShot(clip);
}
