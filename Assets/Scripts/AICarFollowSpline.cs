using UnityEngine;
using UnityEngine.Splines; 

public class AICarFollowSpline : MonoBehaviour
{
    public SplineContainer splineContainer; // Drag and drop the path object here.
    public float speed = 15f;               // AI vehicle speed (m/s) ~ 54 km/h

    [Range(0f, 1f)]
    public float startingProgress = 0f;

    private float progress = 0f;
    private float splineLength = 0f;

    void Start()
    {
        if (splineContainer != null)
        {
            // Calculate the total length of the Spline path.
            splineLength = splineContainer.CalculateLength();
            progress = startingProgress;
        }
    }

    [Header("Vehicle Height Configuration")]
    [Tooltip("Height offset to ensure the wheels touch the ground correctly, avoiding sinking (e.g., 0.5, 1, 1.2...)")]
    public float heightOffset = 0.5f;

    void Update()
    {
        if (splineContainer == null || splineLength == 0) return;

        // Calculate the process (0 to 1)
        progress += (speed * Time.deltaTime) / splineLength;
        if (progress > 1f) progress = 0f; // Loop the path

        // --- CODE TO UPDATE VEHICLE HEIGHT TO MATCH THE GROUND ---

        // 1. Get the base position on the Spline (close to the ground)
        Vector3 splinePosition = (Vector3)splineContainer.EvaluatePosition(progress);

        // 2. ADD the height offset to the Y axis before assigning to the vehicle
        splinePosition.y += heightOffset;
        transform.position = splinePosition;

        // 3. Get the tangent direction (direction of the Spline)
        Vector3 tangent = (Vector3)splineContainer.EvaluateTangent(progress);

        // 4. If there is a direction, rotate the vehicle to face that direction
        if (tangent != Vector3.zero)
        {
            // Use Quaternion.LookRotation to create a rotation that looks in the direction of the tangent
            transform.rotation = Quaternion.LookRotation(tangent);

            // TIP: If the vehicle is still rotated incorrectly, you may need to offset by 90 degrees
            // transform.rotation = Quaternion.LookRotation(tangent) * Quaternion.Euler(0, 90, 0); 
            transform.rotation = Quaternion.LookRotation(tangent) * Quaternion.Euler(0, -90, 0);
        }
    }
}
