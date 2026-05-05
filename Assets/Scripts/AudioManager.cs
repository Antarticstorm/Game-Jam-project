using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM")]
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private AudioSource bgmSource;

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Effects")]
    public AudioClip jumpSFX;
    public AudioClip coinSFX;
    public AudioClip deathSFX;
    public AudioClip landSFX;
    public AudioClip wallSlideSFX;
    public AudioClip jumpPadSFX;
    public AudioClip gameOverSFX;

    [Header("Volume")]
    [Range(0f, 1f)] public float bgmVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 0.3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayBGM();
    }
    public void SwitchBGM(AudioClip newClip)
    {
        if (bgmSource.clip == newClip) return;
        bgmSource.clip = newClip;
        bgmSource.Play();
    }

    public void PlayBGM()
    {
        if (bgmSource == null || bgmClip == null) return;
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null)
            bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
    public void PlayGameOver()
    {
        StopBGM();
        PlayGameOverSFX();
    }
    public void RestartBGM()
    {
        bgmSource.volume = bgmVolume;
        PlayBGM();
    }

    public void PlayJump() => PlaySFX(jumpSFX);
    public void PlayCoin() => PlaySFX(coinSFX);
    public void PlayDeath() => PlaySFX(deathSFX);
    public void PlayLand() => PlaySFX(landSFX);
    public void PlayWallSlide() => PlaySFX(wallSlideSFX);
    public void PlayJumpPad() => PlaySFX(jumpPadSFX);
    public void PlayGameOverSFX() => PlaySFX(gameOverSFX);
}