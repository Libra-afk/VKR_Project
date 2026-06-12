using UnityEngine;

public class SecretRoomSpawner : MonoBehaviour
{
    [SerializeField] private GameObject extraHpPrefab;

    [SerializeField] private float spawnRangeX = 3f;
    [SerializeField] private float spawnRangeY = 2f;

    [SerializeField] private Room room;

    private bool spawned = false;

    private void Awake()
    {
        if (room == null)
            room = GetComponent<Room>();
    }

    private void Update()
    {
        if (spawned)
            return;

        if (room == null || room.currentCell == null)
            return;

        if (room.currentCell.roomType != RoomType.Secret)
            return;

        SpawnHearts();

        spawned = true;
    }

    private void SpawnHearts()
    {
        Vector2[] positions =
        {
            (Vector2)transform.position + new Vector2(-2f, 0f),
            (Vector2)transform.position + new Vector2(0f, 0f),
            (Vector2)transform.position + new Vector2(2f, 0f)
        };

        foreach (Vector2 pos in positions)
        {
            Instantiate(extraHpPrefab, pos, Quaternion.identity);
        }
    }
}