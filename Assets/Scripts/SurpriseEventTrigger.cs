using UnityEngine;

public class SurpriseEventTrigger : MonoBehaviour
{
    [Header("Surprise Object Configuration")]
    public GameObject obstacle;       // Drag and drop the hidden obstacle object in the entrance here.
    public Transform stopPosition;    // Drag and drop the GameObject marking the stop position in the middle of the road here.
    public float moveSpeed = 8f;      // Speed at which the obstacle moves out

    private bool isTriggered = false; // Ensure the event is triggered only once

    void OnTriggerEnter(Collider other)
    {
        // Check if the player's vehicle crosses the line to trigger an unexpected situation.
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            Debug.Log("TÌNH HUỐNG KHẨN CẤP: Chướng ngại vật đang lao ra đường!");

            // Calling TrafficSystem will display a yellow warning message on the home screen.
            TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
            if (traffic != null)
            {
                traffic.ShowNotification("CẢNH BÁO BẤT NGỜ: GIẢM TỐC GẤP!", new Color(1f, 0.5f, 0f));
            }

            // Activate the process of moving obstacles out onto the road.
            if (obstacle != null && stopPosition != null)
            {
                StartCoroutine(MoveObstacleOut());
            }
        }
    }

    // Coroutine smoothly moved the obstacle from the alleyway into the middle of the street.
    private System.Collections.IEnumerator MoveObstacleOut()
    {
        // If the obstacle was initially hidden (Deactivated), turn it on.
        obstacle.SetActive(true);

        while (Vector3.Distance(obstacle.transform.position, stopPosition.position) > 0.1f)
        {
            // The obstacle moves forward and stops midway.
            obstacle.transform.position = Vector3.MoveTowards(
                obstacle.transform.position,
                stopPosition.position,
                moveSpeed * Time.deltaTime
            );
            yield return null; // Wait for the next frame.
        }
    }
}
