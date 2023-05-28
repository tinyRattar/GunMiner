using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SEManager : Singleton<SEManager>
{
    [SerializeField] GameObject pfb_SEPlayer;
    float volume = 1;
    [SerializeField] Text txtVolume;

    float volumeBGM = 1;
    [SerializeField] Text txtVolumeBGM;
    [SerializeField] AudioSource BGM;

    public void VolumeChange(float value)
    {
        volume += value;
        SetVolume();

    }
    public void SetVolume(float value)
    {
        volume = value;
        SetVolume();
    }

    public void VolumeBGMChange(float value)
    {
        volumeBGM += value;
        SetVolumeBGM();
    }

    public void SetVolumeBGM(float value)
    {
        volumeBGM = value;
        SetVolumeBGM();
    }


    public GameObject PlaySE(AudioClip ac, Vector3 position, float scale = 1.0f)
    {
        GameObject go = Instantiate(pfb_SEPlayer, position, Quaternion.identity);
        SEPlayer sePlayer = go.GetComponent<SEPlayer>();
        sePlayer.Play(ac, scale);

        return go;
    }

    public GameObject PlaySE(AudioClip ac, float scale = 1.0f)
    {
        return PlaySE(ac, Camera.main.transform.position, scale);
    }

    public void PlaySE(string SE_name, Vector3 position, float scale = 1.0f)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(SE_name);
        PlaySE(audioClip, position, scale);
    }
    public void PlaySE(string SE_name, float scale = 1.0f)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(SE_name);
        PlaySE(audioClip, Camera.main.transform.position, scale);
    }
    private void SetVolume()
    {
        volume = Mathf.Clamp(volume, 0, 1.0f);
        //PlayerPrefs.SetInt("volumeDecrease", 100 - (int)volume);
        //PlayerPrefs.SetInt("audioValue", (int)volume);
        if (txtVolume)
        {
            if (PlayerPrefs.GetString("language", "chinese") == "chinese")
                txtVolume.text = "音 效:" + volume;
            else
                txtVolume.text = "SFX Volume:" + volume;
        }
        pfb_SEPlayer.GetComponent<SEPlayer>().SetVolume(volume);
    }

    private void SetVolumeBGM()
    {
        volumeBGM = Mathf.Clamp(volumeBGM, 0, 1.0f);
        //PlayerPrefs.SetInt("volumeBGMDecrease", 100 - (int)volumeBGM);
        //PlayerPrefs.SetInt("audioValue", (int)volumeBGM);
        if (txtVolumeBGM)
        {
            if (PlayerPrefs.GetString("language", "chinese") == "chinese")
                txtVolumeBGM.text = "背景音:" + volumeBGM;
            else
                txtVolumeBGM.text = "BGM Volume:" + volumeBGM;
        }
        if (BGM)
            BGM.volume = volumeBGM;
    }

    private void Start()
    {
        //volume = 100 - PlayerPrefs.GetInt("volumeDecrease");
        volume = PlayerPrefs.GetFloat("audioValue", 1.0f);
        //volumeBGM = 100 - PlayerPrefs.GetInt("volumeBGMDecrease");
        volumeBGM = PlayerPrefs.GetFloat("audioValue", 1.0f);
        SetVolume();
        SetVolumeBGM();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
