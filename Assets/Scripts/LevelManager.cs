
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public bool checkpoint1Done = false;
    public bool checkpoint2Done = false;

    // Hàm kiểm tra đã hoàn thành tất cả chưa
    public bool AllCheckpointsCompleted()
    {
        return checkpoint1Done && checkpoint2Done;
    }
}



