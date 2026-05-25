using UnityEngine;

public class EmergencyLaneZone : MonoBehaviour
{
    [Header("Cấu hình phạt")]
    public int penaltyPoints = 10;           // Số điểm bị trừ mỗi lần
    public float timeBeforePenalty = 3f;     // Thời gian giới hạn cho phép (3 giây)
    public float penaltyRepeatRate = 3f;     // Nếu cứng đầu chạy tiếp, cứ mỗi 3s phạt tiếp một lần

    private bool isPlayerInLane = false;
    private float laneTimer = 0f;
    private AdvancedCarController playerCar;

    void Update()
    {
        // Chỉ tính toán khi xe của người chơi đang nằm trong làn khẩn cấp
        if (isPlayerInLane && playerCar != null)
        {
            // Tăng bộ đếm thời gian lên theo thời gian thực (giây)
            laneTimer += Time.deltaTime;

            // Kiểm tra nếu vượt quá thời gian 3 giây quy định
            if (laneTimer >= timeBeforePenalty)
            {
                // Thực hiện trừ điểm
                playerCar.score -= penaltyPoints;

                Debug.LogWarning($"VI PHẠM: Chạy vào làn khẩn cấp quá thời gian cho phép! Trừ {penaltyPoints} điểm.");

                // Hiển thị chữ cảnh báo màu đỏ lên màn hình chính
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null && traffic.warningText != null)
                {
                    traffic.warningText.text = $"VI PHẠM: KHÔNG CHẠY VÀO LÀN KHẨN CẤP! -{penaltyPoints}đ";
                    traffic.warningText.color = Color.red;
                }

                // Reset lại bộ đếm về 0 để nếu họ tiếp tục đi cố tình chạy ở đây, cứ sau 3s nữa lại phạt tiếp
                laneTimer = 0f;

                // Mẹo nhỏ: Thay đổi timeBeforePenalty thành penaltyRepeatRate cho các lần phạt sau nếu muốn dãn cách thời gian
            }
        }
    }

    // Khi bánh xe hoặc thân xe chạm vào dải Trigger của làn khẩn cấp
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCar = other.GetComponent<AdvancedCarController>();
            if (playerCar != null)
            {
                isPlayerInLane = true;
                laneTimer = 0f; // Reset bộ đếm thời gian về 0 ngay khi vừa chạm bánh vào làn
                Debug.Log("Cảnh báo: Bạn vừa đi vào làn khẩn cấp! Hãy đưa xe trở lại làn chính trong vòng 3 giây.");

                // Hiện nhắc nhở màu vàng để người chơi kịp đánh lái ra
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null && traffic.warningText != null)
                {
                    traffic.warningText.text = "CẢNH BÁO: RỜI KHỎI LÀN KHẨN CẤP NGAY!";
                    traffic.warningText.color = Color.yellow;
                }
            }
        }
    }

    // Khi người chơi đã đánh lái đưa xe quay trở lại làn đường chính thành công
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInLane = false;
            laneTimer = 0f; // Xóa bộ đếm để không bị phạt ngầm
            Debug.Log("Đã an toàn quay trở lại làn đường chính.");

            // Xóa chữ cảnh báo hoặc chuyển thành chữ thông báo bình thường
            TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
            if (traffic != null && traffic.warningText != null)
            {
                traffic.warningText.text = ""; // Ẩn chữ đi khi đã đi đúng luật
            }
        }
    }
}
