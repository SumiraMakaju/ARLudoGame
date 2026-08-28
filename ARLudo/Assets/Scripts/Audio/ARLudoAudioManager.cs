using UnityEngine;

public class ARLudoAudioManager : MonoBehaviour
{
    public static ARLudoAudioManager Instance;
    
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    public AudioClip bgm;
    public AudioClip clickSound;
    public AudioClip diceRollSound;
    public AudioClip pawnHopSound;
    public AudioClip pawnCaptureSound;
    public AudioClip pawnGoalSound;
    public AudioClip winSound;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    void Start()
    {
        if (musicSource != null && bgm != null)
        {
            musicSource.volume = 0.05f; 
            musicSource.clip = bgm;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    
    public void PlayClick() { if (sfxSource != null && clickSound != null) sfxSource.PlayOneShot(clickSound); }
    public void PlayDice() { if (sfxSource != null && diceRollSound != null) sfxSource.PlayOneShot(diceRollSound); }
    public void PlayHop() { if (sfxSource != null && pawnHopSound != null) sfxSource.PlayOneShot(pawnHopSound); }
    public void PlayCapture() { if (sfxSource != null && pawnCaptureSound != null) sfxSource.PlayOneShot(pawnCaptureSound); }
    public void PlayGoal() { if (sfxSource != null && pawnGoalSound != null) sfxSource.PlayOneShot(pawnGoalSound); }
    public void PlayWin() { if (sfxSource != null && winSound != null) sfxSource.PlayOneShot(winSound); }
}