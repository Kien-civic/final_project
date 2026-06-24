using UnityEngine;

public class BrakeTriggerZone : MonoBehaviour
{
    // Thay vì kéo 1 xe, nếu có nhiều xe, ta có thể dùng mảng hoặc chỉ kích hoạt xe đi qua
    public AISameDirectionController targetAICar;

    private bool hasTriggered = false; // Biến cờ bảo vệ chống lặp vô hạn

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem có đúng là xe người chơi đâm vào không và bẫy đã dùng chưa
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (targetAICar != null)
            {
                hasTriggered = true; // Khóa bẫy lại ngay lập tức!

                targetAICar.TriggerEmergencyBrake(); // Gọi xe phanh

                Debug.LogWarning("BẪY ĐÃ KÍCH HOẠT VÀ TỰ KHÓA AN TOÀN!");

                // Hủy luôn hộp trigger này khỏi màn hình scene để giải phóng bộ nhớ
                Destroy(gameObject, 0.1f);
            }
        }
    }
}
