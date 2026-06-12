using UnityEngine;
using UnityEngine.UI;
using TMPro; // CHÍNH LÀ DÒNG NÀY: Khai báo thư viện để Unity nhận diện được TextMeshPro

public class RiverLevelManager : MonoBehaviour
{
    [Header("Hệ thống điểm số")]
    public int score = 100;
    public TextMeshProUGUI scoreText; // Kéo thả ScoreText của bạn vào đây (Bây giờ đã hết lỗi)
    public GameObject losePanel; // Kéo thả LosePanel vào đây

    void Start()
    {
        UpdateScoreUI();
        if (losePanel != null) losePanel.SetActive(false);
    }

    // Hàm dùng chung để trừ điểm khi vi phạm luật bến phà hoặc lao xuống nước
    public void DeductPoints(int points, string reason)
    {
        score -= points;
        if (score < 0) score = 0;

        UpdateScoreUI();
        Debug.Log("VI PHẠM: " + reason + "! Bị trừ " + points + " điểm.");

        // Nếu điểm bằng 0 thì dừng game và báo thua cuộc
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
        Time.timeScale = 0f; // Dừng toàn bộ màn chơi giống nút Pause
    }
}