using UnityEngine;

public class UrbanSpeedZone : MonoBehaviour
{
    [Header("Urban Speed Zone Configuration")]
    public float maxSpeedLimit = 50f;        // The maximum speed limit is 50 km/h.
    public int penaltyPoints = 15;          // Penalty points for each violation (e.g., 15 points)
    public float penaltyCheckRate = 2f;     // Every 2 seconds of speeding will result in another penalty

    private bool isPlayerInZone = false;
    private float penaltyTimer = 0f;
    private AdvancedCarController playerCar;

    [Header("System Links")]
    public TrafficSystem trafficSystem; // Create a dropdown menu outside the Inspector.

    // Use flags to prevent the UI from being called repeatedly every frame.
    private bool hasShownOverSpeedWarning = false;

    void Update()
    {
        // Calculations are only performed when the player's vehicle is located within a residential/industrial zone.
        if (isPlayerInZone && playerCar != null)
        {
            // Get the current speed of the vehicle (measured in km/h from the Rigidbody).
            float currentSpeedKMH = playerCar.GetComponent<Rigidbody>().linearVelocity.magnitude * 3.6f;

            if (currentSpeedKMH > maxSpeedLimit)
            {
                // Increase the penalty timer fairly and transparently
                penaltyTimer += Time.deltaTime;

                // ONLY CALL UI ONCE WHEN SPEEDING
                if (!hasShownOverSpeedWarning)
                {
                if (trafficSystem != null)
{
    trafficSystem.ShowNotification("VÀO KHU DÂN CƯ: GIỚI HẠN 50 KM/H!", new Color(1f, 0.6f, 0f));
}
                    hasShownOverSpeedWarning = true; // Mark that the warning has been shown, so it won't be called again in the next frame
                }

                // If speeding continues for the entire check duration (2 seconds)
                if (penaltyTimer >= penaltyCheckRate)
                {
                    playerCar.score -= penaltyPoints;
                    Debug.LogWarning($"VI PHẠM: Chạy quá tốc độ trong khu dân cư! Trừ {penaltyPoints} điểm. Tốc độ: {currentSpeedKMH.ToString("F0")} km/h");

                    TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                    if (traffic != null)
                    {
                        traffic.ShowNotification($"VI PHẠM: PHÓNG NHANH QUA NGÃ TƯ! ({currentSpeedKMH.ToString("F0")}/30 km/h)", Color.red);
                    }

                    penaltyTimer = 0f; // Reset the timer so that if speeding continues, the penalty will be applied again after 2 seconds.
                }
            }
            else
            {
                // If the player has proactively reduced speed below 50 km/h safely
                if (hasShownOverSpeedWarning)
                {
                    penaltyTimer = 0f;
                    hasShownOverSpeedWarning = false; // Reset the flag to its normal state

                    TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                    if (traffic != null)
                    {
                        traffic.ShowNotification("TỐC ĐỘ HỢP LỆ", Color.green);
                    }
                }
            }
        }
    }

    // When the vehicle enters the residential area (Sign R.420)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCar = other.GetComponent<AdvancedCarController>();
            if (playerCar != null)
            {
                isPlayerInZone = true;
                penaltyTimer = 0f;
                hasShownOverSpeedWarning = false; // Reset the flag when entering a new zone
                Debug.Log("Đã đi vào khu vực đông dân cư! Tốc độ tối đa giới hạn 50 km/h.");

                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null)
                {
                    traffic.ShowNotification("SẮP ĐẾN NGÃ TƯ: HÃY GIẢM TỐC DƯỚI 30 KM/H!", new Color(1f, 0.6f, 0f)); // Orange warning light
                }
            }
        }
    }

    // When the vehicle passes the sign indicating the end of the residential area (Sign R.421)
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            penaltyTimer = 0f;
            hasShownOverSpeedWarning = false;
            Debug.Log("Đã hết khu vực đông dân cư. Tốc độ trở lại bình thường.");

            TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
            if (traffic != null)
            {
                traffic.ShowNotification("ĐÃ QUA NGÃ TƯ AN TOÀN", Color.green);
            }
        }
    }
}

