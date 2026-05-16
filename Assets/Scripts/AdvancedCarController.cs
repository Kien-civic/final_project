using UnityEngine;
using UnityEngine.SceneManagement;

public class AdvancedCarController : MonoBehaviour
{
    // Định nghĩa 4 chế độ số P, R, N, D
    public enum GearState { P, R, N, D }

    [Header("Hộp Số (Gear System)")]
    public GearState currentGear = GearState.P; // Mặc định ban đầu là đỗ xe (P)

    [Header("Thông số xe (Car Settings)")]
    public float motorForce = 1500f;   // Lực ga
    public float brakeForce = 3000f;   // Lực phanh
    public float maxSteerAngle = 30f;  // Góc lái tối đa

    [Header("Liên kết bánh xe (Wheel Colliders)")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    private float currentSpeed; // Vận tốc thực tế của xe

    void Update()
    {
        // 1. XỬ LÝ CHUYỂN SỐ BẰNG PHÍM TẮT
        // Nhấn E để tiến số (P -> R -> N -> D)
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShiftGearUp();
        }
        // Nhấn Q để lùi số (D -> N -> R -> P)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ShiftGearDown();
        }
    }

    void FixedUpdate()
    {
        // Tính vận tốc thực tế dựa trên Rigidbody (km/h)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            currentSpeed = rb.linearVelocity.magnitude * 3.6f;
        }

        HandleMotor();
        HandleSteering();
    }

    // Hàm xử lý Ga và Phanh theo chế độ số
    void HandleMotor()
    {
        float motorInput = 0f;
        float brakeInput = 0f;

        // Đọc phím bấm từ bàn phím
        bool isPressingGas = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool isPressingBrake = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        // LOGIC THEO TỪNG CHẾ ĐỘ SỐ
        switch (currentGear)
        {
            case GearState.P:
                // Ở số P: Khóa chặt bánh xe, nhấn ga/phanh đều không chạy
                motorInput = 0f;
                brakeInput = brakeForce; // Tự động cài phanh tay giữ xe đứng im
                break;

            case GearState.N:
                // Ở số N (Số Mo): Xe trôi tự do theo quán tính, nhấn ga không có tác dụng
                motorInput = 0f;
                if (isPressingBrake)
                {
                    brakeInput = brakeForce; // Nhấn S/Xuống để phanh lại
                }
                break;

            case GearState.D:
                // Ở số D (Tiến): W/Lên là Ga tiến, S/Xuống là Phanh chân dừng xe
                if (isPressingGas)
                {
                    motorInput = motorForce;
                    brakeInput = 0f;
                }
                if (isPressingBrake)
                {
                    motorInput = 0f;
                    brakeInput = brakeForce;
                }
                break;

            case GearState.R:
                // Ở số R (Lùi): W/Lên là Ga lùi, S/Xuống là Phanh chân dừng xe
                if (isPressingGas)
                {
                    motorInput = -motorForce; // Lực âm để quay ngược bánh xe
                    brakeInput = 0f;
                }
                if (isPressingBrake)
                {
                    motorInput = 0f;
                    brakeInput = brakeForce;
                }
                break;
        }

        // Áp dụng lực Máy (Motor) vào các bánh sau (hoặc tất cả các bánh)
        rearLeftWheel.motorTorque = motorInput;
        rearRightWheel.motorTorque = motorInput;

        // Áp dụng lực Phanh (Brake) vào tất cả các bánh xe
        frontLeftWheel.brakeTorque = brakeInput;
        frontRightWheel.brakeTorque = brakeInput;
        rearLeftWheel.brakeTorque = brakeInput;
        rearRightWheel.brakeTorque = brakeInput;
    }

    // Hàm xử lý bẻ lái góc cua
    void HandleSteering()
    {
        float steerInput = Input.GetAxis("Horizontal"); // Phím A/D hoặc Mũi tên Trái/Phải
        float steerAngle = steerInput * maxSteerAngle;

        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;
    }

    // Logic tăng số (E)
    void ShiftGearUp()
    {
        if (currentGear == GearState.P) currentGear = GearState.R;
        else if (currentGear == GearState.R) currentGear = GearState.N;
        else if (currentGear == GearState.N) currentGear = GearState.D;
        Debug.Log("Đang ở số: " + currentGear);
    }

    // Logic giảm số (Q)
    void ShiftGearDown()
    {
        if (currentGear == GearState.D) currentGear = GearState.N;
        else if (currentGear == GearState.N) currentGear = GearState.R;
        else if (currentGear == GearState.R) currentGear = GearState.P;
        Debug.Log("Đang ở số: " + currentGear);
    }
}
