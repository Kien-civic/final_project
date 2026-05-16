using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Kéo Playcar vào đây
    public float height = 15f; // Độ cao của camera

    void LateUpdate()
    {
        if (target != null)
        {
            // Camera luôn ở vị trí của xe nhưng cộng thêm độ cao Y
            transform.position = new Vector3(target.position.x, height, target.position.z);
            
            // Luôn nhìn thẳng xuống mặt đất
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
