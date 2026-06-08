using UnityEngine;
using UnityEngine.Splines; // Bắt buộc phải có thư viện này

public class AICarFollowSpline : MonoBehaviour
{
    public SplineContainer splineContainer; // Kéo thả Object đường đi vào đây
    public float speed = 15f;               // Tốc độ xe AI (m/s) ~ 54 km/h

    [Range(0f, 1f)]
    public float startingProgress = 0f;

    private float progress = 0f;
    private float splineLength = 0f;

    void Start()
    {
        if (splineContainer != null)
        {
            // Tính toán tổng chiều dài quãng đường Spline
            splineLength = splineContainer.CalculateLength();
            progress = startingProgress;
        }
    }

    [Header("Cấu hình Độ cao xe")]
    [Tooltip("Độ cao nhích lên để bánh xe chạm đúng mặt đất, tránh bị chìm (Ví dụ: 0.5, 1, 1.2...)")]
    public float heightOffset = 0.5f;

    void Update()
    {
        if (splineContainer == null || splineLength == 0) return;

        // Tính toán tiến trình (0 to 1)
        progress += (speed * Time.deltaTime) / splineLength;
        if (progress > 1f) progress = 0f; // Lặp lại đường đi

        // --- ĐOẠN CODE CẬP NHẬT ĐỂ ĐẨY XE LÊN MẶT ĐẤT ---

        // 1. Lấy vị trí gốc trên đường Spline (nằm sát mặt đất)
        Vector3 splinePosition = (Vector3)splineContainer.EvaluatePosition(progress);

        // 2. CỘNG THÊM độ cao bù trừ vào trục Y trước khi gán cho xe
        splinePosition.y += heightOffset;
        transform.position = splinePosition;

        // 3. Lấy hướng tiếp tuyến (hướng đi của Spline)
        Vector3 tangent = (Vector3)splineContainer.EvaluateTangent(progress);

        // 4. Nếu có hướng đi, quay xe nhìn theo hướng đó
        if (tangent != Vector3.zero)
        {
            // Dùng Quaternion.LookRotation để tạo góc xoay nhìn về hướng đi
            transform.rotation = Quaternion.LookRotation(tangent);

            // MẸO: Nếu xe vẫn bị quay ngang, bạn có thể cần bù một góc 90 độ
            // transform.rotation = Quaternion.LookRotation(tangent) * Quaternion.Euler(0, 90, 0); 
            transform.rotation = Quaternion.LookRotation(tangent) * Quaternion.Euler(0, -90, 0);
        }
    }
}
