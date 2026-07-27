using UnityEngine;

public class SurpriseBrakeEvent : MonoBehaviour
{
    [Header("Brake Settings")]
    public float normalSpeed = 12f;
    public float brakeSpeed = 0f;        // Brake hard to 0 (complete stop) or 2-3f (slow braking)
    public float deceleration = 5f;      // Smoothness when braking (higher value means more abrupt braking)

    [Header("Visual Effects (Optional)")]
    public GameObject brakeLights;       // Rear brake lights (if any, automatically turn on when braking)

    private float currentSpeed;
    private bool isBraking = false;

    void Start()
    {
        currentSpeed = normalSpeed;
        if (brakeLights != null) brakeLights.SetActive(false);
    }

    void Update()
    {
        // If a braking event is triggered, rapidly reduce speed to brakespeed.
        if (isBraking)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, brakeSpeed, deceleration * Time.deltaTime);
        }

        // Move the vehicle forward (along the vehicle's axis).
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    // EVENT DRIVEN FUNCTION: Will be called from the outside to activate the brake.
    public void TriggerEmergencyBrake()
    {
        if (!isBraking)
        {
            isBraking = true;
            Debug.LogWarning("EVENT ACTIVATED: Xe phía trước phanh gấp!");
            if (brakeLights != null) brakeLights.SetActive(true); // Turn on the bright red brake lights.
        }
    }
}
