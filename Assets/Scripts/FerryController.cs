using UnityEngine;

public class FerryController : MonoBehaviour
{
    [Header("Cấu hình hành trình phà")]
    public Transform bến_A;       // Kéo thả BenA từ Hierarchy vào đây
    public Transform bến_B;       // Kéo thả BenB từ Hierarchy vào đây
    public float speed = 3.5f;    // Tốc độ phà chạy qua sông

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        if (bến_A != null) transform.position = bến_A.position;
        targetPosition = bến_B.position;
    }

    void Update()
    {
        // Khi phà được kích hoạt chạy sang sông
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
    }

    // Khi bộ phận của xe chạm vào khối tàng hình AttachTrigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            Debug.Log("-> XE ĐÃ LÊN PHÀ THÀNH CÔNG!");

            // Khóa xe làm con của phà để xe tịnh tiến theo hành trình phà
            other.transform.root.SetParent(transform);

            // Sau 1 giây phà sẽ tự động nhổ neo chạy sang sông
            Invoke("StartFerry", 1f);
        }
    }

    // Khi người chơi cố tình nhấn ga lái xe rời khỏi phạm vi an toàn của phà giữa dòng sông
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            Debug.Log("-> CẢNH BÁO: Xe đã tự ý di chuyển rời khỏi phà!");

            // Giải phóng xe độc lập hoàn toàn khỏi phà để trọng lực tự nhiên xử lý rơi tự do
            other.transform.root.SetParent(null);

            // Đảo mục tiêu bến để phà sẵn sàng cho lượt quay đầu sau này
            if (!isMoving)
            {
                targetPosition = (targetPosition == bến_B.position) ? bến_A.position : bến_B.position;
            }
        }
    }

    void StartFerry()
    {
        isMoving = true;
    }
}