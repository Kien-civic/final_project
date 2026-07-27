using UnityEngine;

public class MountainCameraFollow : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform target;           // Drag and drop Playcar here

    [Header("Basic Distance Configuration")]
    public float defaultArmLength = 6f; // Default arm length when driving straight
    public float minArmLength = 3.5f;   // Minimum arm length when making sharp turns or reversing close to the mountain wall
    public float maxArmLength = 8f;     // Maximum arm length when speeding down a slope for a better view
    public float heightOffset = 2.5f;   // Default camera height relative to the car

    [Header("Response Speed (Smooth)")]
    public float movementSmooth = 5f;   // Smooth follow speed
    public float rotationSmooth = 5f;   // Smooth rotation speed
    public float zoomSmooth = 3f;       // Smooth zoom speed (TargetArmLength)

    [Header("Smart Features for Mountain Roads")]
    [Tooltip("Automatically raise the camera when the car is climbing a steep slope")]
    public bool autoHeightOnSlopes = true;

    private float currentArmLength;
    private Rigidbody targetRigidbody;

    void Start()
    {
        currentArmLength = defaultArmLength;
        if (target != null)
        {
            targetRigidbody = target.GetComponent<Rigidbody>();
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. AUTOMATICALLY ADJUST TARGET ARM LENGTH
        float desiredArmLength = defaultArmLength;

        if (targetRigidbody != null)
        {
            // Get the actual speed of the car
            float speed = targetRigidbody.linearVelocity.magnitude * 3.6f; // km/h

            // Get the current turn rate of the car (If the car is making a sharp turn, the Y-axis turn speed will be high)
            float turnSpeed = Mathf.Abs(targetRigidbody.angularVelocity.y);

            // SHARP TURN LOGIC: When making a sharp turn (high turnSpeed), shorten the Arm Length to bring the camera closer, providing a clear view of the turn
            if (turnSpeed > 0.3f)
            {
                desiredArmLength = Mathf.Lerp(defaultArmLength, minArmLength, turnSpeed * 0.5f);
            }
            // SPEED LOGIC: When speeding down a straight slope, extend the Arm Length for a better view
            else if (speed > 40f)
            {
                desiredArmLength = Mathf.Lerp(defaultArmLength, maxArmLength, (speed - 40f) / 60f);
            }
        }

        // Smoothly interpolate the current arm length
        currentArmLength = Mathf.Lerp(currentArmLength, desiredArmLength, Time.deltaTime * zoomSmooth);


        // 2. AUTOMATICALLY CALCULATE HEIGHT BASED ON SLOPE (Y CHANGES)
        float currentHeightOffset = heightOffset;

        if (autoHeightOnSlopes)
        {
            // Check the car's forward direction to see if it's pitching up (climbing) or down
            float pitchAngle = target.eulerAngles.x;
            // Normalize the angle to the range -180 to 180
            if (pitchAngle > 180) pitchAngle -= 360;

            // If the car is climbing (pitchAngle < 0), raise the camera to avoid the slope obstructing the forward view
            if (pitchAngle < -5f)
            {
                currentHeightOffset += Mathf.Abs(pitchAngle) * 0.08f;
            }
        }


        // 3. POSITION AND ROTATION FOLLOWING THE CAR
        // Calculate the look direction from behind the car based on the car's Y-axis rotation
        Quaternion targetRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);

        // Target position that the Camera wants to reach (Move back by currentArmLength and lift up by currentHeightOffset)
        Vector3 targetPosition = target.position - (targetRotation * Vector3.forward * currentArmLength) + (Vector3.up * currentHeightOffset);

        // Smoothly move and rotate the Camera using Lerp / Slerp
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * movementSmooth);

        // Always rotate the camera to look directly at the car's center
        Vector3 lookAtPos = target.position + Vector3.up * 1f; // Look at the car's waist
        Quaternion lookRotation = Quaternion.LookRotation(lookAtPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSmooth);
    }
}
