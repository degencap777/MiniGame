using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : BaseManager
{

    public AudioManager(GameFacade facade) : base(facade) { }
    public bool musicOn = true;
    public bool soundOn = true;
    private float _musicVolume = 1;
    private float _soundVolume = 1;
    private const string Sound_Prefix = "Sounds";
    private GameObject audioManager;
    private AudioSource mainMusic;
    private List<AudioSource> sounds=new List<AudioSource>();
    public Dictionary<string,AudioClip> Clips { get; private set; }

    public float musicVolume
    {
        get { return _musicVolume; }
        set
        {
            if (value != _musicVolume)
            {
                _musicVolume = value;
                mainMusic.volume = value;
            }
        }
    }

    public float soundVolume
    {
        get { return _soundVolume; }
        set
        {
            if (value != _soundVolume)
            {
                _soundVolume = value;
                foreach (AudioSource src in sounds)
                {
                    src.volume = _soundVolume;
                }
            }
        }
    }
    public override void OnInit()
    {
        base.OnInit();
        audioManager = new GameObject("AudioManager");
        audioManager.transform.parent = facade.transform;
        mainMusic = audioManager.AddComponent<AudioSource>();
        Clips = Resources.LoadAll<AudioClip>(Sound_Prefix).ToDictionary(x=> x.name);
    }

    public void PlayMusic(AudioClip music)
    {
        PlayMusic(music, true);
    }
    public void PlayMusic(AudioClip music, bool loop)
    {
        mainMusic.Stop();
        mainMusic.clip = music;
        mainMusic.loop = loop;
        mainMusic.volume = musicVolume;
        if (musicOn && Time.timeScale != 0)
        {
            mainMusic.Play();
        }
    }

    public void StopMusic()
    {
        mainMusic.Stop();
    }

    public void PauseMusic()
    {
        mainMusic.Pause();
    }

    public void ResumeMusic()
    {
        if (musicOn && Time.timeScale != 0)
        {
            mainMusic.Play();
        }
    }

    public void PlaySound(AudioClip sound)
    {
        PlaySound(sound, false);
    }
    public void PlaySound(AudioClip sound, bool loop)
    {
        AudioSource source = audioManager.AddComponent<AudioSource>();
        source.clip = sound;
        source.volume = soundVolume;
        source.loop = loop;
        sounds.Add(source);
        if (soundOn && Time.timeScale != 0)
        {
            source.Play();
        }
        if (!loop)
        {
            audioManager.AddComponent<DoDestroy>().Set(() => sounds.Remove(source), source, sound.length);
        }
    }
    

    public void PauseAllSounds()
    {
        foreach (AudioSource src in sounds)
        {
            src.Pause();
        }
    }

    public void ResumeAllSounds()
    {
        if (soundOn && Time.timeScale != 0)
        {
            foreach (AudioSource src in sounds)
            {
                src.Play();
            }
        }
    }

    public void Clear()
    {
        foreach (AudioSource src in sounds)
        {
            Object.Destroy(src);
        }
        sounds.Clear();
    }
}
