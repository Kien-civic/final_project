using UnityEngine;
using TMPro;

public class TrafficSystem : MonoBehaviour
{
    public enum LightColor { Green, Yellow, Red }
    [Header("Trạng thái Đèn")]
    public LightColor currentLight = LightColor.Green;

    [Header("Thời gian chuyển đèn (Giây)")]
    public float greenDuration = 7f;
    public float yellowDuration = 3f;
    public float redDuration = 7f;

    [Header("Liên kết mô hình Đèn thực tế")]
    public GameObject greenLampObject;  // Kéo khối cầu đèn Xanh vào đây
    public GameObject yellowLampObject; // Kéo khối cầu đèn Vàng vào đây
    public GameObject redLampObject;    // Kéo khối cầu đèn Đỏ vào đây

    [Header("Liên kết UI hiển thị lỗi")]
    public TextMeshProUGUI warningText; 
    public TextMeshProUGUI scoreUIText; // Ô trống để kéo chữ UI "Điểm: 100" vào
    private int playerScore = 100;      // Biến lưu điểm số thực tế ban đầu là 100

    private float timer;

    void Start()
    {
        currentLight = LightColor.Green;
        timer = greenDuration;
        UpdateVisualLights(); // Cập nhật đèn lúc bắt đầu
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
            if (warningText != null) warningText.text = ""; 
        }

        UpdateVisualLights(); // Mỗi lần đổi trạng thái thì bật/tắt đèn tương ứng
    }

    // HÀM TỰ ĐỘNG BẬT ĐÈN ĐÚNG MÀU VÀ TẮT ĐÈN SAI MÀU
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
                Debug.LogWarning("VI PHẠM: Bạn đã vượt đèn đỏ!");
                
                // 1. Hiển thị chữ báo lỗi lên màn hình chính
                if (warningText != null)
                {
                    warningText.text = "VI PHẠM: Vượt đèn đỏ! Trừ 50 điểm";
                    warningText.color = Color.red;
                }

                // 2. --- LOGIC KẾT NỐI SANG SCRIPT TRÊN XE ĐỂ TRỪ ĐIỂM ---
                // Tìm thành phần script điều khiển xe nằm trên vật thể vừa đâm vào Trigger
                AdvancedCarController carScript = other.GetComponent<AdvancedCarController>();
                
                // Nếu không tìm thấy tên AdvancedCarController, thử tìm theo tên CarController cũ
                if (carScript == null)
                {
                    // Bạn kiểm tra xem script trên xe ở Level 3 tên là gì nhé. 
                    // Nếu dùng file cũ thì bật dòng dưới này lên bằng cách xóa 2 dấu gạch chéo:
                    // CarController carScriptOld = other.GetComponent<CarController>();
                }

                if (carScript != null)
                {
                    // Giả sử trong script xe của bạn có biến chứa điểm tên là 'score' hoặc 'point'
                    // Ở đây mình ví dụ biến tên là 'score'. Bạn sửa lại cho đúng tên biến trong code xe của bạn nhé.
                    carScript.score -= 50; 

                    // Gọi hàm cập nhật lại chữ hiển thị điểm số trên màn hình của script xe (nếu có)
                    // Hoặc nếu script xe tự cập nhật điểm ở hàm Update() thì dòng này không cần thiết.
                    //carScript.UpdateScoreUI(); 
                }
            }
        }
    }
}
