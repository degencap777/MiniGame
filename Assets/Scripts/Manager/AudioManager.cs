using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : BaseManager
{

    public AudioManager(GameFacade facade) : base(facade) { }
    private const string Sound_Prefix = "Sounds/";
    //public const string Sound_ArrowShoot = "ArrowShoot";
    //public const string Sound_Alert = "Alert";
    //public const string Sound_Bg_Moderate = "Bg(moderate)";
    //public const string Sound_Bg_Fast = "Bg(Fast)";
    //public const string Sound_ButtonClick = "ButtonClick";
    //public const string Sound_Miss = "Miss";
    //public const string Sound_ShootPerson = "ShootPerson";
    //public const string Sound_Timer = "Timer";

    private AudioSource bgAudioSource;
    private AudioSource normalAudioSource;

    public override void OnInit()
    {
        base.OnInit();
        GameObject audioSourceGO = new GameObject("AudioSource(GameObject)");
        audioSourceGO.transform.parent = facade.transform;
        bgAudioSource = audioSourceGO.AddComponent<AudioSource>();
        normalAudioSource = audioSourceGO.AddComponent<AudioSource>();

        //PlaySound(bgAudioSource, LoadSound(Sound_Bg_Moderate), 0.2f, true);
    }

    public void PlayBgSound(string soundName)
    {
        PlaySound(bgAudioSource, LoadSound(soundName), 0.2f, true);
    }

    public void PlayNormalSound(string soundName)
    {
        PlaySound(normalAudioSource, LoadSound(soundName), 1f);
    }
    private void PlaySound(AudioSource audioSource, AudioClip clip, float volume, bool loop = false)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.Play();
    }
    private AudioClip LoadSound(string SoundsName)
    {
        return Resources.Load<AudioClip>(Sound_Prefix + SoundsName);
    }

}
