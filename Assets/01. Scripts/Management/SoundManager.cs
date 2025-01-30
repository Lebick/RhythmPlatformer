using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource bgmSource;

    public void PlaySFX(AudioClip sfx, float volume = 1f, float pitch = 1f)
    {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.pitch = pitch;
        newSource.PlayOneShot(sfx, volume);
        Destroy(newSource, sfx.length);
    }
}
