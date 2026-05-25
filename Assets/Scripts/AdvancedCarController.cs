using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AdvancedCarController : MonoBehaviour
{
    public enum GearState { P, R, N, D }

    [Header("Gameplay")]
    public int score = 100;
    private bool isGameOver = false;
    public TMP_Text speedText;
    private Vector3 lastPosition;

    [Header("Hộp Số (Gear System)")]
    public GearState currentGear = GearState.P;
    public TextMeshProUGUI gearUIText;
    public TextMeshProUGUI scoreText;

    [Header("Hệ thống Đích (Finish System)")]
    public TextMeshProUGUI finishUIText;

    [Header("Giao diện Thua/Thắng")]
    public GameObject restartButtonObject;

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
    private Rigidbody carRigidbody;

    void Start()
    {
        lastPosition = transform.position;
        carRigidbody = GetComponent<Rigidbody>();
        if (restartButtonObject != null)
            restartButtonObject.SetActive(false);

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) ShiftGearUp();
        if (Input.GetKeyDown(KeyCode.Q)) ShiftGearDown();

        if (gearUIText != null)
            gearUIText.text = "GEAR: " + currentGear.ToString();

        if (scoreText != null)
            scoreText.text = "Điểm: " + score.ToString();

        if (score <= 0 && !isGameOver)
        {
            isGameOver = true;
            score = 0;

            TrafficSystem traffic = FindObjectOfType<TrafficSystem>();
            if (traffic != null && traffic.warningText != null)
            {
                traffic.warningText.text = "GAME OVER: Bạn đã bị trừ hết điểm!";
                traffic.warningText.color = Color.red;
            }

            if (scoreText != null)
                scoreText.text = "Điểm: 0";

            if (restartButtonObject != null)
            {
                restartButtonObject.SetActive(true);
                Debug.Log("Đã gọi lệnh bật nút Restart!");
            }

            Time.timeScale = 0f;
        }

        // Speed display (meters moved since last frame -> m/s -> km/h)
        if (speedText != null)
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            float speedMS = (Time.deltaTime > 0f) ? (distanceMoved / Time.deltaTime) : 0f;
            float currentSpeedKMH = speedMS * 3.6f;

            if (Time.timeScale == 0f || currentSpeedKMH < 0.5f)
                currentSpeedKMH = 0f;

            speedText.text = "Speed: " + currentSpeedKMH.ToString("F0") + " km/h";
            lastPosition = transform.position;
        }
    }

    void FixedUpdate()
    {
        Rigidbody rb = carRigidbody != null ? carRigidbody : GetComponent<Rigidbody>();
        if (rb != null) currentSpeed = rb.linearVelocity.magnitude * 3.6f;

        HandleMotor();
        HandleSteering();
    }

    void HandleMotor()
    {
        // Basic gear-based motor/brake decision, then refined with input
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

        // More refined input-based motor/brake handling
        float gasPedal = Input.GetAxis("Vertical"); // -1..1
        Vector3 rbVelocity = carRigidbody != null ? carRigidbody.linearVelocity : Vector3.zero;
        float forwardSpeed = Vector3.Dot(rbVelocity, transform.forward);

        float motor = 0f;
        float brake = 0f;

        if (Mathf.Abs(gasPedal) < 0.05f)
        {
            motor = 0f;
            brake = 500f; // small hold brake to prevent roll
        }
        else if (gasPedal > 0f)
        {
            motor = gasPedal * motorForce;
            brake = 0f;
        }
        else // gasPedal < 0
        {
            if (forwardSpeed > 0.5f)
            {
                motor = 0f;
                brake = brakeForce; // emergency braking when pressing S while moving forward
            }
            else
            {
                motor = gasPedal * motorForce; // reverse torque
                brake = 0f;
            }
        }

        // Combine gear decisions and refined decisions: prefer refined values when non-zero
        float appliedMotor = (Mathf.Abs(motor) > 0f) ? motor : motorInput;
        float appliedBrake = (Mathf.Abs(brake) > 0f) ? brake : brakeInput;

        if (frontLeftWheel != null) frontLeftWheel.motorTorque = appliedMotor;
        if (frontRightWheel != null) frontRightWheel.motorTorque = appliedMotor;
        if (rearLeftWheel != null) rearLeftWheel.motorTorque = appliedMotor;
        if (rearRightWheel != null) rearRightWheel.motorTorque = appliedMotor;

        if (frontLeftWheel != null) frontLeftWheel.brakeTorque = appliedBrake;
        if (frontRightWheel != null) frontRightWheel.brakeTorque = appliedBrake;
        if (rearLeftWheel != null) rearLeftWheel.brakeTorque = appliedBrake;
        if (rearRightWheel != null) rearRightWheel.brakeTorque = appliedBrake;
    }

    void HandleSteering()
    {
        float steerInput = Input.GetAxis("Horizontal");
        float steerAngle = steerInput * maxSteerAngle;
        if (frontLeftWheel != null) frontLeftWheel.steerAngle = steerAngle;
        if (frontRightWheel != null) frontRightWheel.steerAngle = steerAngle;
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

    // Trigger for finish
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            Debug.Log("CHÚC MỪNG: Bạn đã về đích thành công!");

            if (finishUIText != null)
            {
                finishUIText.text = "CHÚC MỪNG!\nHOÀN THÀNH LEVEL 3";
                finishUIText.color = Color.green;
            }

            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restart pressed");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
