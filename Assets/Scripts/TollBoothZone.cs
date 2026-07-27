using UnityEngine;

public class TollBoothZone : MonoBehaviour
{
    [Header("Toll Booth Configuration")]
    public string boothName = "Trạm thu phí vào";
    public float speedLimitInsideBooth = 30f; // Maximum speed when passing the station (30 km/h)
    public int penaltyPoints = 30;           // Penalty points for speeding through the booth

    private bool isPlayerInBooth = false;
    private AdvancedCarController playerCar;
    private Rigidbody carRigidbody;
    private bool hasBeenPenalized = false; // Prevent continuous penalties in a single pass

    void Update()
    {
        if (isPlayerInBooth && playerCar != null && carRigidbody != null && !hasBeenPenalized)
        {
            // Calculate the actual speed (km/h)
            float currentSpeedKMH = carRigidbody.linearVelocity.magnitude * 3.6f;

            // If a vehicle exceeds 30 km/h within the toll booth.
            if (currentSpeedKMH > speedLimitInsideBooth)
            {
                hasBeenPenalized = true; // A single penalty as a warning.
                playerCar.score -= penaltyPoints;

                Debug.LogWarning($"VI PHẠM: Phóng nhanh qua {boothName}! Tốc độ: {currentSpeedKMH:F0} km/h");

                // Call the ShowNotification function to trigger a 3-second countdown.
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null)
                {
                    string errorMsg = $"VI PHẠM: Giảm tốc độ dưới {speedLimitInsideBooth}km/h khi qua {boothName}! -{penaltyPoints}đ";
                    traffic.ShowNotification(errorMsg, Color.red);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCar = other.GetComponent<AdvancedCarController>();
            carRigidbody = other.GetComponent<Rigidbody>();

            if (playerCar != null && carRigidbody != null)
            {
                isPlayerInBooth = true;
                hasBeenPenalized = false; // Reset penalty status for this pass
                Debug.Log($"Bạn đang đi vào: {boothName}. Hãy giảm tốc độ dưới {speedLimitInsideBooth} km/h!");

                // Call the ShowNotification function to make the text disappear after 3 seconds.
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null)
                {
                    string warningMsg = $"SẮP TỚI {boothName.ToUpper()}! GIẢM TỐC ĐỘ < {speedLimitInsideBooth} KM/H";
                    traffic.ShowNotification(warningMsg, Color.yellow);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInBooth = false;
            playerCar = null;
            carRigidbody = null;
            Debug.Log($"Đã ra khỏi: {boothName}");

            // You can optionally display a message indicating that you have left the safe zone.
            TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
            if (traffic != null)
            {
                traffic.ShowNotification($"ĐÃ QUA {boothName.ToUpper()} AN TOÀN", Color.green);
            }
        }
    }
}
