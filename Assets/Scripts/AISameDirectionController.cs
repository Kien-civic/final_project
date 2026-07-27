using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class AISameDirectionController : MonoBehaviour
{
    [Header("Spline Settings")]
    public SplineContainer splineContainer;
    public float laneOffset = -2.4f;

    [Header("Position & Rotation Fix")]
    public float heightOffset = 0.5f;
    public Vector3 rotationOffset = new Vector3(0, 0, 0);

    [Header("Spawn Position")]
    [Range(0f, 1f)]
    public float startProgress = 0f;        // Select the starting position for each vehicle (0.0 to 1.0)

    [Header("Brake Event Settings")]
    public float normalSpeed = 12f;
    public float brakeSpeed = 0f;
    public float deceleration = 70f;        // You are setting 70f, very sharp braking, keep it as is!

    [Header("Auto Resume Settings")]
    public float stopDuration = 3f;         // Number of seconds the vehicle stays stopped before resuming

    private float currentSpeed;
    private float progress = 0f;
    private float splineLength;
    private bool isBraking = false;
    private float stopTimer = 0f;           // Timer for counting the stop duration

    void Start()
    {
        currentSpeed = normalSpeed;
        if (splineContainer != null)
        {
            splineLength = splineContainer.CalculateLength();
        }

        // EACH VEHICLE HAS A UNIQUE STARTING POSITION
        progress = startProgress;
    }

    void Update()
    {
        if (splineContainer == null || splineLength == 0) return;

        // BRAKE AND AUTO RESUME LOGIC
        if (isBraking)
        {
            // Reduce speed to brakeSpeed
            currentSpeed = Mathf.MoveTowards(currentSpeed, brakeSpeed, deceleration * Time.deltaTime);

            // If the vehicle has slowed down to or near the brake speed (fully stopped)
            if (currentSpeed <= brakeSpeed + 0.1f)
            {
                stopTimer += Time.deltaTime; // Start counting the stop duration

                if (stopTimer >= stopDuration)
                {
                    // STOP DURATION ELAPSED -> RELEASE BRAKE AND RESUME MOVEMENT
                    isBraking = false;
                    stopTimer = 0f;
                }
            }
        }
        else
        {
            // If not braking (or just released the brake), smoothly accelerate back to normal speed
            currentSpeed = Mathf.MoveTowards(currentSpeed, normalSpeed, deceleration * Time.deltaTime);
        }

        // Calculate the progress along the Spline
        float deltaProgress = (currentSpeed / splineLength) * Time.deltaTime;
        progress += deltaProgress;

        // Loop logic: If the vehicle reaches the end of the path, reset to the start and reset state
        if (progress >= 1f)
        {
            progress = 0f;
            isBraking = false;
            stopTimer = 0f;
            currentSpeed = normalSpeed;
        }

        // Calculate the position and rotation along the Spline (Keep your fix  )
        splineContainer.Evaluate(progress, out float3 splinePosition, out float3 forward, out float3 up);
        float3 rightDirection = math.cross(up, forward);
        float3 offsetVector = math.normalize(rightDirection) * laneOffset;

        Vector3 finalPosition = (Vector3)(splinePosition + offsetVector + (up * heightOffset));
        transform.position = finalPosition;

        if (math.any(forward != float3.zero))
        {
            Quaternion baseRotation = Quaternion.LookRotation((Vector3)forward, (Vector3)up);
            transform.rotation = baseRotation * Quaternion.Euler(rotationOffset);
        }
    }

    public void TriggerEmergencyBrake()
    {
        isBraking = true;
        stopTimer = 0f; // Reset the stop duration timer each time the brake is triggered
        Debug.LogWarning("EMERGENCY BRAKE TRIGGERED: Vehicle is braking!");
    }
}
