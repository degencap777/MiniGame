using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingItem : MonoBehaviour {

    private Toggle musicToggle;
    private Toggle soundToggle;

    private Slider musicSlider;
    private Slider soundSlider;
    private Button closeButton;

    void Awake()
    {
        transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseClick);
        musicToggle = transform.Find("MusicToggle").GetComponent<Toggle>();
        soundToggle = transform.Find("SoundToggle").GetComponent<Toggle>();
        musicSlider = transform.Find("MusicSlider").GetComponent<Slider>();
        soundSlider = transform.Find("SoundSlider").GetComponent<Slider>();
        musicToggle.onValueChanged.AddListener(GameFacade.Instance.OnMusicChanged);
        soundToggle.onValueChanged.AddListener(GameFacade.Instance.OnSoundChanged);
        musicSlider.onValueChanged.AddListener(GameFacade.Instance.OnMusicSliderValueChange);
        soundSlider.onValueChanged.AddListener(GameFacade.Instance.OnSoundSliderValueChange);
        gameObject.SetActive(false);
    }

    void Start()
    {
        GameFacade.Instance.SetAudioSetting(musicToggle.isOn, soundToggle.isOn, musicSlider.value, soundSlider.value);
    }

    private void OnCloseClick()
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        GameFacade.Instance.PlaySound("Setting");
    }
}
