using UnityEngine;

public class SurpriseEventTrigger : MonoBehaviour
{
    [Header("Cấu hình vật thể bất ngờ")]
    public GameObject obstacle;       // Kéo thả Object vật cản ẩn trong ngõ vào đây
    public Transform stopPosition;    // Kéo thả GameObject mốc vị trí dừng giữa đường vào đây
    public float moveSpeed = 8f;      // Tốc độ lao ra của vật cản

    private bool isTriggered = false; // Đảm bảo sự kiện chỉ kích hoạt 1 lần duy nhất

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu xe người chơi đi qua vạch kích hoạt tình huống bất ngờ
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            Debug.Log("TÌNH HUỐNG KHẨN CẤP: Chướng ngại vật đang lao ra đường!");

            // Gọi TrafficSystem bắn chữ cảnh báo màu vàng lên màn hình chính
            TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
            if (traffic != null)
            {
                traffic.ShowNotification("CẢNH BÁO BẤT NGỜ: GIẢM TỐC GẤP!", new Color(1f, 0.5f, 0f));
            }

            // Kích hoạt tiến trình di chuyển vật cản lao ra đường
            if (obstacle != null && stopPosition != null)
            {
                StartCoroutine(MoveObstacleOut());
            }
        }
    }

    // Coroutine di chuyển vật cản mượt mà từ trong ngõ ra giữa đường
    private System.Collections.IEnumerator MoveObstacleOut()
    {
        // Nếu ban đầu vật cản đang bị ẩn (Deactivate), bật nó lên
        obstacle.SetActive(true);

        while (Vector3.Distance(obstacle.transform.position, stopPosition.position) > 0.1f)
        {
            // Tịnh tiến vật cản lao ra điểm dừng giữa đường
            obstacle.transform.position = Vector3.MoveTowards(
                obstacle.transform.position,
                stopPosition.position,
                moveSpeed * Time.deltaTime
            );
            yield return null; // Chờ frame tiếp theo
        }
    }
}
