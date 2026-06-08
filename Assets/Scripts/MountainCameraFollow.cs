using UnityEngine;

public class MountainCameraFollow : MonoBehaviour
{
    [Header("Đối tượng bám đuôi")]
    public Transform target;           // Kéo thả Playcar vào đây

    [Header("Cấu hình Khoảng cách cơ bản")]
    public float defaultArmLength = 6f; // Chiều dài cánh tay đòn mặc định khi đi đường thẳng
    public float minArmLength = 3.5f;   // Thu ngắn tối đa khi cua gấp hoặc lùi sát vách núi
    public float maxArmLength = 8f;     // Kéo dài tối đa khi xe lao dốc nhanh để nhìn bao quát
    public float heightOffset = 2.5f;   // Độ cao mặc định của camera so với xe

    [Header("Tốc độ phản hồi (Mượt mà)")]
    public float movementSmooth = 5f;   // Tốc độ đuổi theo của camera
    public float rotationSmooth = 5f;   // Tốc độ xoay góc nhìn theo đuôi xe
    public float zoomSmooth = 3f;       // Tốc độ co giãn cánh tay đòn (TargetArmLength)

    [Header("Tính năng thông minh cho Đường Đèo")]
    [Tooltip("Tự động nâng camera lên cao hơn khi xe đang leo dốc cao")]
    public bool autoHeightOnSlopes = true;

    private float currentArmLength;
    private Rigidbody targetRigidbody;

    void Start()
    {
        currentArmLength = defaultArmLength;
        if (target != null)
        {
            targetRigidbody = target.GetComponent<Rigidbody>();
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. TỰ ĐỘNG ĐIỀU CHỈNH TARGET ARM LENGTH LINH HOẠT
        float desiredArmLength = defaultArmLength;

        if (targetRigidbody != null)
        {
            // Lấy vận tốc thực tế của xe
            float speed = targetRigidbody.linearVelocity.magnitude * 3.6f; // km/h

            // Lấy góc quay/lái hiện tại của xe (Nếu xe đang bẻ lái gắt, speed quay Y sẽ cao)
            float turnSpeed = Mathf.Abs(targetRigidbody.angularVelocity.y);

            // LOGIC CUA GẤP: Khi cua gắt (turnSpeed lớn), thu ngắn Arm Length để camera áp sát, nhìn rõ góc cua
            if (turnSpeed > 0.3f)
            {
                desiredArmLength = Mathf.Lerp(defaultArmLength, minArmLength, turnSpeed * 0.5f);
            }
            // LOGIC TỐC ĐỘ: Khi phóng nhanh trên đường dốc thẳng, kéo dài Arm Length để nhìn xa hơn
            else if (speed > 40f)
            {
                desiredArmLength = Mathf.Lerp(defaultArmLength, maxArmLength, (speed - 40f) / 60f);
            }
        }

        // Nội suy mượt mà chiều dài cánh tay đòn hiện tại
        currentArmLength = Mathf.Lerp(currentArmLength, desiredArmLength, Time.deltaTime * zoomSmooth);


        // 2. TỰ ĐỘNG TÍNH TOÁN ĐỘ CAO THEO ĐỘ DỐC (Y THAY ĐỔI)
        float currentHeightOffset = heightOffset;

        if (autoHeightOnSlopes)
        {
            // Kiểm tra hướng tiến của xe xem có đang chúi lên (leo dốc) hay chúi xuống không
            float pitchAngle = target.eulerAngles.x;
            // Chuẩn hóa góc về khoảng -180 đến 180
            if (pitchAngle > 180) pitchAngle -= 360;

            // Nếu xe đang leo dốc (pitchAngle < 0), nâng camera cao lên để không bị mặt đường dốc che khuất tầm nhìn phía trước
            if (pitchAngle < -5f)
            {
                currentHeightOffset += Mathf.Abs(pitchAngle) * 0.08f;
            }
        }


        // 3. VỊ TRÍ VÀ GÓC XOAY THEO ĐUÔI XE
        // Tính toán hướng nhìn từ sau đuôi xe dựa trên góc xoay trục Y của xe
        Quaternion targetRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);

        // Vị trí mục tiêu mà Camera muốn vươn tới (Lùi về sau một khoảng currentArmLength và nhấc cao lên currentHeightOffset)
        Vector3 targetPosition = target.position - (targetRotation * Vector3.forward * currentArmLength) + (Vector3.up * currentHeightOffset);

        // Di chuyển và xoay Camera mượt mà bằng Lerp / Slerp
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSmooth);

        // Luôn luôn bẻ góc camera nhìn thẳng vào trọng tâm chiếc xe
        Vector3 lookAtPos = target.position + Vector3.up * 1f; // Nhìn vào thắt lưng xe
        Quaternion lookRotation = Quaternion.LookRotation(lookAtPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSmooth);
    }
}
