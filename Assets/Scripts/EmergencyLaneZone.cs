using UnityEngine;

public class EmergencyLaneZone : MonoBehaviour
{
    [Header("Penalty Configuration")]
    public int penaltyPoints = 10;           // Points deducted each time
    public float timeBeforePenalty = 3f;     // Time limit allowed (3 seconds)
    public float penaltyRepeatRate = 3f;     // If you stubbornly continue running, you will be penalized every 3 seconds.

    private bool isPlayerInLane = false;
    private float laneTimer = 0f;
    private AdvancedCarController playerCar;

    void Update()
    {
        // Calculations are only performed when the player's vehicle is in the emergency lane.
        if (isPlayerInLane && playerCar != null)
        {
            // Increase the timer in real time (seconds)
            laneTimer += Time.deltaTime;

            // Check if the 3-second time limit has been exceeded.
            if (laneTimer >= timeBeforePenalty)
            {
                // Implement point deduction.
                playerCar.score -= penaltyPoints;

                Debug.LogWarning($"VI PHẠM: Chạy vào làn khẩn cấp quá thời gian cho phép! Trừ {penaltyPoints} điểm.");

                // Display a red warning message on the home screen.
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null && traffic.warningText != null)
                {
                    traffic.warningText.text = $"VI PHẠM: KHÔNG CHẠY VÀO LÀN KHẨN CẤP! -{penaltyPoints}đ";
                    traffic.warningText.color = Color.red;
                }

                // Reset the timer to 0 so that if they continue to drive here intentionally, they will be penalized again every 3 seconds.
                laneTimer = 0f;

                // Tip: Change timeBeforePenalty to penaltyRepeatRate for subsequent penalties if you want to space out the time.
            }
        }
    }

    // When the wheels or body of the vehicle touch the Trigger strip of the emergency lane.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCar = other.GetComponent<AdvancedCarController>();
            if (playerCar != null)
            {
                isPlayerInLane = true;
                laneTimer = 0f; // Reset the timer to 0 as soon as the wheel touches the lane.
                Debug.Log("Cảnh báo: Bạn vừa đi vào làn khẩn cấp! Hãy đưa xe trở lại làn chính trong vòng 3 giây.");

                // A yellow warning light is now displayed to give the player time to steer away.
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null && traffic.warningText != null)
                {
                    traffic.warningText.text = "CẢNH BÁO: RỜI KHỎI LÀN KHẨN CẤP NGAY!";
                    traffic.warningText.color = Color.yellow;
                }
            }
        }
    }

    // Once the player has successfully steered the car back into the main lane.
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInLane = false;
            laneTimer = 0f; // Clear the counter to avoid hidden penalties.
            Debug.Log("Đã an toàn quay trở lại làn đường chính.");

            // Remove the warning text or change it to normal notification text.
            TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
            if (traffic != null && traffic.warningText != null)
            {
                traffic.warningText.text = ""; // Hide the text when you've followed the rules.
            }
        }
    }
}
