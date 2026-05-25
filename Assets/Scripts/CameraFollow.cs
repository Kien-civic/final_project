using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float height = 8f;
    public float distance = 10f;
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // Vị trí phía sau xe
        Vector3 desiredPosition =
            target.position
            - target.forward * distance
            + Vector3.up * height;

        // Camera di chuyển mượt
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // Camera nhìn vào xe
        transform.LookAt(target);
    }
}
