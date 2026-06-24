using UnityEngine;

public class SurpriseBrakeEvent : MonoBehaviour
{
    [Header("Brake Settings")]
    public float normalSpeed = 12f;
    public float brakeSpeed = 0f;        // Phanh gấp về 0 (dừng hẳn) hoặc 2-3f (phanh chậm)
    public float deceleration = 5f;      // Độ mượt khi đạp phanh (càng cao phanh càng khựng)

    [Header("Visual Effects (Optional)")]
    public GameObject brakeLights;       // Đèn phanh phía sau xe (nếu có, tự bật khi phanh)

    private float currentSpeed;
    private bool isBraking = false;

    void Start()
    {
        currentSpeed = normalSpeed;
        if (brakeLights != null) brakeLights.SetActive(false);
    }

    void Update()
    {
        // Nếu sự kiện phanh được kích hoạt, giảm tốc độ nhanh chóng về mức brakeSpeed
        if (isBraking)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, brakeSpeed, deceleration * Time.deltaTime);
        }

        // Di chuyển xe về phía trước (theo hướng trục của xe)
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    // HÀM EVENT DRIVEN: Sẽ được gọi từ bên ngoài để kích hoạt phanh
    public void TriggerEmergencyBrake()
    {
        if (!isBraking)
        {
            isBraking = true;
            Debug.LogWarning("EVENT ACTIVATED: Xe phía trước phanh gấp!");
            if (brakeLights != null) brakeLights.SetActive(true); // Bật đèn phanh đỏ rực
        }
    }
}