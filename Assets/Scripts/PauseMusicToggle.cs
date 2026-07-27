using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMusicToggle : MonoBehaviour
{
    private AudioSource bgmAudioSource;
    private TextMeshProUGUI buttonText;
    private Button myButton; 
    private bool isMusicOn = true;

    void Start()
    {
        // 1. Automatically detect the TMP component inside this button.
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // 2. Automatically find the Button component on this object and assign the click event via code
        myButton = GetComponent<Button>();
        if (myButton != null)
        {
            myButton.onClick.AddListener(ToggleMusicInLevel);
        }

        // 3. AUTOMATICALLY FIND THE AUDIO MANAGER ACROSS SCENES
        // Find the object named "AudioManager" running in the background
        GameObject audioManagerObj = GameObject.Find("AudioManager");
        if (audioManagerObj != null)
        {
            bgmAudioSource = audioManagerObj.GetComponent<AudioSource>();
        }

        // 4. Synchronize the initial button text based on the system settings
        if (PlayerPrefs.HasKey("MusicMuted"))
        {
            isMusicOn = PlayerPrefs.GetInt("MusicMuted") == 0;
        }
        
        UpdateUI();
    }

    // Function to toggle music when the button is pressed in the pause menu
    public void ToggleMusicInLevel()
    {
        // If the persistent AudioManager is not found, try to find it again just in case
        if (bgmAudioSource == null)
        {
            GameObject audioManagerObj = GameObject.Find("AudioManager");
            if (audioManagerObj != null) bgmAudioSource = audioManagerObj.GetComponent<AudioSource>();
        }

        if (bgmAudioSource != null)
        {
            isMusicOn = !isMusicOn; // Toggle the state

            // Save the state to the system
            PlayerPrefs.SetInt("MusicMuted", isMusicOn ? 0 : 1);
            PlayerPrefs.Save();

            // Mute/unmute the actual audio
            bgmAudioSource.mute = !isMusicOn;

            // Update the button text
            UpdateUI();
            
            Debug.Log("-> [PAUSE] Đã thay đổi trạng thái nhạc toàn cục: " + (isMusicOn ? "BẬT" : "TẮT"));
        }
        else
        {
            Debug.LogError("-> [PAUSE] Không tìm thấy AudioManager bất tử chạy xuyên scene!");
        }
    }

    void UpdateUI()
    {
        if (buttonText != null)
        {
            buttonText.text = isMusicOn ? "MUSIC: ON" : "MUSIC: OFF";
        }

        // Synchronize the actual audio from AudioManager as well, if available.
        if (bgmAudioSource != null)
        {
            bgmAudioSource.mute = !isMusicOn;
        }
    }
}
