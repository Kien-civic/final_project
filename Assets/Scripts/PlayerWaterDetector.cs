using UnityEngine;

public class PlayerWaterDetector : MonoBehaviour
{
    [Header("Cấu hình quét nước")]
    public float waterLevelY = -0.5f; // ĐỘ CAO TRỤC Y CỦA MẶT NƯỚC SÔNG (Hãy sửa con số này theo đúng trục Y khối nước của bạn)
    public float checkDistance = 1.5f; // Khoảng cách từ gầm xe xuống nước để tính là bị chìm

    void Update()
    {
        // Cách 1: Kiểm tra trực tiếp tọa độ trục Y của xe
        // Nếu xe mất lái rời khỏi phà và tụt sâu xuống thấp hơn mặt nước sông
        if (transform.position.y < (waterLevelY - 0.2f))
        {
            TriggerWaterFailure("Xe bị chìm sâu dưới mặt nước!");
            return;
        }

        // Cách 2: Bắn một tia Raycast từ tâm xe thẳng xuống dưới gầm
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance))
        {
            // Nếu tia quét trúng vào khối có tên là WaterFailureTrigger hoặc có Layer là Water
            if (hit.collider.gameObject.name == "WaterFailureTrigger" || hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                TriggerWaterFailure("Bánh xe chạm vào vùng nước tử thần!");
            }
        }
    }

    void TriggerWaterFailure(string reason)
    {
        Debug.Log("-> [XỬ PHẠT BACKEND] " + reason);

        // Gọi bộ quản lý màn chơi để xử thua
        RiverLevelManager manager = FindObjectOfType<RiverLevelManager>();
        if (manager != null && manager.score > 0)
        {
            manager.DeductPoints(manager.score, "Lao xuống sông chìm xe");

            // Hủy luôn script này để tránh gọi lệnh trừ điểm liên tục nhiều lần
            this.enabled = false;
        }
    }

    // Vẽ tia quét màu đỏ trong cửa sổ Scene để bạn dễ dàng quan sát bằng mắt
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * checkDistance);
    }
}
