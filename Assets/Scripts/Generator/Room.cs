using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EdgeDirection
{
    Up,
    Down,
    Left,
    Right
}

public class Room : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public Cell currentCell;
    private List<Door> roomDoors = new List<Door>();

    private readonly Vector2 oneCellCameraSize = new Vector2(10.6f, 5.3f);

    public EnemyRoomSpawner enemyRoomSpawner;

    private void Awake()
    {
        enemyRoomSpawner = GetComponent<EnemyRoomSpawner>();
    }

    public void SetupRoom(Cell currentCell, RoomScriptable room)
    {
        this.currentCell = currentCell;
        roomDoors.Clear();

        Sprite selectedRoomSprite = room.roomVariantions[Random.Range(0, room.roomVariantions.Length)];
        spriteRenderer.sprite = selectedRoomSprite;

        SetupRoomColliders(selectedRoomSprite.name);

        if (currentCell.roomType == RoomType.Secret) return;

        var floorplan = MapGenerator.instance.getFloorPlan;
        var cellList = MapGenerator.instance.getSpawnedCells;

        switch (currentCell.roomShape)
        {
            case RoomShape.OneByOne:
                SetupOneByOne(currentCell, floorplan, cellList);
                break;
            case RoomShape.OneByTwo:
                SetupOneByTwo(currentCell, floorplan, cellList);
                break;
            case RoomShape.TwoByOne:
                SetupTwoByOne(currentCell, floorplan, cellList);
                break;
            case RoomShape.TwoByTwo:
                SetupTwoByTwo(currentCell, floorplan, cellList);
                break;
            case RoomShape.LShape:
                SetupLShapeRoom(currentCell, floorplan, cellList);
                break;

            default:
                break;
        }
        if (enemyRoomSpawner != null)
        {
            enemyRoomSpawner.SpawnEnemies(currentCell);
        }

    }

    public void SetupOneByOne(Cell cell, int[] floorplan, List<Cell> cellList)
    {
        var currentCell = cell.cellList[0];

        TryPlaceDoor(currentCell, new Vector2(0, 1.60f), EdgeDirection.Up, floorplan, cellList, cell);
        TryPlaceDoor(currentCell, new Vector2(0, -1.60f), EdgeDirection.Down, floorplan, cellList, cell);
        TryPlaceDoor(currentCell, new Vector2(-4.25f, 0), EdgeDirection.Left, floorplan, cellList, cell);
        TryPlaceDoor(currentCell, new Vector2(4.25f, 0), EdgeDirection.Right, floorplan, cellList, cell);
    }

    public void SetupOneByTwo(Cell cell, int[] floorplan, List<Cell> cellList)
    {
        var cellA = cell.cellList[0];
        var cellB = cell.cellList[1];

        TryPlaceDoor(cellA, new Vector2(0f, 4.245f), EdgeDirection.Up, floorplan, cellList, cell);
        TryPlaceDoor(cellA, new Vector2(-4.25f, 2.6125f), EdgeDirection.Left, floorplan, cellList, cell);
        TryPlaceDoor(cellA, new Vector2(4.25f, 2.6125f), EdgeDirection.Right, floorplan, cellList, cell);

        TryPlaceDoor(cellB, new Vector2(0, -4.245f), EdgeDirection.Down, floorplan, cellList, cell);
        TryPlaceDoor(cellB, new Vector2(-4.25f, -2.6125f), EdgeDirection.Left, floorplan, cellList, cell);
        TryPlaceDoor(cellB, new Vector2(4.25f, -2.6125f), EdgeDirection.Right, floorplan, cellList, cell);
    }

    public void SetupTwoByOne(Cell cell, int[] floorplan, List<Cell> cellList)
    {
        var cellA = cell.cellList[0];
        var cellB = cell.cellList[1];

        TryPlaceDoor(cellA, new Vector2(-5.3125f, 1.6f), EdgeDirection.Up, floorplan, cellList, cell);
        TryPlaceDoor(cellA, new Vector2(-9.6f, 0), EdgeDirection.Left, floorplan, cellList, cell);
        TryPlaceDoor(cellA, new Vector2(-5.3125f, -1.6f), EdgeDirection.Down, floorplan, cellList, cell);

        TryPlaceDoor(cellB, new Vector2(5.3125f, 1.6f), EdgeDirection.Up, floorplan, cellList, cell);
        TryPlaceDoor(cellB, new Vector2(5.3125f, -1.6f), EdgeDirection.Down, floorplan, cellList, cell);
        TryPlaceDoor(cellB, new Vector2(9.6f, 0), EdgeDirection.Right, floorplan, cellList, cell);
    }

    public void SetupTwoByTwo(Cell cell, int[] floorplan, List<Cell> cellList)
    {
        var cellA = cell.cellList[0];
        var cellB = cell.cellList[1];
        var cellC = cell.cellList[2];
        var cellD = cell.cellList[3];

        TryPlaceDoor(cellA, new Vector2(-5.3125f, 4.3f), EdgeDirection.Up, floorplan, cellList, cell);
        TryPlaceDoor(cellB, new Vector2(5.3125f, 4.3f), EdgeDirection.Up, floorplan, cellList, cell);

        TryPlaceDoor(cellA, new Vector2(-9.6f, 2.65f), EdgeDirection.Left, floorplan, cellList, cell);
        TryPlaceDoor(cellC, new Vector2(-9.6f, -2.65f), EdgeDirection.Left, floorplan, cellList, cell);

        TryPlaceDoor(cellC, new Vector2(-5.3125f, -4.3f), EdgeDirection.Down, floorplan, cellList, cell);
        TryPlaceDoor(cellD, new Vector2(5.3125f, -4.3f), EdgeDirection.Down, floorplan, cellList, cell);

        TryPlaceDoor(cellB, new Vector2(9.6f, 2.65f), EdgeDirection.Right, floorplan, cellList, cell);
        TryPlaceDoor(cellD, new Vector2(9.6f, -2.65f), EdgeDirection.Right, floorplan, cellList, cell);
    }
    public void SetupLShapeRoom(Cell cell, int[] floorplan, List<Cell> cellList)
    {
        var cellA = cell.cellList[0];
        var cellB = cell.cellList[1];
        var cellC = cell.cellList[2];

        if (cellA + 1 == cellB && cellA + 10 == cellC)
        {
            TryPlaceDoor(cellA, new Vector2(-5.3125f, 4.3f), EdgeDirection.Up, floorplan, cellList, cell);
            TryPlaceDoor(cellA, new Vector2(-9.6f, 2.6125f), EdgeDirection.Left, floorplan, cellList, cell);

            TryPlaceDoor(cellB, new Vector2(5.3125f, 4.3f), EdgeDirection.Up, floorplan, cellList, cell);
            TryPlaceDoor(cellB, new Vector2(9.6f, 2.6375f), EdgeDirection.Right, floorplan, cellList, cell);
            TryPlaceDoor(cellB, new Vector2(5.3125f, 1f), EdgeDirection.Down, floorplan, cellList, cell);

            TryPlaceDoor(cellC, new Vector2(-5.3125f, -4.3f), EdgeDirection.Down, floorplan, cellList, cell);
            TryPlaceDoor(cellC, new Vector2(-1f, -2.6375f), EdgeDirection.Right, floorplan, cellList, cell);
            TryPlaceDoor(cellC, new Vector2(-9.6f, -2.6125f), EdgeDirection.Left, floorplan, cellList, cell);
        }
        else if (cellA + 1 == cellB && cellB + 10 == cellC)
        {
            TryPlaceDoor(cellA, new Vector2(-5.3125f, 4.3f), EdgeDirection.Up, floorplan, cellList, cell);
            TryPlaceDoor(cellA, new Vector2(-9.6f, 2.6125f), EdgeDirection.Left, floorplan, cellList, cell);
            TryPlaceDoor(cellA, new Vector2(-5.3125f, 1f), EdgeDirection.Down, floorplan, cellList, cell);

            TryPlaceDoor(cellB, new Vector2(5.3125f, 4.3f), EdgeDirection.Up, floorplan, cellList, cell);
            TryPlaceDoor(cellB, new Vector2(9.6f, 2.6375f), EdgeDirection.Right, floorplan, cellList, cell);

            TryPlaceDoor(cellC, new Vector2(5.3125f, -4.3f), EdgeDirection.Down, floorplan, cellList, cell);
            TryPlaceDoor(cellC, new Vector2(9.6f, -2.6375f), EdgeDirection.Right, floorplan, cellList, cell);
            TryPlaceDoor(cellC, new Vector2(1, -2.6125f), EdgeDirection.Left, floorplan, cellList, cell);
        }
        else if (cellA + 10 == cellB)
        {
            TryPlaceDoor(cellA, new Vector2(-5.3125f, 4.3f), EdgeDirection.Up, floorplan, cellList, cell);
            TryPlaceDoor(cellA, new Vector2(-9.6f, 2.6125f), EdgeDirection.Left, floorplan, cellList, cell);
            TryPlaceDoor(cellA, new Vector2(-1, 2.6125f), EdgeDirection.Right, floorplan, cellList, cell);

            TryPlaceDoor(cellB, new Vector2(-5.3125f, -4.3f), EdgeDirection.Down, floorplan, cellList, cell);
            TryPlaceDoor(cellB, new Vector2(-9.6f, -2.6125f), EdgeDirection.Left, floorplan, cellList, cell);

            TryPlaceDoor(cellC, new Vector2(5.3125f, -1), EdgeDirection.Up, floorplan, cellList, cell);
            TryPlaceDoor(cellC, new Vector2(5.3125f, -4.3f), EdgeDirection.Down, floorplan, cellList, cell);
            TryPlaceDoor(cellC, new Vector2(9.6f, -2.6375f), EdgeDirection.Right, floorplan, cellList, cell);
        }
        else if (cellA + 10 == cellC)
        {
            TryPlaceDoor(cellA, new Vector2(5.3125f, 4.3f), EdgeDirection.Up, floorplan, cellList, cell);
            TryPlaceDoor(cellA, new Vector2(1, 2.6125f), EdgeDirection.Left, floorplan, cellList, cell);
            TryPlaceDoor(cellA, new Vector2(9.6f, 2.6125f), EdgeDirection.Right, floorplan, cellList, cell);

            TryPlaceDoor(cellB, new Vector2(-5.3125f, -1f), EdgeDirection.Up, floorplan, cellList, cell);
            TryPlaceDoor(cellB, new Vector2(-5.3125f, -4.3f), EdgeDirection.Down, floorplan, cellList, cell);
            TryPlaceDoor(cellB, new Vector2(-9.6f, -2.6125f), EdgeDirection.Left, floorplan, cellList, cell);

            TryPlaceDoor(cellC, new Vector2(5.3125f, -4.3f), EdgeDirection.Down, floorplan, cellList, cell);
            TryPlaceDoor(cellC, new Vector2(9.6f, -2.6375f), EdgeDirection.Right, floorplan, cellList, cell);
        }

    }

    private void TryPlaceDoor(int fromIndex, Vector2 positionOffset, EdgeDirection direction, int[] floorplan, List<Cell> cellList, Cell currentCell)
    {
        int neighbourIndex = fromIndex + GetOffset(direction);

        if (neighbourIndex < 0 || neighbourIndex >= floorplan.Length) return;

        if (floorplan[neighbourIndex] != 1) return;

        var foundCell = cellList.FirstOrDefault(x => x.cellList.Contains(neighbourIndex));

        if (foundCell == null) return;

        if (foundCell.roomType == RoomType.Secret) return;

        var door = Instantiate(RoomManager.instance.doorPrefab, transform);

        door.transform.position = (Vector2)transform.position + positionOffset;

        SetupDoor(door, direction, currentCell.roomType == RoomType.Regular ? foundCell.roomType : currentCell.roomType);

        door.SetupDoorTrigger(this, fromIndex, neighbourIndex, direction);
        roomDoors.Add(door);
    }

    private void SetupDoor(Door door, EdgeDirection direction, RoomType roomType)
    {
        var doorTypes = GetDoorOptions(roomType);

        switch (direction)
        {
            case EdgeDirection.Up:
                door.SetDoorSprite(doorTypes.upDoor);
                break;
            case EdgeDirection.Down:
                door.SetDoorSprite(doorTypes.downDoor);
                break;
            case EdgeDirection.Left:
                door.SetDoorSprite(doorTypes.leftDoor);
                break;
            case EdgeDirection.Right:
                door.SetDoorSprite(doorTypes.rightDoor);
                break;

            default:
                break;
        }
        /*door.EnableDoorCollider(direction)*/;
    }

    private DoorScriptable GetDoorOptions(RoomType roomType)
    {
        return RoomManager.instance.doors.FirstOrDefault(x => x.roomType == roomType);
    }

    private int GetOffset(EdgeDirection direction)
    {
        switch (direction)
        {
            case EdgeDirection.Up:
                return -10;
            case EdgeDirection.Down:
                return 10;
            case EdgeDirection.Right:
                return 1;
            case EdgeDirection.Left:
                return -1;
        }
        return 0;
    }

    public bool ContainsCellIndex(int cellIndex)
    {
        return currentCell != null && currentCell.cellList.Contains(cellIndex);
    }

    public Vector2 GetRoomCenter()
    {
        return transform.position;
    }

    public Vector2 GetCameraSize()
    {
        if (currentCell == null || currentCell.cellList.Count == 0)
            return oneCellCameraSize;

        int minX = currentCell.cellList.Min(index => index % 10);
        int maxX = currentCell.cellList.Max(index => index % 10);

        int minY = currentCell.cellList.Min(index => index / 10);
        int maxY = currentCell.cellList.Max(index => index / 10);

        int widthCells = maxX - minX + 1;
        int heightCells = maxY - minY + 1;

        return new Vector2(
            oneCellCameraSize.x * widthCells,
            oneCellCameraSize.y * heightCells
        );
    }

    public Vector2 GetCameraCenterForDoor(Door entryDoor)
    {
        if (currentCell != null && currentCell.roomShape == RoomShape.LShape)
        {
            return GetCellWorldCenter(entryDoor.FromCellIndex);
        }

        return GetRoomCenter();
    }

    public Vector2 GetCameraSizeForDoor(Door entryDoor)
    {
        if (currentCell != null && currentCell.roomShape == RoomShape.LShape)
        {
            return oneCellCameraSize;
        }

        return GetCameraSize();
    }

    public Door GetPairDoor(Door sourceDoor)
    {
        EdgeDirection oppositeDirection = GetOppositeDirection(sourceDoor.Direction);

        Door exactDoor = roomDoors.FirstOrDefault(door =>
            door.Direction == oppositeDirection &&
            door.TargetCellIndex == sourceDoor.FromCellIndex);

        if (exactDoor != null)
            return exactDoor;

        return roomDoors
            .Where(door => door.Direction == oppositeDirection)
            .OrderBy(door => Vector2.Distance(door.GetDoorWorldPosition(), sourceDoor.GetDoorWorldPosition()))
            .FirstOrDefault();
    }

    public void SetDoorsActive(bool active)
    {
        foreach (Door door in roomDoors)
        {
            door.SetDoorColliderActive(active);
        }
    }


    public Vector2 GetPlayerEntryPosition(int targetCellIndex, EdgeDirection enteredFromDirection)
    {
        Vector2 cellCenter = GetCellWorldCenter(targetCellIndex);

        float enterOffset = 1.4f;

        switch (enteredFromDirection)
        {
            case EdgeDirection.Up:
                return cellCenter + new Vector2(0f, -oneCellCameraSize.y / 2f + enterOffset);

            case EdgeDirection.Down:
                return cellCenter + new Vector2(0f, oneCellCameraSize.y / 2f - enterOffset);

            case EdgeDirection.Left:
                return cellCenter + new Vector2(oneCellCameraSize.x / 2f - enterOffset, 0f);

            case EdgeDirection.Right:
                return cellCenter + new Vector2(-oneCellCameraSize.x / 2f + enterOffset, 0f);

            default:
                return cellCenter;
        }
    }



    public Vector2 GetCameraCenterByCell(int targetCellIndex)
    {
        if (currentCell != null && currentCell.roomShape == RoomShape.LShape)
        {
            return GetCellWorldCenter(targetCellIndex);
        }

        return GetRoomCenter();
    }

    public Vector2 GetCameraSizeByCell(int targetCellIndex)
    {
        if (currentCell != null && currentCell.roomShape == RoomShape.LShape)
        {
            return oneCellCameraSize;
        }

        return GetCameraSize();
    }


    private Vector2 GetCellWorldCenter(int cellIndex)
    {
        if (currentCell == null || currentCell.cellList.Count == 0)
            return transform.position;

        int minX = currentCell.cellList.Min(index => index % 10);
        int maxX = currentCell.cellList.Max(index => index % 10);

        int minY = currentCell.cellList.Min(index => index / 10);
        int maxY = currentCell.cellList.Max(index => index / 10);

        int widthCells = maxX - minX + 1;
        int heightCells = maxY - minY + 1;

        int cellX = cellIndex % 10;
        int cellY = cellIndex / 10;

        float localX = ((cellX - minX) - (widthCells - 1) / 2f) * oneCellCameraSize.x;
        float localY = ((maxY - cellY) - (heightCells - 1) / 2f) * oneCellCameraSize.y;

        return (Vector2)transform.position + new Vector2(localX, localY);
    }

    private EdgeDirection GetOppositeDirection(EdgeDirection direction)
    {
        switch (direction)
        {
            case EdgeDirection.Up:
                return EdgeDirection.Down;

            case EdgeDirection.Down:
                return EdgeDirection.Up;

            case EdgeDirection.Left:
                return EdgeDirection.Right;

            case EdgeDirection.Right:
                return EdgeDirection.Left;

            default:
                return direction;
        }
    }

    public bool IsLShapeRoom()
    {
        return currentCell != null && currentCell.roomShape == RoomShape.LShape;
    }

    public int GetCameraCellIndexByPlayerPosition(Vector2 playerPosition)
    {
        if (currentCell == null || currentCell.cellList.Count == 0)
            return -1;

        int closestCellIndex = currentCell.cellList[0];
        float closestDistance = Vector2.Distance(playerPosition, GetCellWorldCenter(closestCellIndex));

        foreach (int cellIndex in currentCell.cellList)
        {
            float distance = Vector2.Distance(playerPosition, GetCellWorldCenter(cellIndex));

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCellIndex = cellIndex;
            }
        }

        return closestCellIndex;
    }

    public Vector2 GetCameraCenterByPlayerPosition(Vector2 playerPosition)
    {
        if (!IsLShapeRoom())
            return GetRoomCenter();

        int cameraCellIndex = GetCameraCellIndexByPlayerPosition(playerPosition);

        if (cameraCellIndex == -1)
            return GetRoomCenter();

        return GetCellWorldCenter(cameraCellIndex);
    }

    public Vector2 GetCameraSizeByPlayerPosition(Vector2 playerPosition)
    {
        if (IsLShapeRoom())
            return oneCellCameraSize;

        return GetCameraSize();
    }

    private void SetupRoomColliders(string spriteName)
    {
        DisableAllRoomColliders();

        string colliderPrefix = GetColliderPrefixBySpriteName(spriteName);

        foreach (Transform child in transform)
        {
            if (!child.name.StartsWith(colliderPrefix))
                continue;

            BoxCollider2D boxCollider = child.GetComponent<BoxCollider2D>();

            if (boxCollider != null)
            {
                boxCollider.enabled = true;
            }
        }
    }

    private void DisableAllRoomColliders()
    {
        foreach (Transform child in transform)
        {
            BoxCollider2D boxCollider = child.GetComponent<BoxCollider2D>();

            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
        }
    }

    private string GetColliderPrefixBySpriteName(string spriteName)
    {
        switch (spriteName)
        {
            case "1x1_room":
                return "1x1_Room";

            case "1x2_room":
                return "1x2_Room";

            case "2x1_room":
                return "2x1_Room";

            case "2x2_room":
                return "2x2_Room";

            case "l_shape_room":
                return "L";

            case "rotate_1":
                return "Rotate1";

            case "rotate_2":
                return "Rotate2";

            case "rotate_3":
                return "Rotate3";

            case "1x1_boss_room":
                return "1x1_Room";

            case "shop_room":
                return "1x1_Room";

            case "treasure_room":
                return "1x1_Room";

            case "1x1_secret_room":
                return "1x1_Room";

            default:
                Debug.LogWarning($"No room collider prefix found for sprite: {spriteName}");
                return "";
        }
    }

    public bool HasEnemies()
    {
        EnemyEntity[] enemies = GetComponentsInChildren<EnemyEntity>();
        foreach (var enemy in enemies)
        {
            if (enemy.IsAlive)
                return true;
        }
        return false;
    }

}
