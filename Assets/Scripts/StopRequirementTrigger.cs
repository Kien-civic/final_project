using UnityEngine;

public class StopRequirementTrigger : MonoBehaviour
{
    [Header("Cấu hình thử thách")]
    public float requiredStopTime = 3f; // Thời gian bắt buộc phải dừng (3 giây)
    public int penaltyPoints = 50;       // Điểm phạt nếu vi phạm
    public string locationName = "Vạch đi bộ"; // Tên vị trí để hiện thông báo

    private float stopTimer = 0f;       // Bộ đếm thời gian xe đã dừng
    private bool isPlayerInside = false; // Trạng thái xe đang ở trong vùng Trigger
    private bool hasStoppedEnough = false; // Trạng thái đã dừng đủ 3 giây chưa
    private AdvancedCarController playerCar; // Lưu trữ script của xe khi đi vào

    void Update()
    {
        // Nếu xe đang ở bên trong vùng Trigger và chưa hoàn thành thử thách dừng 3s
        if (isPlayerInside && playerCar != null && !hasStoppedEnough)
        {
            // GIẢ SỬ trong AdvancedCarController bạn có biến vận tốc, hoặc tính qua Rigidbody.
            // Ở đây mình lấy vận tốc trực tiếp từ Rigidbody của xe cho chính xác nhất.
            Rigidbody rb = playerCar.GetComponent<Rigidbody>();

            // Kiểm tra nếu xe đã dừng hẳn (vận tốc xấp xỉ bằng 0)
            if (rb != null && rb.linearVelocity.magnitude < 0.1f)
            {
                stopTimer += Time.deltaTime; // Bắt đầu tích lũy thời gian dừng
                Debug.Log($"Xe đang dừng tại {locationName}: {stopTimer:F1}s / {requiredStopTime}s");

                // Nếu dừng liên tục đủ thời gian yêu cầu
                if (stopTimer >= requiredStopTime)
                {
                    hasStoppedEnough = true;
                    Debug.Log($"Chúc mừng! Đã dừng đủ {requiredStopTime}s tại {locationName}.");

                    // Hiển thị thông báo khen ngợi lên UI nếu muốn (mượn tạm warningText của TrafficSystem)
                    TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                    if (traffic != null && traffic.warningText != null)
                    {
                        traffic.warningText.text = "Đạt yêu cầu: Đã dừng đủ 3 giây!";
                        traffic.warningText.color = Color.green;
                    }
                }
            }
            else
            {
                // Nếu xe di chuyển (bỏ chân phanh), reset lại bộ đếm từ đầu (bắt buộc dừng liên tục)
                if (stopTimer > 0f && !hasStoppedEnough)
                {
                    stopTimer = 0f;
                    Debug.Log("Xe di chuyển! Bộ đếm thời gian dừng đã bị reset.");
                }
            }
        }
    }

    // Khi xe bắt đầu đi vào vùng vạch dừng
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCar = other.GetComponent<AdvancedCarController>();
            if (playerCar != null)
            {
                isPlayerInside = true;
                stopTimer = 0f;
                hasStoppedEnough = false;
                Debug.Log($"Đi vào vùng yêu cầu dừng: {locationName}. Hãy dừng xe 3 giây!");
            }
        }
    }

    // Khi xe đi ra khỏi vùng vạch dừng (vượt qua vạch)
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Nếu đi ra khỏi vùng mà CHƯA dừng đủ 3 giây -> PHẠT
            if (!hasStoppedEnough)
            {
                Debug.LogWarning($"VI PHẠM: Chưa dừng đủ 3s tại {locationName}!");

                // Thực hiện trừ 50 điểm của xe
                if (playerCar != null)
                {
                    playerCar.score -= penaltyPoints;
                }

                // Hiển thị chữ phạt màu đỏ rực lên màn hình chính
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null && traffic.warningText != null)
                {
                    traffic.warningText.text = $"VI PHẠM: Không dừng đủ 3s tại {locationName}! Trừ {penaltyPoints} điểm";
                    traffic.warningText.color = Color.red;
                }
            }

            // Reset trạng thái khi xe đã đi qua hẳn
            isPlayerInside = false;
            playerCar = null;
        }
    }
}
