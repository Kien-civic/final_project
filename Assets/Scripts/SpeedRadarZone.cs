using UnityEngine;

public class SpeedRadarZone : MonoBehaviour
{
    private AdvancedCarController playerCar;
    private Rigidbody carRigidbody;
    private bool isPlayerOnHighway = false;
    private float penaltyTimer = 0f;

    [Header("System Linking")]
    public TrafficSystem trafficSystem;

    // ADD THIS FLAG VARIABLE TO PREVENT TEXT REPETITION
    private bool isTextDisplayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCar = other.GetComponent<AdvancedCarController>();
            carRigidbody = other.GetComponent<Rigidbody>();

            if (playerCar != null && carRigidbody != null)
            {
                isPlayerOnHighway = true;
                penaltyTimer = 0f; // Give the player time to prepare (e.g., 3 seconds).
                isTextDisplayed = false; // Reset the flag when entering a new area.

                if (trafficSystem != null)
                {
                    // Only notify you once you've just entered the zone.
                    trafficSystem.ShowNotification("ĐÃ VÀO CAO TỐC! TỐC ĐỘ: 60 - 100 KM/H", new Color(1f, 0.6f, 0f));
                }
                Debug.Log("Đã vào đoạn đường cao tốc! Giới hạn: 60 - 100 km/h.");
            }
        }
    }

    void Update()
    {
        if (isPlayerOnHighway && carRigidbody != null && playerCar != null)
        {
            // Calculate the current speed of the vehicle (km/h).
            float currentSpeedKMH = carRigidbody.linearVelocity.magnitude * 3.6f;

            // Calculate the initial penalty delay (if there is a 3-second countdown logic)
            penaltyTimer += Time.deltaTime;

            if (penaltyTimer >= 3f) // After 3 seconds of preparation, start checking for violations
            {
                // CHECK FOR VIOLATIONS: Exceeding the maximum speed or falling below the minimum speed
                if (currentSpeedKMH > 100f || currentSpeedKMH < 60f)
                {
                    // IMPORTANT: Only display the text if it hasn't been shown yet
                    if (!isTextDisplayed && trafficSystem != null)
                    {
                        isTextDisplayed = true; // Khóa lệnh lại ngay lập tức!

                        string errorMsg = currentSpeedKMH > 100f ?
                            $"VI PHẠM: QUÁ TỐC ĐỘ CAO TỐC! ({currentSpeedKMH.ToString("F0")}/100 km/h)" :
                            $"VI PHẠM: TỐC ĐỘ DƯỚI MỨC TỐI THIỂU! ({currentSpeedKMH.ToString("F0")}/60 km/h)";

                        trafficSystem.ShowNotification(errorMsg, Color.red);

                        // Deduct points from the player (e.g., deduct 10 administrative points)
                        playerCar.score -= 10;
                    }
                }
                else
                {
                    // If the vehicle has adjusted its speed back to the safe range (60 - 100 km/h)
                    if (isTextDisplayed)
                    {
                        isTextDisplayed = false; // Unlock the flag so that violations can be detected again if repeated
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnHighway = false;
            playerCar = null;
            carRigidbody = null;
            isTextDisplayed = false; // Reset the flag when exiting the area

            if (trafficSystem != null)
            {
                trafficSystem.ShowNotification("RỜI CAO TỐC AN TOÀN", Color.green);
            }
            Debug.Log("Vừa rời khỏi đoạn đường cao tốc.");
        }
    }
}
