using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyRoomSpawner : MonoBehaviour
{
    [SerializeField] private GameObject skeletonPrefab;

    [SerializeField] private GameObject bossPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Vector2 oneCellSize = new Vector2(10.6f, 5.3f);
    [SerializeField] private float spawnPaddingX = 2f;
    [SerializeField] private float spawnPaddingY = 1.2f;
    [SerializeField] private float minDistanceBetweenEnemies = 1.5f;
    [SerializeField] private int maxSpawnAttempts = 100;

    private const int StartRoomIndex = 44;

    public void SpawnEnemies(Cell roomCell)
    {
        if (skeletonPrefab == null || bossPrefab == null)
            return;

        // Если комната обычная, спавним обычных скелетов
        if (roomCell.roomType == RoomType.Regular)
        {
            if (roomCell.cellList.Contains(StartRoomIndex))
                return;

            int enemiesCount = GetEnemiesCount(roomCell.roomShape);
            List<Vector2> spawnPositions = new List<Vector2>();
            List<Rect> roomRects = GetRoomRects(roomCell);

            for (int i = 0; i < enemiesCount; i++)
            {
                if (TryGetSpawnPosition(roomRects, spawnPositions, out Vector2 spawnPosition))
                {
                    GameObject enemy = Instantiate(skeletonPrefab, spawnPosition, Quaternion.identity, transform);

                    EnemyAi enemyAi = enemy.GetComponent<EnemyAi>();
                    if (enemyAi != null)
                        enemyAi.SetRoomBounds(roomRects);

                    spawnPositions.Add(spawnPosition);
                }
            }
        }
        // Если комната босса, спавним одного босса-скелета
        else if (roomCell.roomType == RoomType.Boss)
        {
            List<Rect> roomRects = GetRoomRects(roomCell);
            Vector2 spawnPosition = Vector2.zero;

            // Спавним босса примерно в центре комнаты
            if (roomRects.Count > 0)
            {
                Rect rect = roomRects[0]; // берём первую ячейку комнаты
                spawnPosition = new Vector2(
                    rect.center.x,
                    rect.center.y
                );
            }

            GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity, transform);

            EnemyAi bossAi = boss.GetComponent<EnemyAi>();
            if (bossAi != null)
                bossAi.SetRoomBounds(roomRects);
        }
    }

    private int GetEnemiesCount(RoomShape roomShape)
    {
        switch (roomShape)
        {
            case RoomShape.OneByOne:
                return 2;

            case RoomShape.OneByTwo:
            case RoomShape.TwoByOne:
                return 3;

            case RoomShape.TwoByTwo:
                return 6;

            case RoomShape.LShape:
                return 5;

            default:
                return 0;
        }
    }

    private bool TryGetSpawnPosition(List<Rect> roomRects, List<Vector2> usedPositions, out Vector2 spawnPosition)
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Rect rect = roomRects[Random.Range(0, roomRects.Count)];

            float x = Random.Range(rect.xMin + spawnPaddingX, rect.xMax - spawnPaddingX);
            float y = Random.Range(rect.yMin + spawnPaddingY, rect.yMax - spawnPaddingY);

            Vector2 candidate = new Vector2(x, y);

            bool tooClose = false;

            foreach (Vector2 usedPosition in usedPositions)
            {
                if (Vector2.Distance(candidate, usedPosition) < minDistanceBetweenEnemies)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                spawnPosition = candidate;
                return true;
            }
        }

        spawnPosition = Vector2.zero;
        return false;
    }

    private List<Rect> GetRoomRects(Cell roomCell)
    {
        List<Rect> rects = new List<Rect>();

        int minX = roomCell.cellList.Min(index => index % 10);
        int maxX = roomCell.cellList.Max(index => index % 10);

        int minY = roomCell.cellList.Min(index => index / 10);
        int maxY = roomCell.cellList.Max(index => index / 10);

        int widthCells = maxX - minX + 1;
        int heightCells = maxY - minY + 1;

        foreach (int index in roomCell.cellList)
        {
            int cellX = index % 10;
            int cellY = index / 10;

            float localX = ((cellX - minX) - (widthCells - 1) / 2f) * oneCellSize.x;
            float localY = ((maxY - cellY) - (heightCells - 1) / 2f) * oneCellSize.y;

            Vector2 cellCenter = (Vector2)transform.position + new Vector2(localX, localY);

            Rect rect = new Rect(
                cellCenter.x - oneCellSize.x / 2f,
                cellCenter.y - oneCellSize.y / 2f,
                oneCellSize.x,
                oneCellSize.y
            );

            rects.Add(rect);
        }

        return rects;
    }
}