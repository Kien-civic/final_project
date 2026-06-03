using UnityEngine;

public class TollBoothZone : MonoBehaviour
{
    [Header("Cấu hình Trạm")]
    public string boothName = "Trạm thu phí vào";
    public float speedLimitInsideBooth = 30f; // Tốc độ tối đa khi qua trạm (30 km/h)
    public int penaltyPoints = 30;           // Điểm phạt nếu phóng nhanh qua trạm

    private bool isPlayerInBooth = false;
    private AdvancedCarController playerCar;
    private Rigidbody carRigidbody;
    private bool hasBeenPenalized = false; // Chặn phạt liên tục trong 1 lần qua trạm

    void Update()
    {
        if (isPlayerInBooth && playerCar != null && carRigidbody != null && !hasBeenPenalized)
        {
            // Tính vận tốc thực tế (km/h)
            float currentSpeedKMH = carRigidbody.linearVelocity.magnitude * 3.6f;

            // Nếu xe chạy quá 30 km/h trong trạm thu phí
            if (currentSpeedKMH > speedLimitInsideBooth)
            {
                hasBeenPenalized = true; // Phạt 1 lần duy nhất để cảnh cáo
                playerCar.score -= penaltyPoints;

                Debug.LogWarning($"VI PHẠM: Phóng nhanh qua {boothName}! Tốc độ: {currentSpeedKMH:F0} km/h");

                // --- ĐÃ SỬA: Gọi thông qua hàm ShowNotification để kích hoạt đếm ngược 3 giây ---
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null)
                {
                    string errorMsg = $"VI PHẠM: Giảm tốc độ dưới {speedLimitInsideBooth}km/h khi qua {boothName}! -{penaltyPoints}đ";
                    traffic.ShowNotification(errorMsg, Color.red);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCar = other.GetComponent<AdvancedCarController>();
            carRigidbody = other.GetComponent<Rigidbody>();

            if (playerCar != null && carRigidbody != null)
            {
                isPlayerInBooth = true;
                hasBeenPenalized = false; // Reset trạng thái phạt cho lượt này
                Debug.Log($"Bạn đang đi vào: {boothName}. Hãy giảm tốc độ dưới {speedLimitInsideBooth} km/h!");

                // --- ĐÃ SỬA: Gọi thông qua hàm ShowNotification để chữ tự biến mất sau 3 giây ---
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null)
                {
                    string warningMsg = $"SẮP TỚI {boothName.ToUpper()}! GIẢM TỐC ĐỘ < {speedLimitInsideBooth} KM/H";
                    traffic.ShowNotification(warningMsg, Color.yellow);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInBooth = false;
            playerCar = null;
            carRigidbody = null;
            Debug.Log($"Đã ra khỏi: {boothName}");

            // Bạn có thể tùy chọn bắn chữ thông báo đã ra khỏi trạm an toàn
            TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
            if (traffic != null)
            {
                traffic.ShowNotification($"ĐÃ QUA {boothName.ToUpper()} AN TOÀN", Color.green);
            }
        }
    }
}