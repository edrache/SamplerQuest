using UnityEngine;
using System.Collections.Generic;

namespace SamplerQuest
{
    public class RandomSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject prefabToSpawn;
        [SerializeField] private float minSpawnInterval = 1f;
        [SerializeField] private float maxSpawnInterval = 3f;
        [SerializeField] private float instanceLifetime = 5f;

        [Header("Position Offset")]
        [SerializeField] private Vector2 xOffsetRange = new Vector2(-1f, 1f);
        [SerializeField] private Vector2 yOffsetRange = new Vector2(-1f, 1f);
        [SerializeField] private Vector2 zOffsetRange = new Vector2(-1f, 1f);

        private List<GameObject> activeInstances = new List<GameObject>();
        private float nextSpawnTime;
        private bool isSpawning = false;

        private void OnValidate()
        {
            // Ensure ranges are valid
            if (minSpawnInterval > maxSpawnInterval)
            {
                maxSpawnInterval = minSpawnInterval;
            }

            if (xOffsetRange.x > xOffsetRange.y)
            {
                xOffsetRange.y = xOffsetRange.x;
            }

            if (yOffsetRange.x > yOffsetRange.y)
            {
                yOffsetRange.y = yOffsetRange.x;
            }

            if (zOffsetRange.x > zOffsetRange.y)
            {
                zOffsetRange.y = zOffsetRange.x;
            }
        }

        private void Update()
        {
            // Clean up destroyed instances from the list
            activeInstances.RemoveAll(instance => instance == null);

            // Handle automatic spawning
            if (isSpawning && Time.time >= nextSpawnTime)
            {
                SpawnInstance();
                ScheduleNextSpawn();
            }
        }

        private void ScheduleNextSpawn()
        {
            float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
            nextSpawnTime = Time.time + interval;
        }

        public void SpawnInstance()
        {
            if (prefabToSpawn == null)
            {
                Debug.LogError("No prefab assigned to spawn!");
                return;
            }

            // Calculate random position offset
            float xOffset = Random.Range(xOffsetRange.x, xOffsetRange.y);
            float yOffset = Random.Range(yOffsetRange.x, yOffsetRange.y);
            float zOffset = Random.Range(zOffsetRange.x, zOffsetRange.y);
            Vector3 spawnPosition = transform.position + new Vector3(xOffset, yOffset, zOffset);

            // Spawn the instance
            GameObject instance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            activeInstances.Add(instance);

            // Schedule destruction
            Destroy(instance, instanceLifetime);
        }

        public void StartSpawning()
        {
            if (!isSpawning)
            {
                isSpawning = true;
                ScheduleNextSpawn();
                Debug.Log("Started automatic spawning");
            }
        }

        public void StopSpawning()
        {
            if (isSpawning)
            {
                isSpawning = false;
                Debug.Log("Stopped automatic spawning");
            }
        }

        private void OnDestroy()
        {
            // Clean up any remaining instances
            foreach (var instance in activeInstances)
            {
                if (instance != null)
                {
                    Destroy(instance);
                }
            }
            activeInstances.Clear();
        }
    }
} 