using UnityEngine;

public class AICarCollision : MonoBehaviour
{
    [Header("Cấu hình phạt va chạm")]
    public int penaltyPoints = 50; // Số điểm phạt khi va chạm là 50 điểm

    // Hàm này tự động kích hoạt khi có va chạm vật lý (Collision) giữa 2 khối Collider
    private void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra xem vật thể đâm vào xe AI này có phải là xe người chơi (Player) hay không
        if (collision.gameObject.CompareTag("Player"))
        {
            // 1. Tìm script điều khiển trên xe người chơi để thực hiện trừ điểm
            AdvancedCarController playerCar = collision.gameObject.GetComponent<AdvancedCarController>();

            if (playerCar != null)
            {
                // Trừ 50 điểm trong dữ liệu của xe
                playerCar.score -= penaltyPoints;
                Debug.LogWarning($"VA CHẠM GIAO THÔNG: Va chạm với xe AI trên đường! Trừ {penaltyPoints} điểm.");

                // 2. Gọi TrafficSystem để bắn chữ đỏ giật gân lên màn hình chính
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null)
                {
                    traffic.ShowNotification($"TAI NẠN GIAO THÔNG! Trừ {penaltyPoints}đ", Color.red);
                }
            }
        }
    }
}