using UnityEngine.Audio;
using System;
using UnityEngine;

/** A script controlling the AudioManager of the game.
 * Controls all sounds played throughout the game. */
public class AudioManager : MonoBehaviour
{
    /** An array of all possible Sounds that are playable. */
    public Sound[] sounds;

    /** DoNotDestroy. */
    public static AudioManager instance;

    /** A reference to the current soundtrack that is playing. */
    private static AudioSource currAudioSource;

    /** A reference to the name of the current soundtrack that is playing. */
    private static string currSong;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

        /** initializes all the sounds. */
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    /** Plays the sound with the given name. */
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }
        s.source.Play();
    }

    /** Plays the soundtrack with the given name. */
    public void PlaySoundtrack(string name)
    {
        if (currSong != null && currSong.Equals(name))
        {
            return;
        }
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }
        if (currAudioSource != null)
        {
            currAudioSource.Stop();
        }
        s.source.Play();
        currSong = s.name;
        currAudioSource = s.source;
    }
}

/** A class representing a SoundEffect/Soundtrack in the game. */
[System.Serializable]
public class Sound
{
    /** The name of the sound. */
    public string name;

    /** The audioClip of the sound. */
    public AudioClip clip;

    /** The volume of the sound. */
    [Range(0f, 1f)]
    public float volume;

    /** The pitch of the sound. */
    [Range(0.1f, 3f)]
    public float pitch;

    /** Whether the sound loops. */
    public bool loop;

    /** The AudioSource corresponding to this sound. */
    [HideInInspector]
    public AudioSource source;
}
