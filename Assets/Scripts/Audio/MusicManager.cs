using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 0.5f;
    }

    [SerializeField] private SceneMusic[] sceneMusic;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float crossfadeDuration = 1.5f;

    private string currentScene = "";
    private float targetVolume;
    private float crossfadeSpeed;
    private System.Random rng = new System.Random();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        targetVolume = 0f;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == currentScene) return;
        currentScene = scene.name;

        var entries = GetEntriesForScene(scene.name);
        if (entries.Count > 0)
        {
            var entry = entries[rng.Next(entries.Count)];
            crossfadeSpeed = 1f / crossfadeDuration;
            audioSource.clip = entry.clip;
            audioSource.volume = 0f;
            targetVolume = entry.volume;
            audioSource.Play();
        }
        else
        {
            crossfadeSpeed = 1f / crossfadeDuration;
            targetVolume = 0f;
        }
    }

    private void Update()
    {
        if (Mathf.Abs(audioSource.volume - targetVolume) > 0.001f)
        {
            audioSource.volume = Mathf.MoveTowards(
                audioSource.volume, targetVolume,
                crossfadeSpeed * Time.deltaTime
            );
            if (audioSource.volume <= 0.001f && targetVolume <= 0f)
                audioSource.Stop();
        }
    }

    private List<SceneMusic> GetEntriesForScene(string sceneName)
    {
        var result = new List<SceneMusic>();
        foreach (var entry in sceneMusic)
        {
            if (entry.sceneName == sceneName)
                result.Add(entry);
        }
        return result;
    }

    public void SetVolume(float vol)
    {
        targetVolume = vol;
    }
}
