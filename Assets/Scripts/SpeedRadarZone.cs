using UnityEngine;

public class SpeedRadarZone : MonoBehaviour
{
    private AdvancedCarController playerCar;
    private Rigidbody carRigidbody;
    private bool isPlayerOnHighway = false;
    private float penaltyTimer = 0f;

    [Header("Liên kết hệ thống")]
    public TrafficSystem trafficSystem;

    // --- THÊM BIẾN CỜ HIỆU NÀY ĐỂ CHẶN LẶP CHỮ ---
    private bool isTextDisplayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCar = other.GetComponent<AdvancedCarController>();
            carRigidbody = other.GetComponent<Rigidbody>();

            if (playerCar != null && carRigidbody != null)
            {
                isPlayerOnHighway = true;
                penaltyTimer = 0f; // Cho người chơi thời gian chuẩn bị (ví dụ 3 giây)
                isTextDisplayed = false; // Reset cờ khi vào vùng mới

                if (trafficSystem != null)
                {
                    // Chỉ thông báo chớm vào vùng 1 lần duy nhất
                    trafficSystem.ShowNotification("ĐÃ VÀO CAO TỐC! TỐC ĐỘ: 60 - 100 KM/H", new Color(1f, 0.6f, 0f));
                }
                Debug.Log("Đã vào đoạn đường cao tốc! Giới hạn: 60 - 100 km/h.");
            }
        }
    }

    void Update()
    {
        if (isPlayerOnHighway && carRigidbody != null && playerCar != null)
        {
            // Tính vận tốc hiện tại của xe (km/h)
            float currentSpeedKMH = carRigidbody.linearVelocity.magnitude * 3.6f;

            // Tính toán thời gian trì hoãn phạt ban đầu (Nếu có logic đếm ngược 3s)
            penaltyTimer += Time.deltaTime;

            if (penaltyTimer >= 3f) // Sau 3 giây chuẩn bị, bắt đầu bắt lỗi
            {
                // KIỂM TRA VI PHẠM: Quá tốc độ tối đa hoặc dưới tốc độ tối thiểu
                if (currentSpeedKMH > 100f || currentSpeedKMH < 60f)
                {
                    // QUAN TRỌNG: Chỉ bật chữ nếu chữ chưa được hiển thị
                    if (!isTextDisplayed && trafficSystem != null)
                    {
                        isTextDisplayed = true; // Khóa lệnh lại ngay lập tức!

                        string errorMsg = currentSpeedKMH > 100f ?
                            $"VI PHẠM: QUÁ TỐC ĐỘ CAO TỐC! ({currentSpeedKMH.ToString("F0")}/100 km/h)" :
                            $"VI PHẠM: TỐC ĐỘ DƯỚI MỨC TỐI THIỂU! ({currentSpeedKMH.ToString("F0")}/60 km/h)";

                        trafficSystem.ShowNotification(errorMsg, Color.red);

                        // Thực hiện trừ điểm của người chơi (Ví dụ trừ 10 điểm hành chính)
                        playerCar.score -= 10;
                    }
                }
                else
                {
                    // Nếu xe đã điều chỉnh tốc độ về dải an toàn (60 - 100 km/h)
                    if (isTextDisplayed)
                    {
                        isTextDisplayed = false; // Mở khóa cờ hiệu để có thể bắt lỗi tiếp nếu tái phạm
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnHighway = false;
            playerCar = null;
            carRigidbody = null;
            isTextDisplayed = false; // Reset sạch sẽ khi ra khỏi vùng

            if (trafficSystem != null)
            {
                trafficSystem.ShowNotification("RỜI CAO TỐC AN TOÀN", Color.green);
            }
            Debug.Log("Vừa rời khỏi đoạn đường cao tốc.");
        }
    }
}
