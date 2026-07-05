using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class AIVehicleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class LevelSplineConfig
    {
        public SplineContainer splineContainer; // Đường spline của tuyến đường này
        public bool isOneWay = false;           // Tuyến đường này là 1 chiều hay 2 chiều?
        public float laneOffsetAmount = 2.4f;   // Độ rộng để dạt làn
    }

    [Header("Tuyến Đường Trong Màn Chơi")]
    public List<LevelSplineConfig> trafficRoutes; // Danh sách các con đường xe có thể chạy

    [Header("Danh Sách Mẫu Xe AI (Prefabs)")]
    public List<GameObject> vehiclePrefabs;       // Kéo các Model xe AI (hoặc người/chó) vào đây

    [Header("Cấu Hình Ngẫu Nhiên (Random Settings)")]
    public int maxVehiclesInLevel = 10;           // Số lượng xe tối đa muốn sinh ra trong màn
    public Vector2 speedRange = new Vector2(5f, 15f); // Tốc độ ngẫu nhiên từ Min đến Max

    void Start()
    {
        if (vehiclePrefabs == null || vehiclePrefabs.Count == 0 || trafficRoutes == null || trafficRoutes.Count == 0)
        {
            Debug.LogError("Chưa cấu hình Tuyến đường hoặc Prefabs xe trong Spawner!");
            return;
        }

        SpawnRandomTraffic();
    }

    void SpawnRandomTraffic()
    {
        for (int i = 0; i < maxVehiclesInLevel; i++)
        {
            // 1. Chọn ngẫu nhiên một mẫu xe trong danh sách Prefabs công đức
            GameObject selectedPrefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Count)];

            // 2. Chọn ngẫu nhiên một tuyến đường Spline trong map để thả xe xuống
            LevelSplineConfig selectedRoute = trafficRoutes[Random.Range(0, trafficRoutes.Count)];

            // 3. Tiến hành sinh xe (Instantiate) vào thế giới game
            GameObject newVehicle = Instantiate(selectedPrefab, Vector3.zero, Quaternion.identity);

            // 4. Lấy script di chuyển trên xe ra để nhồi thông số ngẫu nhiên vào
            AISameDirectionController controller = newVehicle.GetComponent<AISameDirectionController>();

            if (controller != null)
            {
                // Gán đường chạy ngẫu nhiên trúng tuyển
                controller.splineContainer = selectedRoute.splineContainer;

                // RANDOM VỊ TRÍ XUẤT PHÁT: Cho rải rác từ 0.0 (đầu đường) đến 0.9 (gần cuối đường)
                controller.startProgress = Random.Range(0f, 0.9f);

                // RANDOM TỐC ĐỘ: Mỗi xe phóng một tốc độ khác nhau không ai giống ai
                controller.normalSpeed = Random.Range(speedRange.x, speedRange.y);

                // RANDOM LÀN ĐƯỜNG (Nếu là đường 2 chiều)
                if (!selectedRoute.isOneWay)
                {
                    // Ngẫu nhiên ra số âm (đi làn trái/cùng chiều) hoặc số dương (làn phải/ngược chiều)
                    bool goSameDirection = (Random.value > 0.5f);
                    controller.laneOffset = goSameDirection ? -selectedRoute.laneOffsetAmount : selectedRoute.laneOffsetAmount;

                    // Nếu đi ngược chiều, ta xoay đầu xe lại 180 độ so với hướng gốc của Spline
                    if (!goSameDirection)
                    {
                        controller.rotationOffset = new Vector3(0, 180f, 0);
                    }
                }
                else
                {
                    // Nếu là đường 1 chiều, ép dạt sang làn cùng chiều mặc định
                    controller.laneOffset = -selectedRoute.laneOffsetAmount;
                }
            }
        }
    }
}
