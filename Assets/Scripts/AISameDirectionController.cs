using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class AISameDirectionController : MonoBehaviour
{
    [Header("Spline Settings")]
    public SplineContainer splineContainer;
    public float laneOffset = -2.4f;

    [Header("Position & Rotation Fix")]
    public float heightOffset = 0.5f;
    public Vector3 rotationOffset = new Vector3(0, 0, 0);

    [Header("Spawn Position")]
    [Range(0f, 1f)]
    public float startProgress = 0f;        // Chọn vị trí xuất phát cho từng xe (0.0 đến 1.0)

    [Header("Brake Event Settings")]
    public float normalSpeed = 12f;
    public float brakeSpeed = 0f;
    public float deceleration = 70f;        // Bạn đang để 70f phanh rất gắt, giữ nguyên nhé!

    [Header("Auto Resume Settings")]
    public float stopDuration = 3f;         // Số giây xe đứng im trước khi tự chạy tiếp

    private float currentSpeed;
    private float progress = 0f;
    private float splineLength;
    private bool isBraking = false;
    private float stopTimer = 0f;           // Biến đếm thời gian dừng

    void Start()
    {
        currentSpeed = normalSpeed;
        if (splineContainer != null)
        {
            splineLength = splineContainer.CalculateLength();
        }

        // ĐỂ MỖI XE CÓ VỊ TRÍ XUẤT PHÁT RIÊNG
        progress = startProgress;
    }

    void Update()
    {
        if (splineContainer == null || splineLength == 0) return;

        // LOGIC XỬ LÝ PHANH VÀ TỰ CHẠY TIẾP
        if (isBraking)
        {
            // Giảm tốc độ về brakeSpeed
            currentSpeed = Mathf.MoveTowards(currentSpeed, brakeSpeed, deceleration * Time.deltaTime);

            // Nếu xe đã giảm tốc xuống bằng hoặc gần bằng tốc độ phanh (đã dừng hẳn)
            if (currentSpeed <= brakeSpeed + 0.1f)
            {
                stopTimer += Time.deltaTime; // Bắt đầu đếm giây

                if (stopTimer >= stopDuration)
                {
                    // ĐÃ HẾT THỜI GIAN CHỜ -> NHẢ PHANH CHẠY TIẾP!
                    isBraking = false;
                    stopTimer = 0f;
                }
            }
        }
        else
        {
            // Nếu không phanh (hoặc vừa nhả phanh), tăng tốc mượt mà trở lại tốc độ bình thường
            currentSpeed = Mathf.MoveTowards(currentSpeed, normalSpeed, deceleration * Time.deltaTime);
        }

        // Tính tiến trình chạy dọc theo đường Spline
        float deltaProgress = (currentSpeed / splineLength) * Time.deltaTime;
        progress += deltaProgress;

        // Logic vòng lặp (Loop): Hết đường thì tự quay lại đầu dốc và reset trạng thái
        if (progress >= 1f)
        {
            progress = 0f;
            isBraking = false;
            stopTimer = 0f;
            currentSpeed = normalSpeed;
        }

        // Tính toán vị trí và hướng xoay bám theo Spline (Giữ nguyên phần fix lỗi của bạn)
        splineContainer.Evaluate(progress, out float3 splinePosition, out float3 forward, out float3 up);
        float3 rightDirection = math.cross(up, forward);
        float3 offsetVector = math.normalize(rightDirection) * laneOffset;

        Vector3 finalPosition = (Vector3)(splinePosition + offsetVector + (up * heightOffset));
        transform.position = finalPosition;

        if (math.any(forward != float3.zero))
        {
            Quaternion baseRotation = Quaternion.LookRotation((Vector3)forward, (Vector3)up);
            transform.rotation = baseRotation * Quaternion.Euler(rotationOffset);
        }
    }

    public void TriggerEmergencyBrake()
    {
        isBraking = true;
        stopTimer = 0f; // Reset lại bộ đếm thời gian mỗi khi bị kích hoạt phanh
        Debug.LogWarning("BẪY KÍCH HOẠT: Xe đang phanh!");
    }
}
