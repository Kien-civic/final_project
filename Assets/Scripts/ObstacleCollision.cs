using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("VA CHẠM: Bạn đã đâm vào vỉa hè/chướng ngại vật!");

            AdvancedCarController car = collision.gameObject.GetComponent<AdvancedCarController>();
            if (car != null)
            {
                car.score -= 10;

                // --- ĐOẠN CODE HIỂN THỊ CHỮ LÊN MÀN HÌNH CHÍNH ---
                // Mượn ô chữ warningText từ hệ thống quản lý hoặc ép trực tiếp qua biến của xe
                if (car.scoreText != null)
                {
                    // Nếu bạn có một ô chữ thông báo riêng trên xe (ví dụ: warningText) thì dùng nó.
                    // Ở đây, để nhanh gọn, ta gọi hệ thống quản lý Đèn hoặc UI để hiện chữ.
                    // Cách đơn giản nhất là tìm Script TrafficSystem đang có trong Scene để ké ô chữ cảnh báo của nó:
                    TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                    if (traffic != null && traffic.warningText != null)
                    {
                        traffic.warningText.text = "VI PHẠM: Va chạm vỉa hè! Trừ 10 điểm";
                        traffic.warningText.color = Color.yellow; // Đổi sang màu vàng cho phân biệt với đèn đỏ
                    }
                }
            }
        }
    }
}
