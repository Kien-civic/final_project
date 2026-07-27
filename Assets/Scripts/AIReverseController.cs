using UnityEngine;
using UnityEngine.Splines; 
using Unity.Mathematics;

public class AIReverseController : MonoBehaviour
{
    [Header("Spline Settings")]
    public SplineContainer splineContainer; // Drag the Spline strip (1) here
    public float speed = 10f;               // AI vehicle speed (m/s)
    public float laneOffset = 2.4f;         // Lane offset distance (X axis)
    public float heightOffset = 0.5f; // Height offset to lift the vehicle off the ground (meters)
    public Vector3 rotationOffset = new Vector3(0, 0, 0); // Rotation offset for the vehicle's front

    private float progress = 1f;            // Start from the end of the path (1.0 = 100%)
    private float splineLength;

    [Header("Spawn Position")]
    [Range(0f, 1f)]
    public float startProgress = 1f; // Custom spawn position (From 0.0 to 1.0)

    void Start()
    {
        if (splineContainer != null)
        {
            // Calculate the total length of the Spline path to convert speed from meters/second    
            splineLength = splineContainer.CalculateLength();
        }
        progress = startProgress;
    }

    void Update()
    {
        if (splineContainer == null || splineLength == 0) return;

        // 1. REVERSE LOGIC: Decrease progress over time to move the vehicle backward along the path
        float deltaProgress = (speed / splineLength) * Time.deltaTime;
        progress -= deltaProgress;

        // If the vehicle reaches the start of the path (0), reset it to the end (1) to continue moving
        if (progress <= 0f)
        {
            progress = 1f;
        }

        // 2. Calculate the base position and rotation at the current point on the Spline
        // Use the Evaluate function to get the position, forward direction, and up direction
        splineContainer.Evaluate(progress, out float3 splinePosition, out float3 forward, out float3 up);

        // 3. REVERSE LANE LOGIC: Calculate the offset vector to the right lane
        // Use the cross product between the forward and up directions to find the right direction vector
        float3 rightDirection = math.cross(up, forward);
        float3 offsetVector = math.normalize(rightDirection) * laneOffset;

        // Add up * heightOffset to lift the vehicle off the ground along the perpendicular direction to the road
        Vector3 finalPosition = (Vector3)(splinePosition + offsetVector + (up * heightOffset));

        // 4. Update the position and rotation of the AI vehicle
        transform.position = finalPosition;

        // Because the vehicle is moving in the reverse direction along the Spline, the look direction must be inverted by 180 degrees
        Vector3 lookDirection = -(Vector3)forward;
        if (lookDirection != Vector3.zero)
        {
            // 1. Calculate the base rotation according to the forward direction
            Quaternion baseRotation = Quaternion.LookRotation(lookDirection, (Vector3)up);

            // 2. Apply the rotation offset to align the vehicle's front with the lane
            transform.rotation = baseRotation * Quaternion.Euler(rotationOffset);
        }
    }
}
