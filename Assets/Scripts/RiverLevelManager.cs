using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // BẮT BUỘC THÊM: Để sử dụng lệnh nạp lại màn chơi

public class RiverLevelManager : MonoBehaviour
{
    [Header("Hệ thống điểm số")]
    public int score = 100;
    public TextMeshProUGUI scoreText;
    public GameObject losePanel; // Khung bảng báo thua cuộc

    void Start()
    {
        UpdateScoreUI();
        if (losePanel != null) losePanel.SetActive(false);

        // Luôn đảm bảo thời gian chạy bình thường khi bắt đầu lại màn chơi
        Time.timeScale = 1f;
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
        Time.timeScale = 0f; // Dừng toàn bộ màn chơi
        Debug.Log("-> [GAME OVER] Người chơi đã thua cuộc tại bến phà!");
    }

    // --- HÀM MỚI: GỌI KHI ẤN NÚT RESTART TRÊN LOSE PANEL ---
    public void RestartLevel()
    {
        // Trả lại thời gian chạy thực tế trước khi load scene để tránh game bị đóng băng
        Time.timeScale = 1f;

        // Tự động lấy tên của Scene hiện tại (Level 8) để nạp lại từ đầu
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

        Debug.Log("-> [SYSTEM] Đã nạp lại màn chơi: " + currentSceneName);
    }
}
}
