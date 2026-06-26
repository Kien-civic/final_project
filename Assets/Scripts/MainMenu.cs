using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // 1. Kiểm tra xem trong bộ nhớ máy đã từng lưu khóa "SavedLevelIndex" chưa
        if (PlayerPrefs.HasKey("SavedLevelIndex"))
        {
            // Lấy ra index đã lưu
            int savedLevel = PlayerPrefs.GetInt("SavedLevelIndex");

            Debug.Log("-> [BACKEND] Tìm thấy tiến trình chơi cũ! Đang tải Level Index: " + savedLevel);

            // Tải chính xác Level vừa thoát ra
            SceneManager.LoadScene(savedLevel);
        }
        else
        {
            // 2. Nếu người chơi mới tinh, chưa từng chơi màn nào, mặc định nạp Level 1 (Index của Level 1 thường là 2)
            Debug.Log("-> Chơi lần đầu, nạp mặc định Level 1");

            SceneManager.LoadScene("Level1"); // Hoặc dùng số index: SceneManager.LoadScene(2);
        }
    }

    public void OpenLevels()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Thoát game");
    }

    // MẸO NHỎ (Tùy chọn): Tạo thêm một nút "Xóa tiến trình" nếu muốn người chơi chơi lại từ đầu
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("SavedLevelIndex");
        Debug.Log("Đã xóa sạch tiến trình chơi cũ!");
    }
}
