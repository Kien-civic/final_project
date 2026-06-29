using UnityEngine;
using TMPro;
using UnityEngine.UI; // Bắt buộc có để làm việc với Button

public class PauseMusicToggle : MonoBehaviour
{
    private AudioSource bgmAudioSource;
    private TextMeshProUGUI buttonText;
    private Button myButton;
    private bool isMusicOn = true;

    void Start()
    {
        // 1. Tự động tìm thành phần chữ TMP bên trong nút bấm này
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // 2. Tự động tìm Button trên chính đối tượng này và gán sự kiện click bằng code
        myButton = GetComponent<Button>();
        if (myButton != null)
        {
            myButton.onClick.AddListener(ToggleMusicInLevel);
        }

        // 3. TỰ ĐỘNG TÌM TỔNG ĐÀI ÂM THANH XUYÊN SCENE
        // Tìm đối tượng tên là "AudioManager" đang chạy ngầm trong game
        GameObject audioManagerObj = GameObject.Find("AudioManager");
        if (audioManagerObj != null)
        {
            bgmAudioSource = audioManagerObj.GetComponent<AudioSource>();
        }

        // 4. Đồng bộ trạng thái chữ hiển thị ban đầu dựa theo cài đặt máy
        if (PlayerPrefs.HasKey("MusicMuted"))
        {
            isMusicOn = PlayerPrefs.GetInt("MusicMuted") == 0;
        }
        
        UpdateUI();
    }

    // Hàm thực thi bật/tắt nhạc khi bấm nút ở bảng Pause
    public void ToggleMusicInLevel()
    {
        // Nếu không tìm thấy AudioManager bất tử, thử tìm lại lần nữa đề phòng
        if (bgmAudioSource == null)
        {
            GameObject audioManagerObj = GameObject.Find("AudioManager");
            if (audioManagerObj != null) bgmAudioSource = audioManagerObj.GetComponent<AudioSource>();
        }

        if (bgmAudioSource != null)
        {
            isMusicOn = !isMusicOn; // Đảo trạng thái

            // Ghi nhận trạng thái vào hệ thống máy
            PlayerPrefs.SetInt("MusicMuted", isMusicOn ? 0 : 1);
            PlayerPrefs.Save();

            // Thực hiện tắt/bật âm thanh thực tế
            bgmAudioSource.mute = !isMusicOn;

            // Cập nhật lại chữ trên nút
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
        
        // Đồng bộ cả âm thanh thực tế của AudioManager nếu có
        if (bgmAudioSource != null)
        {
            bgmAudioSource.mute = !isMusicOn;
        }
    }
}
