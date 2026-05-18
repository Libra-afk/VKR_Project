using UnityEngine;

public class RoomItemSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private GameObject extraHpPrefab;
    [SerializeField] private GameObject bossKeyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int rocksCount = 3;
    [SerializeField] private Vector2 spawnRangeX = new Vector2(-3f, 3f);
    [SerializeField] private Vector2 spawnRangeY = new Vector2(-2f, 2f);

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
        // Только для нужных комнат
        if (room.currentCell.roomType != RoomType.Item &&
            room.currentCell.roomType != RoomType.Shop)
            return;

        int bonusIndex = Random.Range(0, rocksCount);

        for (int i = 0; i < rocksCount; i++)
        {
            float x = Random.Range(spawnRangeX.x, spawnRangeX.y);
            float y = Random.Range(spawnRangeY.x, spawnRangeY.y);
            Vector2 spawnPos = (Vector2)room.transform.position + new Vector2(x, y);

            GameObject rock = Instantiate(rockPrefab, spawnPos, Quaternion.identity);

            if (i == bonusIndex)
            {
                DestructibleStoneVisual visual = rock.GetComponent<DestructibleStoneVisual>();
                if (visual != null)
                {
                    GameObject itemToSpawn = (Random.value < 0.5f) ? extraHpPrefab : bossKeyPrefab;
                    visual.SetItemPrefab(itemToSpawn);
                }
            }
        }
    }
}