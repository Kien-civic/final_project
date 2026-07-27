using UnityEngine;
using TMPro;
using System.Collections;

public class TrafficSystem : MonoBehaviour
{
    // --- INITIATING THE SINGLETON LIBRARY (CENTRALIZED MANAGEMENT) ---
    public static TrafficSystem Instance { get; private set; }

    public enum LightColor { Green, Yellow, Red }

    [Header("Trạng thái Đèn")]
    public LightColor currentLight = LightColor.Green;

    [Header("Thời gian chuyển đèn (Giây)")]
    public float greenDuration = 7f;
    public float yellowDuration = 3f;
    public float redDuration = 7f;

    [Header("Liên kết mô hình Đèn thực tế")]
    public GameObject greenLampObject;
    public GameObject yellowLampObject;
    public GameObject redLampObject;

    [Header("Liên kết UI hiển thị (Kéo thả từ Canvas của Level hiện tại)")]
    public TextMeshProUGUI warningText;
    public TextMeshProUGUI scoreUIText;

    private float timer;
    private Coroutine clearTextCoroutine;
    private string lastMessage = "";
    private bool isCountdownRunning = false;

    void Awake()
    {
        // Establish a unique, centralized library management structure.
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            // Avoid having multiple TrafficSystem sets conflicting with each other in a single Scene.
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        currentLight = LightColor.Green;
        timer = greenDuration;
        UpdateVisualLights();

        // Automatically locates the Warning text box on the screen if you forgot to drag and drop it.
        if (warningText == null)
        {
            GameObject warningObj = GameObject.Find("WarningUIText");
            if (warningObj != null) warningText = warningObj.GetComponent<TextMeshProUGUI>();
        }

        if (warningText != null) warningText.text = "";
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SwitchLight();
        }
    }

    void SwitchLight()
    {
        if (currentLight == LightColor.Green)
        {
            currentLight = LightColor.Yellow;
            timer = yellowDuration;
        }
        else if (currentLight == LightColor.Yellow)
        {
            currentLight = LightColor.Red;
            timer = redDuration;
        }
        else if (currentLight == LightColor.Red)
        {
            currentLight = LightColor.Green;
            timer = greenDuration;
        }

        UpdateVisualLights();
    }

    void UpdateVisualLights()
    {
        if (greenLampObject != null) greenLampObject.SetActive(currentLight == LightColor.Green);
        if (yellowLampObject != null) yellowLampObject.SetActive(currentLight == LightColor.Yellow);
        if (redLampObject != null) redLampObject.SetActive(currentLight == LightColor.Red);
    }
    // ADDED AGAIN: This function completely fixes the CS1061 error in the TriggerZone script.
    public void CheckVehicleViolation(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentLight == LightColor.Red)
            {
                Debug.LogWarning("-> [CONSOLE] Phát hiện xe vượt đèn đỏ qua TriggerZone!");

                // Use Singleton to display bright red text on the home screen.
                ShowNotification("VI PHẠM: Vượt đèn đỏ! Trừ 50 điểm", Color.red);

                // Points will be deducted directly from the vehicle.
                AdvancedCarController carScript = other.GetComponent<AdvancedCarController>();
                if (carScript != null)
                {
                    carScript.score -= 50;
                }
            }
        }
    }


    // STANDARD NOTIFICATION DISPLAY FUNCTION - ABSOLUTELY NO CHARACTER SWALLOWING
    public void ShowNotification(string message, Color color)
    {
        if (warningText != null)
        {
            if (isCountdownRunning && lastMessage == message)
            {
                return;
            }

            warningText.text = message;
            warningText.color = color;
            lastMessage = message;

            if (clearTextCoroutine != null)
            {
                StopCoroutine(clearTextCoroutine);
            }

            isCountdownRunning = true;
            clearTextCoroutine = StartCoroutine(ClearTextAfterDelay(3f));
        }
    }

    private IEnumerator ClearTextAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (warningText != null)
        {
            warningText.text = "";
            lastMessage = "";
            isCountdownRunning = false;
        }
    }
}
