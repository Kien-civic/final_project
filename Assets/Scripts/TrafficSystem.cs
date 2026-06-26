using UnityEngine;
using TMPro;
using System.Collections;

public class TrafficSystem : MonoBehaviour
{
    // --- KHỞI TẠO THƯ VIỆN SINGLETON (BỘ QUẢN LÝ TẬP TRUNG) ---
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
        // Thiết lập cấu trúc thư viện quản lý tập trung độc nhất
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            // Tránh việc có nhiều bộ TrafficSystem đá nhau trong một Scene
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        currentLight = LightColor.Green;
        timer = greenDuration;
        UpdateVisualLights();

        // Tự động đi tìm ô chữ Warning trên màn hình nếu quên chưa kéo thả
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
    // ĐÃ THÊM LẠI: Hàm này giúp sửa triệt để lỗi CS1061 ở script TriggerZone
    public void CheckVehicleViolation(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentLight == LightColor.Red)
            {
                Debug.LogWarning("-> [CONSOLE] Phát hiện xe vượt đèn đỏ qua TriggerZone!");

                // Gọi lệnh hiển thị chữ đỏ rực lên màn hình chính thông qua Singleton
                ShowNotification("VI PHẠM: Vượt đèn đỏ! Trừ 50 điểm", Color.red);

                // Tiến hành trừ điểm trực tiếp trên xe
                AdvancedCarController carScript = other.GetComponent<AdvancedCarController>();
                if (carScript != null)
                {
                    carScript.score -= 50;
                }
            }
        }
    }


    // HÀM HIỂN THỊ THÔNG BÁO CHUẨN - CHỐNG NUỐT CHỮ TUYỆT ĐỐI
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
