using UnityEngine;

public class KeepAudio : MonoBehaviour
{
    // Create a static variable to manage uniqueness (Singleton Pattern)
    private static KeepAudio instance;

    void Awake()
    {
        // Check if a copy of AudioManager already exists.
        if (instance == null)
        {
            instance = this;

            // IMPORTANT COMMAND: Keep this object from being deleted when switching Scenes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If returning to the MainMenu and music is already playing, immediately delete the newly created copy to avoid duplicate music
            Destroy(gameObject);
        }
    }
}

