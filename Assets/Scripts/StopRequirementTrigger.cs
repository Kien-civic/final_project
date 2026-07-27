uusing UnityEngine;

public class StopRequirementTrigger : MonoBehaviour
{
    [Header("Challenge Configuration")]
    public float requiredStopTime = 3f; // The required stopping time is 3 seconds.
    public int penaltyPoints = 50;       // Penalty points for violations.
    public string locationName = "Vạch đi bộ"; // Location name to display notifications

    private float stopTimer = 0f;       // Vehicle stopped timer
    private bool isPlayerInside = false; // The vehicle is currently in the Trigger zone.
    private bool hasStoppedEnough = false; // Has the state been paused for 3 seconds yet?
    private AdvancedCarController playerCar; // Store the vehicle's script when it enters.

    void Update()
    {
        // If the vehicle is inside the Trigger Zone and has not yet completed the 3-second stop challenge.
        if (isPlayerInside && playerCar != null && !hasStoppedEnough)
        {
            // Assume in AdvancedCarController you have a speed variable, or calculate it via Rigidbody.
            // Here we take the speed directly from the car's Rigidbody for the most accurate measurement.
            Rigidbody rb = playerCar.GetComponent<Rigidbody>();

            // Check if the vehicle has come to a complete stop (speed is approximately zero).
            if (rb != null && rb.linearVelocity.magnitude < 0.1f)
            {
                stopTimer += Time.deltaTime; // Start accumulating stopping time.
                Debug.Log($"Xe đang dừng tại {locationName}: {stopTimer:F1}s / {requiredStopTime}s");

                // If the vehicle has stopped continuously for the required time
                if (stopTimer >= requiredStopTime)
                {
                    hasStoppedEnough = true;
                    Debug.Log($"Chúc mừng! Đã dừng đủ {requiredStopTime}s tại {locationName}.");

                    // Display a compliment message on the UI if desired (borrowing TrafficSystem's warningText temporarily).
                    TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                    if (traffic != null && traffic.warningText != null)
                    {
                        traffic.warningText.text = "Đạt yêu cầu: Đã dừng đủ 3 giây!";
                        traffic.warningText.color = Color.green;
                    }
                }
            }
            else
            {
                // If the vehicle moves (releases the brake), reset the stop timer (must stop continuously)
                if (stopTimer > 0f && !hasStoppedEnough)
                {
                    stopTimer = 0f;
                    Debug.Log("Xe di chuyển! Bộ đếm thời gian dừng đã bị reset.");
                }
            }
        }
    }

    // When the car begins to enter the stop line
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCar = other.GetComponent<AdvancedCarController>();
            if (playerCar != null)
            {
                isPlayerInside = true;
                stopTimer = 0f;
                hasStoppedEnough = false;
                Debug.Log($"Đi vào vùng yêu cầu dừng: {locationName}. Hãy dừng xe 3 giây!");
            }
        }
    }

    // When the vehicle moves out of the stop line (crosses the line)
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // If you leave the designated area WITHOUT stopping for at least 3 seconds -> FINE
            if (!hasStoppedEnough)
            {
                Debug.LogWarning($"VI PHẠM: Chưa dừng đủ 3s tại {locationName}!");

                // Deduct 50 points from the vehicle.
                if (playerCar != null)
                {
                    playerCar.score -= penaltyPoints;
                }

                // Display the penalty message in bright red on the home screen.
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null && traffic.warningText != null)
                {
                    traffic.warningText.text = $"VI PHẠM: Không dừng đủ 3s tại {locationName}! Trừ {penaltyPoints} điểm";
                    traffic.warningText.color = Color.red;
                }
            }

            // Reset the status once the vehicle has completely passed.
            isPlayerInside = false;
            playerCar = null;
        }
    }
}

