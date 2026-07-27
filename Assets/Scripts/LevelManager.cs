
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public bool checkpoint1Done = false;
    public bool checkpoint2Done = false;

    // The function checks if everything is complete.
    public bool AllCheckpointsCompleted()
    {
        return checkpoint1Done && checkpoint2Done;
    }
}



