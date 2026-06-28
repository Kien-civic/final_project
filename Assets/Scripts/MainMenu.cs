using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Bắt buộc thêm thư viện này để điều khiển chữ trên nút Music

public class MainMenu : MonoBehaviour
{
    [Header("Giao diện Panels")]
    public GameObject howToPlayPanel;
    public GameObject settingsPanel; // Kéo SettingsPanel vào đây

    [Header("Cấu hình Âm thanh (Audio)")]
    public AudioSource bgmAudioSource;       // Kéo AudioManager (có AudioSource) vào đây
    public TextMeshProUGUI musicButtonText;  // Kéo ô Text (TMP) của nút nhạc vào đây

    private bool isMusicOn = true; // Trạng thái nhạc hiện tại

    void Start()
    {
        // Ẩn các bảng khi vừa vào game
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // ĐỌC DỮ LIỆU ĐÃ LƯU: Kiểm tra xem người chơi trước đó tắt hay bật nhạc
        if (PlayerPrefs.HasKey("MusicMuted"))
        {
            // Nếu giá trị là 1 tức là đã tắt (Mute), ngược lại là bật
            isMusicOn = PlayerPrefs.GetInt("MusicMuted") == 0;
        }

        // Áp dụng trạng thái nhạc ngay khi khởi động
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

    // --- MỤC HƯỚNG DẪN CHƠI (HOW TO PLAY) ---
    public void OpenHowToPlay() { if (howToPlayPanel != null) howToPlayPanel.SetActive(true); }
    public void CloseHowToPlay() { if (howToPlayPanel != null) howToPlayPanel.SetActive(false); }

    // --- MỤC CÀI ĐẶT (SETTINGS) ---
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // --- HÀM TỰ ĐỘNG BẬT / T T NHẠC NỀN ---
    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn; // Đổi trạng thái (Bật thành Tắt, Tắt thành Bật)

        // Lưu cài đặt âm thanh vào bộ nhớ máy (0 = Bật, 1 = Tắt)
        PlayerPrefs.SetInt("MusicMuted", isMusicOn ? 0 : 1);
        PlayerPrefs.Save();

        // Thực thi lệnh bật tắt thực tế và đổi chữ UI
        ApplyMusicState();
    }

    private void ApplyMusicState()
    {
        if (bgmAudioSource != null)
        {
            // Nếu isMusicOn = true thì mute = false (phát nhạc) và ngược lại
            bgmAudioSource.mute = !isMusicOn;
        }

        if (musicButtonText != null)
        {
            // Tự động cập nhật chữ hiển thị trên nút bấm tương ứng
            musicButtonText.text = isMusicOn ? "MUSIC: ON" : "MUSIC: OFF";
        }
    }

    public void OpenLevels() { SceneManager.LoadScene("LevelSelect"); }
    public void QuitGame() { Application.Quit(); Debug.Log("Thoát game"); }
}
