using UnityEngine;

public class FerryController : MonoBehaviour
{
    [Header("Ferry Route Configuration")]
    public Transform bến_A;       // Drag and drop A station from Hierarchy into this location.
    public Transform bến_B;       // Drag and drop B station from Hierarchy into this location.
    public float speed = 3.5f;    // Ferry speed across the river

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        if (bến_A != null) transform.position = bến_A.position;
        targetPosition = bến_B.position;
    }

    void Update()
    {
        // When the ferry is activated to cross the river
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
    }

    // When a part of the vehicle touches the AttachTrigger stealth block.
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            Debug.Log("-> XE ĐÃ LÊN PHÀ THÀNH CÔNG!");

            // Lock the vehicle in place, allowing it to move along the ferry's course.
            other.transform.root.SetParent(transform);

            // After 1 second, the ferry will automatically weigh anchor and move across the river.
            Invoke("StartFerry", 1f);
        }
    }

    // When the player intentionally accelerates the vehicle out of the safe area of the ferry in the middle of the river.
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            Debug.Log("-> CẢNH BÁO: Xe đã tự ý di chuyển rời khỏi phà!");

            // Release the vehicle completely from the ferry to allow natural gravity to handle the free fall.
            other.transform.root.SetParent(null);

            // Swap the target dock to prepare the ferry for the next return trip.
            if (!isMoving)
            {
                targetPosition = (targetPosition == bến_B.position) ? bến_A.position : bến_B.position;
            }
        }
    }

    void StartFerry()
    {
        isMoving = true;
    }
}
