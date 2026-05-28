using UnityEngine;

public class UrbanSpeedZone : MonoBehaviour
{
    [Header("Cấu hình giới hạn tốc độ")]
    public float maxSpeedLimit = 50f;        // Giới hạn tốc độ tối đa là 50 km/h
    public int penaltyPoints = 15;          // Điểm phạt mỗi lần vi phạm (Ví dụ: 15đ)
    public float penaltyCheckRate = 2f;     // Cứ mỗi 2 giây chạy quá tốc độ sẽ phạt tiếp

    private bool isPlayerInZone = false;
    private float penaltyTimer = 0f;
    private AdvancedCarController playerCar;

    // Biến cờ hiệu (Flag) để ngăn chặn việc gọi UI liên tục mỗi khung hình
    private bool hasShownOverSpeedWarning = false;

    void Update()
    {
        // Chỉ tính toán khi xe người chơi đang nằm trong vùng khu dân cư/công nghiệp
        if (isPlayerInZone && playerCar != null)
        {
            // Lấy vận tốc hiện tại của xe (Tính theo km/h từ Rigidbody)
            float currentSpeedKMH = playerCar.GetComponent<Rigidbody>().linearVelocity.magnitude * 3.6f;

            if (currentSpeedKMH > maxSpeedLimit)
            {
                // Tăng bộ đếm thời gian vi phạm công khai công bằng
                penaltyTimer += Time.deltaTime;

                // CHỈ GỌI UI MỘT LẦN DUY NHẤT KHI CHỚM QUÁ TỐC ĐỘ
                if (!hasShownOverSpeedWarning)
                {
                    TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                    if (traffic != null)
                    {
                        traffic.ShowNotification($"QUÁ TỐC ĐỘ KHU DÂN CƯ! ({currentSpeedKMH.ToString("F0")}/{maxSpeedLimit} km/h)", Color.red);
                    }
                    hasShownOverSpeedWarning = true; // Đánh dấu là đã hiện chữ, frame sau không gọi lại nữa
                }

                // Nếu quá tốc độ duy trì liên tục hết thời gian check (2 giây)
                if (penaltyTimer >= penaltyCheckRate)
                {
                    playerCar.score -= penaltyPoints;
                    Debug.LogWarning($"VI PHẠM: Chạy quá tốc độ trong khu dân cư! Trừ {penaltyPoints} điểm. Tốc độ: {currentSpeedKMH.ToString("F0")} km/h");

                    TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                    if (traffic != null)
                    {
                        traffic.ShowNotification($"VI PHẠM TỐC ĐỘ! -{penaltyPoints}đ", Color.red);
                    }

                    penaltyTimer = 0f; // Reset thời gian để nếu tiếp tục phóng nhanh thì 2s sau phạt tiếp
                }
            }
            else
            {
                // Nếu người chơi đã chủ động giảm tốc độ xuống dưới 50 km/h an toàn
                if (hasShownOverSpeedWarning)
                {
                    penaltyTimer = 0f;
                    hasShownOverSpeedWarning = false; // Reset cờ hiệu về trạng thái bình thường

                    TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                    if (traffic != null)
                    {
                        traffic.ShowNotification("TỐC ĐỘ HỢP LỆ", Color.green);
                    }
                }
            }
        }
    }

    // Khi xe đi vào biển bắt đầu khu dân cư (Biển R.420)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCar = other.GetComponent<AdvancedCarController>();
            if (playerCar != null)
            {
                isPlayerInZone = true;
                penaltyTimer = 0f;
                hasShownOverSpeedWarning = false; // Reset cờ hiệu khi vào vùng mới
                Debug.Log("Đã đi vào khu vực đông dân cư! Tốc độ tối đa giới hạn 50 km/h.");

                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null)
                {
                    traffic.ShowNotification("VÀO KHU DÂN CƯ: GIỚI HẠN 50 KM/H!", new Color(1f, 0.6f, 0f));
                }
            }
        }
    }

    // Khi xe đi qua biển hết khu dân cư (Biển R.421)
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            penaltyTimer = 0f;
            hasShownOverSpeedWarning = false;
            Debug.Log("Đã hết khu vực đông dân cư. Tốc độ trở lại bình thường.");

            TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
            if (traffic != null)
            {
                traffic.ShowNotification("HẾT KHU DÂN CƯ - TỐC ĐỘ TỰ DO", Color.green);
            }
        }
    }
}
