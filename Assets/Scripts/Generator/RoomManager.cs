using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private List<Room> createdRooms;

    [SerializeField] private float doorActivationDelay = 0.35f;
    private bool isTeleporting = false;

    [Header("Camera References")]
    [SerializeField] private CameraFollow cameraFollow;

    [Header("Offset Variables")]
    public float offsetX;
    public float offsetY;

    [Header("Prefab References")]
    public Room roomPrefab;
    public Door doorPrefab;

    [Header("Player References")]
    public GameObject playerPrefab;
    private GameObject currentPlayer;
    private Room currentActiveRoom;

    [Header("Scriptable Objects References")]
    public DoorScriptable[] doors;
    public RoomScriptable[] rooms;

    public static RoomManager instance;

    private int currentCameraCellIndex = -1;

    private const int StartRoomIndex = 44;

    private void Awake()
    {
        instance = this;
        createdRooms = new List<Room>();
    }
    private void Update()
    {
        UpdateCameraBoundsForActiveRoom();
    }

    private void UpdateCameraBoundsForActiveRoom()
    {
        if (currentPlayer == null || currentActiveRoom == null || cameraFollow == null)
            return;

        if (!currentActiveRoom.IsLShapeRoom())
            return;

        int newCameraCellIndex = currentActiveRoom.GetCameraCellIndexByPlayerPosition(currentPlayer.transform.position);

        if (newCameraCellIndex == currentCameraCellIndex)
            return;

        currentCameraCellIndex = newCameraCellIndex;

        cameraFollow.SetRoomBounds(
            currentActiveRoom.GetCameraCenterByPlayerPosition(currentPlayer.transform.position),
            currentActiveRoom.GetCameraSizeByPlayerPosition(currentPlayer.transform.position)
        );
    }

    public Room GetRoomByCellIndex(int cellIndex)
    {
        return createdRooms.FirstOrDefault(room => room.ContainsCellIndex(cellIndex));
    }

    public void SetupRooms(List<Cell> spawnedCells)
    {
        for (int i = createdRooms.Count - 1; i >= 0; i--)
        {
            Destroy(createdRooms[i].gameObject);
        }

        createdRooms.Clear();

        foreach (var currentCell in spawnedCells)
        {
            var foundRoom = rooms.FirstOrDefault(x =>
                x.roomShape == currentCell.roomShape &&
                x.roomType == currentCell.roomType &&
                DoesTileMatchCell(x.occupiedTiles, currentCell));

            if (foundRoom == null)
            {
                Debug.LogWarning($"Room Scriptable not found for cell index: {currentCell.index}, type: {currentCell.roomType}, shape: {currentCell.roomShape}");
                continue;
            }

            var currentPosition = currentCell.transform.position;

            var convertedPosition = new Vector2(
                currentPosition.x * offsetX,
                currentPosition.y * offsetY
            );

            var spawnedRoom = Instantiate(roomPrefab, convertedPosition, Quaternion.identity);

            spawnedRoom.SetupRoom(currentCell, foundRoom);

            createdRooms.Add(spawnedRoom);
        }

        SpawnPlayerInStartRoom(spawnedCells);
    }

    private void SpawnPlayerInStartRoom(List<Cell> spawnedCells)
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning("Player Prefab is not assigned in RoomManager.");
            return;
        }

        var startCell = spawnedCells.FirstOrDefault(cell => cell.cellList.Contains(StartRoomIndex));

        if (startCell == null)
        {
            Debug.LogWarning("Start room was not found.");
            return;
        }

        Vector2 spawnPosition = new Vector2(
            startCell.transform.position.x * offsetX,
            startCell.transform.position.y * offsetY
        );

        if (currentPlayer == null)
        {
            currentPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            currentPlayer.transform.position = spawnPosition;
        }

        Player player = currentPlayer.GetComponent<Player>();
        if (player != null)
        {
            player.ResetRunState();
        }


        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(currentPlayer.transform);

            cameraFollow.SetRoomBounds(
                spawnPosition,
                new Vector2(10.6f, 5.3f)
            );
        }
        Room startRoom = createdRooms.FirstOrDefault(room => room.ContainsCellIndex(StartRoomIndex));

        if (startRoom != null)
        {
            SetActiveRoom(startRoom);
        }
    }



    public bool DoesTileMatchCell(int[] occupiedTiles, Cell cell)
    {
        if (occupiedTiles.Length != cell.cellList.Count)
            return false;

        int minIndex = cell.cellList.Min();
        List<int> normalizedCell = new List<int>();

        foreach (int index in cell.cellList)
        {
            int dx = (index % 10) - (minIndex % 10);
            int dy = (index / 10) - (minIndex / 10);

            normalizedCell.Add(dy * 10 + dx);
        }

        normalizedCell.Sort();

        int[] sortedOccupied = (int[])occupiedTiles.Clone();
        Array.Sort(sortedOccupied);

        return normalizedCell.SequenceEqual(sortedOccupied);
    }

    public CameraFollow GetCameraFollow()
    {
        return cameraFollow;
    }

    public void TeleportPlayerByDoor(Transform player, Door sourceDoor)
    {
        if (isTeleporting)
            return;

        isTeleporting = true;

        Room targetRoom = createdRooms.FirstOrDefault(room => room.ContainsCellIndex(sourceDoor.TargetCellIndex));

        if (targetRoom == null)
        {
            Debug.LogWarning($"Target room was not found for cell index: {sourceDoor.TargetCellIndex}");
            isTeleporting = false;
            return;
        }

        Door pairDoor = targetRoom.GetPairDoor(sourceDoor);

        if (pairDoor == null)
        {
            Debug.LogWarning("Pair door was not found.");
            isTeleporting = false;
            return;
        }

        SetAllRoomsDoorsActive(false);

        Vector2 newPlayerPosition = GetPlayerPositionNearPairDoor(pairDoor, sourceDoor.Direction);
        player.position = newPlayerPosition;

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            playerRb.position = newPlayerPosition;
        }

        if (cameraFollow != null)
        {
            currentCameraCellIndex = targetRoom.GetCameraCellIndexByPlayerPosition(player.position);

            cameraFollow.SetRoomBounds(
                targetRoom.GetCameraCenterByPlayerPosition(player.position),
                targetRoom.GetCameraSizeByPlayerPosition(player.position)
            );
        }

        currentActiveRoom = targetRoom;

        StartCoroutine(EnableTargetRoomDoorsAfterDelay(targetRoom));
    }
    private Vector2 GetPlayerPositionNearPairDoor(Door pairDoor, EdgeDirection sourceDirection)
    {
        float enterOffset = 1.6f;

        Vector2 doorPosition = pairDoor.GetDoorWorldPosition();

        switch (sourceDirection)
        {
            case EdgeDirection.Up:
                return doorPosition + new Vector2(0f, enterOffset);

            case EdgeDirection.Down:
                return doorPosition + new Vector2(0f, -enterOffset);

            case EdgeDirection.Left:
                return doorPosition + new Vector2(-enterOffset, 0f);

            case EdgeDirection.Right:
                return doorPosition + new Vector2(enterOffset, 0f);

            default:
                return doorPosition;
        }
    }

    private IEnumerator EnableTargetRoomDoorsAfterDelay(Room targetRoom)
    {
        yield return new WaitForSeconds(doorActivationDelay);

        if (targetRoom != null)
        {
            targetRoom.SetDoorsActive(true);
        }

        isTeleporting = false;
    }

    private void SetActiveRoom(Room room)
    {
        currentActiveRoom = room;

        SetAllRoomsDoorsActive(false);

        if (currentActiveRoom != null)
        {
            currentActiveRoom.SetDoorsActive(true);
        }
    }

    private void SetAllRoomsDoorsActive(bool active)
    {
        foreach (Room room in createdRooms)
        {
            room.SetDoorsActive(active);
        }
    }

    
}