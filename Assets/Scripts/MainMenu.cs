using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject howToPlayPanel;
    public GameObject settingsPanel; // Drag the Settings Panel here.

    [Header("Audio Configuration")]
    public AudioSource bgmAudioSource;       // Drag the AudioManager (with AudioSource) here
    public TextMeshProUGUI musicButtonText;  // Drag the Text (TMP) of the music button here

    private bool isMusicOn = true; // Current music state

    void Start()
    {
        // Hide panels when the game starts
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // READ SAVED DATA: Check if the player previously turned off or on the music
        if (PlayerPrefs.HasKey("MusicMuted"))
        {
            // If the value is 1, it means muted, otherwise it's on
            isMusicOn = PlayerPrefs.GetInt("MusicMuted") == 0;
        }

        // Apply the music state immediately on startup
        ApplyMusicState();
    }

    public void PlayGame()
    {
        if (PlayerPrefs.HasKey("SavedLevelIndex"))
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("SavedLevelIndex"));
        }
        else
        {
            SceneManager.LoadScene("Level1");
        }
    }

    // --- HOW TO PLAY ---
    public void OpenHowToPlay() { if (howToPlayPanel != null) howToPlayPanel.SetActive(true); }
    public void CloseHowToPlay() { if (howToPlayPanel != null) howToPlayPanel.SetActive(false); }

    // --- SETTINGS ---
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // --- TOGGLE MUSIC ---
    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn; // Toggle the state (On to Off, Off to On)

        // Save the audio settings to the device memory (0 = On, 1 = Off)
        PlayerPrefs.SetInt("MusicMuted", isMusicOn ? 0 : 1);
        PlayerPrefs.Save();

        // Apply the actual toggle and update the UI text
        ApplyMusicState();
    }

    private void ApplyMusicState()
    {
        if (bgmAudioSource != null)
        {
            // If isMusicOn = true then mute = false (play music) and vice versa
            bgmAudioSource.mute = !isMusicOn;
        }

        if (musicButtonText != null)
        {
            // Automatically update the text displayed on the button accordingly
            musicButtonText.text = isMusicOn ? "MUSIC: ON" : "MUSIC: OFF";
        }
    }

    public void OpenLevels() { SceneManager.LoadScene("LevelSelect"); }
    public void QuitGame() { Application.Quit(); Debug.Log("Thoát game"); }
}
