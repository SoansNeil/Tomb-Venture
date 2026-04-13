using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    public AudioClip gameplayMusic;
    public AudioClip menuMusic;
    public AudioClip gameOverMusic;
    public AudioClip coinSound;
    public AudioClip jumpSound;
    public AudioClip damageSound;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Room1")
    {
        ChangeMusic(gameplayMusic);
    }
    else if (scene.name == "Menu")
    {
        ChangeMusic(menuMusic);
    }
    else if (scene.name == "GameOver")
    {
        ChangeMusic(gameOverMusic);
    }
    }

    void Start()
    {
        PlayMusic(menuMusic);
    }
    
    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }
    
    public void PlaySoundEffect(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public void ChangeMusic(AudioClip newClip)
{
    if (musicSource.clip == newClip)
    {
        return; // Already playing this track
    }
    
    musicSource.Stop();
    PlayMusic(newClip);
}
    void OnDestroy()
{
    SceneManager.sceneLoaded -= OnSceneLoaded; // ← always unsubscribe
}

}