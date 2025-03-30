using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }

    public List<Sound> sounds = new List<Sound>();
    private Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        Instance = this;
        foreach (Sound sound in sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = sound.clip;
            audioSources[sound.name] = source;
        }
    }

    public static void Play(string name)
    {
        if (Instance.audioSources.TryGetValue(name, out AudioSource source))
            source.Play();
    }

    public static void Stop(string name)
    {
        if (Instance.audioSources.TryGetValue(name, out AudioSource source))
            source.Stop();
    }
}
