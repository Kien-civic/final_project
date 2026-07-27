using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    // Link to the main traffic light management system
    public TrafficSystem trafficSystem;

    void OnTriggerEnter(Collider other)
    {
        if (trafficSystem != null)
        {
            // Call the function to check if you've been fined for running a red light.
            trafficSystem.CheckVehicleViolation(other);
        }
    }
}
