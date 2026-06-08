using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Bảng Menu Tạm Dừng")]
    public GameObject pausePanel; // Kéo thả LosePanel sao chép hoặc Panel Tạm dừng vào đây

    // Biến trạng thái để kiểm tra game có đang dừng không
    private bool isPaused = false;

    void Start()
    {
        // Đảm bảo lúc mới vào màn chơi, bảng Pause Menu phải ẩn đi
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Trả lại tốc độ thời gian bình thường khi bắt đầu màn mới
        Time.timeScale = 1f;
    }

    // 1. CHỨC NĂNG TẠM DỪNG (Gắn vào nút ||)
    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel != null)
        {
            pausePanel.SetActive(true); // Hiện bảng menu lên
        }

        // NGỪNG THỜI GIAN: Mọi chuyển động vật lý, AI, xe cộ sẽ đứng yên hoàn toàn
        Time.timeScale = 0f;
    }

    // 2. CHỨC NĂNG TIẾP TỤC (Gắn vào nút Resume)
    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null)
        {
            pausePanel.SetActive(false); // Ẩn bảng menu đi
        }

        // KHÔI PHỤC THỜI GIAN: Tiếp tục trò chơi bình thường
        Time.timeScale = 1f;
    }

    // 3. CHỨC NĂNG CHƠI LẠI (Gắn vào nút Repeat)
    public void RepeatLevel()
    {
        // Đặt lại thời gian về bình thường trước khi load để tránh bị đứng game
        Time.timeScale = 1f;

        // Load lại chính Scene hiện tại đang chơi
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 4. CHỨC NĂNG VỀ MÀN HÌNH CHÍNH (Gắn vào nút Main Menu)
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        // Hãy thay tên "MainMenu" bằng tên chính xác của Scene màn hình chính của bạn
        SceneManager.LoadScene("MainMenu");
    }
}
