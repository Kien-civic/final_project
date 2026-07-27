using UnityEngine;

public class TrafficLightTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if the "TrafficSystem" call center is showing a red light.
            if (TrafficSystem.Instance != null && TrafficSystem.Instance.currentLight == TrafficSystem.LightColor.Red)
            {
                Debug.LogWarning("-> [CONSOLE] Phát hiện xe vượt đèn đỏ!");

                // 1. Call the UI display command using the centralized Singleton library (Guaranteed to display on screen 100%)
                TrafficSystem.Instance.ShowNotification("VI PHẠM: Vượt đèn đỏ! Trừ 50 điểm", Color.red);

                // 2. Deduct points directly from the vehicle.
                AdvancedCarController carScript = other.GetComponent<AdvancedCarController>();
                if (carScript != null)
                {
                    carScript.score -= 50;
                }
            }
        }
    }
}
