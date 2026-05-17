using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    // Liên kết tới hệ thống quản lý đèn giao thông chính
    public TrafficSystem trafficSystem;

    void OnTriggerEnter(Collider other)
    {
        if (trafficSystem != null)
        {
            // Gọi hàm kiểm tra xem có bị phạt vượt đèn đỏ không
            trafficSystem.CheckVehicleViolation(other);
        }
    }
}
