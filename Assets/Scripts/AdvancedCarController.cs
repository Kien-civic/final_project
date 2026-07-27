using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Added: Helps support basic UI elements
using TMPro;
using System.Collections;

public class AdvancedCarController : MonoBehaviour
{
    [Header("UI Panels Settings")]
    public GameObject finishPanel; // Frame containing the Winnings table (Finish)

    public enum GearState { P, R, N, D }

    [Header("Gameplay")]
    public int score = 100;
    private bool isGameOver = false;
    public TMP_Text speedText;
    private Vector3 lastPosition;

    [Header("Gear System")]
    public GearState currentGear = GearState.P;
    public TextMeshProUGUI gearUIText;
    public TextMeshProUGUI scoreText;

    [Header("Finish System")]
    public TextMeshProUGUI finishUIText; // ONLY ONE RETAINED: The TextMeshProUGUI variable no longer has a duplicate name.

    [Header("Lose/Win UI")]
    public GameObject restartButtonObject; // Drag and drop the Repeat or Lose Panel button.

    [Header("Car Settings")]
    public float motorForce = 7000f;
    public float brakeForce = 3000f;
    public float maxSteerAngle = 30f;

    [Header("Wheel Colliders")]
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

        // EXTRACT THE CURRENT GAME INDEX AND SAVE IT TO THE DEVICE.
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        // Only save if the level index is greater than the LevelSelect and MainMenu
        if (currentSceneIndex >= 2)
        {
            PlayerPrefs.SetInt("SavedLevelIndex", currentSceneIndex);
            PlayerPrefs.Save(); // Lock data to the device's hard drive.
            Debug.Log("-> [BACKEND] Đã tự động lưu tiến trình chơi: Level Index " + currentSceneIndex);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) ShiftGearUp();
        if (Input.GetKeyDown(KeyCode.Q)) ShiftGearDown();
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

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
        float motorInput = 0f;
        float brakeInput = 0f;

        bool isPressingGas = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool isPressingBrake = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        switch (currentGear)
        {
            // PARK
            case GearState.P:
                motorInput = 0f;
                brakeInput = brakeForce;
                break;

            // NEUTRAL
            case GearState.N:
                motorInput = 0f;

                if (isPressingBrake)
                    brakeInput = brakeForce;

                break;

            // DRIVE
            case GearState.D:

                // W = đi tới
                if (isPressingGas)
                {
                    motorInput = motorForce;
                    brakeInput = 0f;
                }

                // S = phanh
                if (isPressingBrake)
                {
                    motorInput = 0f;
                    brakeInput = brakeForce;
                }

                break;

            // REVERSE
            case GearState.R:

                // W = lùi xe
                if (isPressingGas)
                {
                    motorInput = -motorForce;
                    brakeInput = 0f;
                }

                // S = phanh
                if (isPressingBrake)
                {
                    motorInput = 0f;
                    brakeInput = brakeForce;
                }

                break;
        }

        // Power is only transmitted to the rear wheel.
        rearLeftWheel.motorTorque = motorInput;
        rearRightWheel.motorTorque = motorInput;

        // Brakes on all four wheels
        frontLeftWheel.brakeTorque = brakeInput;
        frontRightWheel.brakeTorque = brakeInput;
        rearLeftWheel.brakeTorque = brakeInput;
        rearRightWheel.brakeTorque = brakeInput;
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
                int currentLevel = SceneManager.GetActiveScene().buildIndex - 1;
                finishUIText.text = "CHÚC MỪNG!\nHOÀN THÀNH LEVEL ";
                finishUIText.color = Color.green;
            }

            if (finishPanel != null)
            {
                finishPanel.SetActive(true);
            }

            Time.timeScale = 0f;
        }
    }

    // --- FUNCTIONS FOR BUTTON CLICKS ---

    public void ClickRepeat()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ClickNextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Đã hết Level! Quay lại Menu chính.");
            SceneManager.LoadScene(0);
        }
    }

    public void RestartGame()
    {
        Debug.Log("RESTART BUTTON CLICKED - LOADING SCENE...");
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
