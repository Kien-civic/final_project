using UnityEngine;

public class BrakeTriggerZone : MonoBehaviour
{
    // Instead of dragging a single car, if there are multiple cars, we can use an array or only activate the car that passes through.
    public AISameDirectionController targetAICar;

    private bool hasTriggered = false; // Flag to prevent infinite loop

    void OnTriggerEnter(Collider other)
    {
        // Check if the player's car actually crashed into it and if any traps were used.
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (targetAICar != null)
            {
                hasTriggered = true; // Lock the trap immediately!

                targetAICar.TriggerEmergencyBrake(); // Call the car to brake

                Debug.LogWarning("BẪY ĐÃ KÍCH HOẠT VÀ TỰ KHÓA AN TOÀN!");

                // Remove this trigger box from the scene screen to free up memory.
                Destroy(gameObject, 0.1f);
            }
        }
    }
}
