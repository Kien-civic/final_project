using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // ĐỒNG BỘ: Thêm dòng này để code hiểu được TextMeshPro

public class AdvancedCarController : MonoBehaviour
{
    public enum GearState { P, R, N, D }
    public int score = 100; // Hoặc public int point = 100;
    private bool isGameOver = false; // Biến kiểm tra xem đã thua chưa

    [Header("Hộp Số (Gear System)")]
    public GearState currentGear = GearState.P;
    public TextMeshProUGUI gearUIText; // Ô trống để kéo chữ UI vào
    public TextMeshProUGUI scoreText;

    [Header("Hệ thống Đích (Finish System)")]
    public TextMeshProUGUI finishUIText; // Ô trống để kéo chữ UI "HOÀN THÀNH LEVEL" vào

    [Header("Giao diện Thua/Thắng")]
    public GameObject restartButtonObject; // Ô trống để kéo thả Nút Restart vào


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
        if (score <= 0 && !isGameOver)
        {
            isGameOver = true; // Đánh dấu đã Game Over để không chạy lại đoạn này nữa
            score = 0;

            // 1. Hiện chữ THẤT BẠI
            TrafficSystem traffic = FindFirstObjectByType<TrafficSystem>();
            if (traffic != null && traffic.warningText != null)
            {
                traffic.warningText.text = "GAME OVER: Bạn đã bị trừ hết điểm!";
                traffic.warningText.color = Color.red;
            }

            // 2. Ép cập nhật lại chữ UI điểm số lần cuối
            if (scoreText != null)
            {
                scoreText.text = "Điểm: 0";
            }

            // 3. BẬT NÚT RESTART LÊN TRƯỚC
            if (restartButtonObject != null)
            {
                restartButtonObject.SetActive(true);
                Debug.Log("Đã gọi lệnh bật nút Restart!");
            }

            // 4. ĐÓNG BĂNG THỜI GIAN SAU CÙNG
            Time.timeScale = 0f;
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
    // HÀM TỰ ĐỘNG CHẠY KHI XE ĐÂM XUYÊN QUA CÁC VÙNG TRIGGER
    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem vật thể xe vừa đâm vào có phải là Đích (Tag: Finish) hay không
        if (other.CompareTag("Finish"))
        {
            Debug.Log("CHÚC MỪNG: Bạn đã về đích thành công!");

            // 1. Hiển thị chữ Hoàn thành lên màn hình chính
            if (finishUIText != null)
            {
                finishUIText.text = "CHÚC MỪNG!\nHOÀN THÀNH LEVEL 3";
                finishUIText.color = Color.green; // Chữ màu xanh lá tươi vui
            }

            // 2. Đóng băng game lại (hoặc bạn có thể cho chuyển cảnh sau 3 giây)
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restart pressed");

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
