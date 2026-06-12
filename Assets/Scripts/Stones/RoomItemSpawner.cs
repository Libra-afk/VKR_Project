using System.Collections.Generic;
using UnityEngine;

public class RoomItemSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private GameObject extraHpPrefab;
    [SerializeField] private GameObject bossKeyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int rocksCount = 3;
    [SerializeField] private Vector2 spawnRangeX = new Vector2(-2.2f, 2.2f);
    [SerializeField] private Vector2 spawnRangeY = new Vector2(-1.1f, 1.1f);
    [SerializeField] private float minDistanceBetweenRocks = 1.5f;
    [SerializeField] private Vector2 rockCheckSize = new Vector2(1.2f, 1.2f);
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private int maxSpawnAttempts = 100;



    [SerializeField] private Room room;

    private bool rocksSpawned = false;

    private void Awake()
    {
        if (room == null)
            room = GetComponent<Room>();
    }

    private void Update()
    {
        // Ждём, пока currentCell будет назначен MapGenerator
        if (!rocksSpawned && room != null && room.currentCell != null)
        {
            SpawnRocksWithItems();
            rocksSpawned = true; // больше не спавним
        }
    }

    private void SpawnRocksWithItems()
    {
        if (room.currentCell.roomType != RoomType.Item &&
            room.currentCell.roomType != RoomType.Shop)
            return;

        int bonusIndex = Random.Range(0, rocksCount);
        List<Vector2> usedPositions = new List<Vector2>();

        for (int i = 0; i < rocksCount; i++)
        {
            if (!TryGetRockSpawnPosition(usedPositions, out Vector2 spawnPos))
            {
                Debug.LogWarning("Не удалось найти свободное место для камня.");
                continue;
            }

            GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity, room.transform);
            usedPositions.Add(spawnPos);

            if (i == bonusIndex)
            {
                DestructibleStoneVisual visual = rock.GetComponentInChildren<DestructibleStoneVisual>();

                if (visual != null)
                {
                    GameObject itemToSpawn = room.currentCell.roomType == RoomType.Item
                        ? bossKeyPrefab
                        : extraHpPrefab;

                    visual.SetItemPrefab(itemToSpawn);
                }
                else
                {
                    Debug.LogWarning("DestructibleStoneVisual не найден на камне или его дочерних объектах.");
                }
            }
        }
    }

    private bool TryGetRockSpawnPosition(List<Vector2> usedPositions, out Vector2 spawnPos)
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            float x = Random.Range(spawnRangeX.x, spawnRangeX.y);
            float y = Random.Range(spawnRangeY.x, spawnRangeY.y);

            Vector2 candidate = (Vector2)room.transform.position + new Vector2(x, y);

            bool tooCloseToOtherRock = false;

            foreach (Vector2 usedPosition in usedPositions)
            {
                if (Vector2.Distance(candidate, usedPosition) < minDistanceBetweenRocks)
                {
                    tooCloseToOtherRock = true;
                    break;
                }
            }

            if (tooCloseToOtherRock)
                continue;

            Collider2D obstacle = Physics2D.OverlapBox(
                candidate,
                rockCheckSize,
                0f,
                obstacleLayerMask
            );

            if (obstacle != null)
                continue;

            spawnPos = candidate;
            return true;
        }

        spawnPos = Vector2.zero;
        return false;
    }

}