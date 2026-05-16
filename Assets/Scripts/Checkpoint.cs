
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID;

    private Renderer rend;
    private bool completed = false;

    private LevelManager levelManager;

    private void Start()
    {
        rend = GetComponent<Renderer>();

        // Tìm LevelManager
        levelManager = FindObjectOfType<LevelManager>();

        rend.material.color = Color.white;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !completed)
        {
            completed = true;

            // Đổi vàng
            rend.material.color = Color.yellow;

            Debug.Log("Checkpoint " + checkpointID + " completed!");

            // Báo về LevelManager
            if (checkpointID == 1)
            {
                levelManager.checkpoint1Done = true;
            }

            if (checkpointID == 2)
            {
                levelManager.checkpoint2Done = true;
            }
        }
    }
}




