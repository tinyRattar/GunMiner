using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    public void Play(AudioClip ac, float scale = 1.0f)
    {
        audioSource.clip = ac;
        audioSource.Play();
        float lifetime = audioSource.clip.length;
        Destroy(this.gameObject, lifetime);

        audioSource.volume = audioSource.volume * scale;
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
