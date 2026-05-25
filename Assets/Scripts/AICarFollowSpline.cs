using UnityEngine;
using UnityEngine.Splines; // Bắt buộc phải có thư viện này

public class AICarFollowSpline : MonoBehaviour
{
    public SplineContainer splineContainer; // Kéo thả Object đường đi vào đây
    public float speed = 15f;               // Tốc độ xe AI (m/s) ~ 54 km/h

    private float progress = 0f;
    private float splineLength = 0f;

    void Start()
    {
        if (splineContainer != null)
        {
            // Tính toán tổng chiều dài quãng đường Spline
            splineLength = splineContainer.CalculateLength();
        }
    }

    void Update()
    {
        if (splineContainer == null || splineLength == 0) return;

        // Tăng tiến trình di chuyển dựa trên tốc độ và thời gian
        progress += (speed * Time.deltaTime) / splineLength;

        // Nếu xe chạy hết đường thì reset về đầu đường (Tạo luồng giao thông vô tận)
        if (progress > 1f) progress = 0f;

        // Cập nhật vị trí và góc xoay của xe AI bám theo đường cong Spline
        transform.position = (Vector3)splineContainer.EvaluatePosition(progress);
        transform.forward = (Vector3)splineContainer.EvaluateTangent(progress);
    }
}
