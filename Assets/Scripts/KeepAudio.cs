using UnityEngine;

public class KeepAudio : MonoBehaviour
{
    // Tạo một biến static để quản lý độc nhất (Singleton Pattern)
    private static KeepAudio instance;

    void Awake()
    {
        // Kiểm tra xem đã có một bản sao của AudioManager nào tồn tại chưa
        if (instance == null)
        {
            instance = this;

            // LỆNH QUAN TRỌNG: Giữ đối tượng này không bị xóa khi chuyển Scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu quay lại MainMenu mà đã có nhạc đang chạy, xóa ngay bản sao mới tạo để tránh trùng nhạc
            Destroy(gameObject);
        }
    }
}
