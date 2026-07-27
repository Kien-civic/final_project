using UnityEngine;

public class OppositeLaneZone : MonoBehaviour
{
    public int penaltyPoints = 20;           // Lane encroachment is heavily penalized: 20 points.
    public float timeBeforePenalty = 2f;     // Only allow 2 seconds of violation before penalizing

    private bool isPlayerViolating = false;
    private float violationTimer = 0f;
    private AdvancedCarController playerCar;

    void Update()
    {
        if (isPlayerViolating && playerCar != null)
        {
            violationTimer += Time.deltaTime;

            if (violationTimer >= timeBeforePenalty)
            {
                playerCar.score -= penaltyPoints;
                Debug.LogError($"CẢNH BÁO NGUY HIỂM: Bạn đang đi ngược chiều! Trừ {penaltyPoints} điểm.");

                // Push emergency notifications to the main UI.
                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null && traffic.warningText != null)
                {
                    traffic.warningText.text = $"VI PHẠM: ĐI SAI PHẦN ĐƯỜNG NGƯỢC CHIỀU! -{penaltyPoints}đ";
                    traffic.warningText.color = Color.red;
                }

                violationTimer = 0f; // The penalty will continue every 2 seconds if the driver fails to return to their lane.
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCar = other.GetComponent<AdvancedCarController>();
            if (playerCar != null)
            {
                
                isPlayerViolating = true;
                violationTimer = 0f;

                TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
                if (traffic != null && traffic.warningText != null)
                {
                    traffic.warningText.text = "NGUY HIỂM: BẠN ĐANG LẤN LÀN NGƯỢC CHIỀU!";
                    traffic.warningText.color = new Color(1f, 0.5f, 0f); // Orange warning light
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerViolating = false;
            violationTimer = 0f;

            TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
            if (traffic != null && traffic.warningText != null)
            {
                traffic.warningText.text = ""; // Delete the text once you've returned to the safe lane.
            }
        }
    }
}
