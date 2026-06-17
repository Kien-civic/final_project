using UnityEngine;
using UnityEngine.Splines; // Bắt buộc phải có thư viện Spline của Unity 6
using Unity.Mathematics;

public class AIReverseController : MonoBehaviour
{
    [Header("Spline Settings")]
    public SplineContainer splineContainer; // Kéo dải Spline (1) vào đây
    public float speed = 10f;               // Tốc độ chạy của xe AI
    public float laneOffset = 2.4f;         // Khoảng cách dạt sang làn xanh (trục X)
    public float heightOffset = 0.5f; // Độ cao nâng xe lên khỏi mặt đường (mét)
    public Vector3 rotationOffset = new Vector3(0, 0, 0); // Góc bù trừ cho đầu xe

    private float progress = 1f;            // Bắt đầu từ cuối đường (1.0 = 100%)
    private float splineLength;

    [Header("Spawn Position")]
    [Range(0f, 1f)]
    public float startProgress = 1f; // Vị trí xuất phát riêng (Từ 0.0 đến 1.0)

    void Start()
    {
        if (splineContainer != null)
        {
            // Tính toán tổng chiều dài của dải Spline để quy đổi vận tốc mét/giây
            splineLength = splineContainer.CalculateLength();
        }
        progress = startProgress;
    }

    void Update()
    {
        if (splineContainer == null || splineLength == 0) return;

        // 1. LOGIC CHẠY NGƯỢC: Trừ dần progress theo thời gian để xe đi lùi về 0
        float deltaProgress = (speed / splineLength) * Time.deltaTime;
        progress -= deltaProgress;

        // Nếu xe chạy về đến đầu đường (0), cho nó reset quay lại cuối đường (1) để chạy tiếp
        if (progress <= 0f)
        {
            progress = 1f;
        }

        // 2. Tính toán vị trí gốc và hướng xoay tại điểm hiện tại trên Spline
        // Sử dụng hàm Evaluate để lấy Tọa độ (position), Hướng tiến (forward), Hướng thiên đỉnh (up)
        splineContainer.Evaluate(progress, out float3 splinePosition, out float3 forward, out float3 up);

        // 3. LOGIC LÀN ĐƯỜNG NGƯỢC CHIỀU: Tính Vector dịch chuyển sang làn bên phải
        // Dùng tích có hướng (Cross Product) giữa hướng tiến và hướng đỉnh để tìm ra Vector chỉ sang bên phải đường
        float3 rightDirection = math.cross(up, forward);
        float3 offsetVector = math.normalize(rightDirection) * laneOffset;

        // Thêm up * heightOffset để nhấc bổng xe lên theo phương vuông góc với mặt đường
        Vector3 finalPosition = (Vector3)(splinePosition + offsetVector + (up * heightOffset));

        // 4. Cập nhật vị trí và hướng xoay cho xe AI
        transform.position = finalPosition;

        // Vì xe chạy ngược chiều Spline, hướng nhìn của xe phải quay ngược 180 độ
        Vector3 lookDirection = -(Vector3)forward;
        if (lookDirection != Vector3.zero)
        {
            // 1. Tính hướng xoay chuẩn theo đường đèo trước
            Quaternion baseRotation = Quaternion.LookRotation(lookDirection, (Vector3)up);

            // 2. Nhân thêm góc bù (rotationOffset) để nắn đầu xe về đúng làn chạy
            transform.rotation = baseRotation * Quaternion.Euler(rotationOffset);
        }
    }
}