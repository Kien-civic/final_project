using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    public AudioSource musicSource;

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }
}
