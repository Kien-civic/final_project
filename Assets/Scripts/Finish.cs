
using UnityEngine;

public class Finish : MonoBehaviour
{
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (levelManager.AllCheckpointsCompleted())
            {
                Debug.Log("WIN LEVEL!");
            }
            else
            {
                Debug.Log("Bạn chưa hoàn thành đủ checkpoint!");
            }
        }
    }
}



