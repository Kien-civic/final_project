using UnityEngine;

public class TrafficLightTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Kiểm tra xem "Tổng đài" TrafficSystem có đang bật đèn đỏ hay không
            if (TrafficSystem.Instance != null && TrafficSystem.Instance.currentLight == TrafficSystem.LightColor.Red)
            {
                Debug.LogWarning("-> [CONSOLE] Phát hiện xe vượt đèn đỏ!");

                // 1. Gọi lệnh hiển thị UI thông qua thư viện Singleton tập trung (Chắc chắn lên màn hình 100%)
                TrafficSystem.Instance.ShowNotification("VI PHẠM: Vượt đèn đỏ! Trừ 50 điểm", Color.red);

                // 2. Tiến hành trừ điểm trực tiếp trên xe
                AdvancedCarController carScript = other.GetComponent<AdvancedCarController>();
                if (carScript != null)
                {
                    carScript.score -= 50;
                }
            }
        }
    }
}