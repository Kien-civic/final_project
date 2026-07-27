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

        // Rear position of the car
        Vector3 desiredPosition =
            target.position
            - target.forward * distance
            + Vector3.up * height;

        // The camera moves smoothly.
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // Camera looking inside the car.
        transform.LookAt(target);
    }
}
