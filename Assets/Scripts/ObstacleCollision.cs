using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("VA CHẠM: Bạn đã đâm vào vỉa hè/chướng ngại vật!");

            AdvancedCarController car = collision.gameObject.GetComponent<AdvancedCarController>();
            if (car != null)
            {
                // Deduct 10 points from the car's physical damage.
                car.score -= 10;

                // Find and call the ShowNotification function to automatically delete text after 3 seconds. ---
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null)
                {
                    string collisionMsg = "VI PHẠM: Va chạm vỉa hè/vật cản! Trừ 10 điểm";
                    traffic.ShowNotification(collisionMsg, Color.yellow); // Yellow warning light
                }
            }
        }
    }
}
