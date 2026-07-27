using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class AIVehicleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class LevelSplineConfig
    {
        public SplineContainer splineContainer; // Spline curve of this route
        public bool isOneWay = false;           // Is this route one-way or two-way?
        public float laneOffsetAmount = 2.4f;   // Lane departure width
    }

    [Header("Traffic Routes in Level")]
    public List<LevelSplineConfig> trafficRoutes; // List of roads that vehicles can travel on.

    [Header("AI Vehicle Prefabs")]
    public List<GameObject> vehiclePrefabs;       // Drag AI (or human/dog) car models here.

    [Header("Random Settings")]
    public int maxVehiclesInLevel = 10;           // The maximum number of cars you want to spawn in the screen.
    public Vector2 speedRange = new Vector2(5f, 15f); // Random speed from Min to Max

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
            // 1. Randomly select a car model from the list of meritorious Prefabs.
            GameObject selectedPrefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Count)];

            // 2. Randomly select a Spline route on the map to drop the vehicle off.
            LevelSplineConfig selectedRoute = trafficRoutes[Random.Range(0, trafficRoutes.Count)];

            // 3. Initiate vehicle creation (Instantiate) into the game world.
            GameObject newVehicle = Instantiate(selectedPrefab, Vector3.zero, Quaternion.identity);

            // 4. Extract the vehicle movement script and stuff random parameters into it.
            AISameDirectionController controller = newVehicle.GetComponent<AISameDirectionController>();

            if (controller != null)
            {
                // Assign a random route to the winning team.
                controller.splineContainer = selectedRoute.splineContainer;

                // RANDOM STARTING POSITION: Scattered from 0.0 (beginning of the line) to 0.9 (near the end of the line)troller.startProgress = Random.Range(0f, 0.9f);

                // RANDOM SPEED: Each car speeds at a different rate; no two are alike.
                controller.normalSpeed = Random.Range(speedRange.x, speedRange.y);

                // RANDOM LANE ASSIGNMENT (If it's a two-way street)
                if (!selectedRoute.isOneWay)
                {
                    // Randomly generated negative numbers (left lane/same direction) or positive numbers (right lane/opposite direction).
                    bool goSameDirection = (Random.value > 0.5f);
                    controller.laneOffset = goSameDirection ? -selectedRoute.laneOffsetAmount : selectedRoute.laneOffsetAmount;

                    // If going in the opposite direction, we rotate the vehicle 180 degrees relative to the original direction of the Spline.
                    if (!goSameDirection)
                    {
                        controller.rotationOffset = new Vector3(0, 180f, 0);
                    }
                }
                else
                {
                    // If it's a one-way street, it will automatically move into the same-direction lane.
                    controller.laneOffset = -selectedRoute.laneOffsetAmount;
                }
            }
        }
    }
}

