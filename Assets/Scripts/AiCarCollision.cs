using UnityEngine;

public class AICarCollision : MonoBehaviour
{
    [Header("Collision penalty configuration")]
    public int penaltyPoints = 50; // The penalty points for a collision are 50 points.

    // This function is automatically activated when there is a physical collision between two Collider blocks.
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object that crashed into this AI vehicle is a player's vehicle.
        if (collision.gameObject.CompareTag("Player"))
        {
            // 1. Find the player's vehicle control script to deduct points.
            AdvancedCarController playerCar = collision.gameObject.GetComponent<AdvancedCarController>();

            if (playerCar != null)
            {
                // Deduct 50 points from the vehicle's data.
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
