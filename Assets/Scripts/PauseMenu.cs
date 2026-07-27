using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu Configuration")]
    public GameObject pausePanel; // Drag and drop the Copy LosePanel or Pause Panel here.

    // Use the state variable to check if the game is paused.
    private bool isPaused = false;

    void Start()
    {
        // Ensure the Pause Menu panel is hidden when the game starts
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Restore normal time scale when starting a new level
        Time.timeScale = 1f;
    }

    // 1. PAUSE FUNCTION (Attach to the || button)
    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel != null)
        {
            pausePanel.SetActive(true); // Show the pause menu panel
        }

        // STOP TIME: All physics, AI, and vehicles will be completely frozen
        Time.timeScale = 0f;
    }

    // 2. RESUME FUNCTION (Attach to the Resume button)
    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null)
        {
            pausePanel.SetActive(false); // Hide the pause menu panel
        }

        // RESTORE TIME: Resume the game normally
        Time.timeScale = 1f;
    }

    // 3. REPEAT FUNCTION (Attach to the Repeat button)
    public void RepeatLevel()
    {
        // Reset the time scale to normal before loading to avoid freezing the game
        Time.timeScale = 1f;

        // Reload the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 4. MAIN MENU FUNCTION (Attach to the Main Menu button)
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        // Replace "MainMenu" with the exact name of your main menu scene
        SceneManager.LoadScene("MainMenu");
    }
}
