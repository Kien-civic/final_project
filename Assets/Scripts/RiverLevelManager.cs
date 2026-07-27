using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 

public class RiverLevelManager : MonoBehaviour
{
    [Header("Score System")]
    public int score = 100;
    public TextMeshProUGUI scoreText;
    public GameObject losePanel; // Lose announcement board

    void Start()
    {
        UpdateScoreUI();
        if (losePanel != null) losePanel.SetActive(false);

        // Always ensure normal runtime when restarting the game.
        Time.timeScale = 1f;
    }

    // Common function to deduct points when violating ferry rules or falling into water
    public void DeductPoints(int points, string reason)
    {
        score -= points;
        if (score < 0) score = 0;

        UpdateScoreUI();
        Debug.Log("VI PHẠM: " + reason + "! Bị trừ " + points + " điểm.");

        // If the score reaches 0, stop the game and declare a loss
        if (score <= 0)
        {
            GameOver();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Điểm: " + score;
        }
    }

    void GameOver()
    {
        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f; // Stop the entire game
        Debug.Log("-> [GAME OVER] Người chơi đã thua cuộc tại bến phà!");
    }

    // --- NEW FUNCTION: CALL WHEN THE RESTART BUTTON IS PRESSED ON THE LOSE PANEL ---
    public void RestartLevel()
    {
        // Restore the actual runtime before loading the scene to avoid the game freezing.
        Time.timeScale = 1f;

        // Automatically retrieve the name of the current Scene (Level 8) to reload from scratch.
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

        Debug.Log("-> [SYSTEM] Đã nạp lại màn chơi: " + currentSceneName);
    }
}
