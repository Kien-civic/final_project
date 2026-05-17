using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // ĐỒNG BỘ: Thêm dòng này để code hiểu được TextMeshPro

public class AdvancedCarController : MonoBehaviour
{
    public enum GearState { P, R, N, D }
    public int score = 100; // Hoặc public int point = 100;

    [Header("Hộp Số (Gear System)")]
    public GearState currentGear = GearState.P;
    public TextMeshProUGUI gearUIText; // Ô trống để kéo chữ UI vào
    public TextMeshProUGUI scoreText; 
   

    [Header("Thông số xe (Car Settings)")]
    public float motorForce = 7000f;   
    public float brakeForce = 3000f;
    public float maxSteerAngle = 30f;

    [Header("Liên kết bánh xe (Wheel Colliders)")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    private float currentSpeed;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) ShiftGearUp();
        if (Input.GetKeyDown(KeyCode.Q)) ShiftGearDown();

        // Cập nhật chữ hiển thị trên màn hình game
        if (gearUIText != null)
        {
            gearUIText.text = "GEAR: " + currentGear.ToString();
        }
        // Đoạn này ép chữ UI điểm số luôn luôn chạy theo giá trị thực của biến score
        // (Bạn hãy thay 'scoreText' thành ĐÚNG tên biến TextMeshPro hiển thị điểm trên xe của bạn nhé)
        if (scoreText != null)
        {
            scoreText.text = "Điểm: " + score.ToString();
        }
        // --- ĐOẠN CODE KIỂM TRA GAME OVER KHI ĐIỂM VỀ 0 ---
        if (score <= 0)
        {
            score = 0; // Khóa điểm không cho tụt xuống số âm (như -10, -20)

            // Cách 1: Hiện chữ THẤT BẠI to đùng lên màn hình thông báo
            TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
            if (traffic != null && traffic.warningText != null)
            {
                traffic.warningText.text = "GAME OVER: Bạn đã bị trừ hết điểm !";
                traffic.warningText.color = Color.red;
            }

            // Cách 2 (Nâng cao): Gọi màn hình Reset lại Level hoặc hiện Panel Game Over
            // Để đóng băng chiếc xe không cho chạy tiếp khi thua cuộc:
            Time.timeScale = 0f; // Dừng toàn bộ thời gian vật lý trong game lại

            Debug.LogError("GAME OVER: Điểm số đã về 0!");
        }
    }

    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) currentSpeed = rb.linearVelocity.magnitude * 3.6f;

        HandleMotor();
        HandleSteering();
    }

    void HandleMotor()
    {
        float motorInput = 0f;
        float brakeInput = 0f;

        bool isPressingGas = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool isPressingBrake = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        switch (currentGear)
        {
            case GearState.P:
                motorInput = 0f;
                brakeInput = brakeForce;
                break;

            case GearState.N:
                motorInput = 0f;
                if (isPressingBrake) brakeInput = brakeForce;
                break;

            case GearState.D:
                if (isPressingGas) { motorInput = motorForce; brakeInput = 0f; }
                if (isPressingBrake) { motorInput = 0f; brakeInput = brakeForce; }
                break;

            case GearState.R:
                if (isPressingGas) { motorInput = -motorForce; brakeInput = 0f; }
                if (isPressingBrake) { motorInput = 0f; brakeInput = brakeForce; }
                break;
        }

        rearLeftWheel.motorTorque = motorInput;
        rearRightWheel.motorTorque = motorInput;

        frontLeftWheel.brakeTorque = brakeInput;
        frontRightWheel.brakeTorque = brakeInput;
        rearLeftWheel.brakeTorque = brakeInput;
        rearRightWheel.brakeTorque = brakeInput;
    }

    void HandleSteering()
    {
        float steerInput = Input.GetAxis("Horizontal");
        float steerAngle = steerInput * maxSteerAngle;
        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;
    }

    void ShiftGearUp()
    {
        if (currentGear == GearState.P) currentGear = GearState.R;
        else if (currentGear == GearState.R) currentGear = GearState.N;
        else if (currentGear == GearState.N) currentGear = GearState.D;
    }

    void ShiftGearDown()
    {
        if (currentGear == GearState.D) currentGear = GearState.N;
        else if (currentGear == GearState.N) currentGear = GearState.R;
        else if (currentGear == GearState.R) currentGear = GearState.P;
    }
}