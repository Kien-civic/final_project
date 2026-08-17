using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic; 

public class TrafficSystem : MonoBehaviour
{
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

    [Header("Liên kết UI hiển thị")]
    public TextMeshProUGUI warningText;
    public TextMeshProUGUI scoreUIText;

    // ---Variables that store a fixed list of violations.
    [HideInInspector]
    public List<string> violationLog = new List<string>();

    private float timer;
    private Coroutine clearTextCoroutine;
    private string lastMessage = "";
    private bool isCountdownRunning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        currentLight = LightColor.Green;
        timer = greenDuration;
        UpdateVisualLights();

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

    public void CheckVehicleViolation(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentLight == LightColor.Red)
            {
                Debug.LogWarning("-> [CONSOLE] Phát hiện xe vượt đèn đỏ qua TriggerZone!");

                ShowNotification("VI PHẠM: Vượt đèn đỏ! Trừ 50 điểm", Color.red);

                AdvancedCarController carScript = other.GetComponent<AdvancedCarController>();
                if (carScript != null)
                {
                    carScript.score -= 50;
                }
            }
        }
    }

    public void ShowNotification(string message, Color color)
    {
        // ---Log the violation notification to the list for display after Game Over.
        violationLog.Add("- " + message);

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
