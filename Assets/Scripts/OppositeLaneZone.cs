using UnityEngine;

public class OppositeLaneZone : MonoBehaviour
{
    public int penaltyPoints = 20;           // Lấn làn phạt nặng: 20 điểm
    public float timeBeforePenalty = 2f;     // Chỉ cho phép quá tay 2 giây là phạt luôn

    private bool isPlayerViolating = false;
    private float violationTimer = 0f;
    private AdvancedCarController playerCar;

    void Update()
    {
        if (isPlayerViolating && playerCar != null)
        {
            violationTimer += Time.deltaTime;

            if (violationTimer >= timeBeforePenalty)
            {
                playerCar.score -= penaltyPoints;
                Debug.LogError($"CẢNH BÁO NGUY HIỂM: Bạn đang đi ngược chiều! Trừ {penaltyPoints} điểm.");

                // Đẩy thông báo khẩn cấp lên UI chính
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null && traffic.warningText != null)
                {
                    traffic.warningText.text = $"VI PHẠM: ĐI SAI PHẦN ĐƯỜNG NGƯỢC CHIỀU! -{penaltyPoints}đ";
                    traffic.warningText.color = Color.red;
                }

                violationTimer = 0f; // Tiếp tục phạt sau mỗi 2s nếu không chịu về làn
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCar = other.GetComponent<AdvancedCarController>();
            if (playerCar != null)
            {
                
                isPlayerViolating = true;
                violationTimer = 0f;

                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null && traffic.warningText != null)
                {
                    traffic.warningText.text = "NGUY HIỂM: BẠN ĐANG LẤN LÀN NGƯỢC CHIỀU!";
                    traffic.warningText.color = new Color(1f, 0.5f, 0f); // Màu cam cảnh báo
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerViolating = false;
            violationTimer = 0f;

            TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
            if (traffic != null && traffic.warningText != null)
            {
                traffic.warningText.text = ""; // Xóa chữ khi đã về làn an toàn
            }
        }
    }
}
