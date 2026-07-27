using UnityEngine;

public class PlayerWaterDetector : MonoBehaviour
{
    [Header("Water Scan Configuration")]
    public float waterLevelY = -0.5f; // The Y-axis height of the river's surface (Adjust this value according to your water object's Y-axis)
    public float checkDistance = 1.5f; // The distance from the underside of the vehicle to the water is used to determine if it is submerged.

    void Update()
    {
        // Method 1: Directly check the Y-axis coordinates of the vehicle.
        // If the vehicle loses control, veers off the ferry, and plunges below the river's surface.
        if (transform.position.y < (waterLevelY - 0.2f))
        {
            TriggerWaterFailure("Xe bị chìm sâu dưới mặt nước!");
            return;
        }

        // Method 2: Shoot a Raycast beam from the center of the car straight down under the chassis.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance))
        {
            // If the scan beam hits a block named WaterFailureTrigger or with a Water layer.
            if (hit.collider.gameObject.name == "WaterFailureTrigger" || hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                TriggerWaterFailure("Bánh xe chạm vào vùng nước tử thần!");
            }
        }
    }

    void TriggerWaterFailure(string reason)
    {
        Debug.Log("-> [XỬ PHẠT BACKEND] " + reason);

        // Call the game manager to declare a loss.
        RiverLevelManager manager = FindObjectOfType<RiverLevelManager>();
        if (manager != null && manager.score > 0)
        {
            manager.DeductPoints(manager.score, "Lao xuống sông chìm xe");

            // Cancel this script to avoid repeatedly calling the point deduction command.
            this.enabled = false;
        }
    }

    // Draw a red scan line in the Scene window so you can easily observe it with your eyes.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * checkDistance);
    }
}
