using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private Door door;

    public void Setup(Door newDoor)
    {
        door = newDoor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Sword>())
        {
            if (door.IsSecretDoor)
            {
                RevealDoorPair();
            }

            return;
        }

        Player player = collision.GetComponentInParent<Player>();
        if (player == null && !collision.CompareTag("Player"))
            return;

        if (door.IsSecretDoor && !door.IsRevealed)
            return;

        if (door.OwnerRoom.HasEnemies())
            return;

        Room targetRoom = RoomManager.instance.GetRoomByCellIndex(door.TargetCellIndex);

        // Для входа в комнату босса нужен ключ
        if (targetRoom != null && targetRoom.currentCell.roomType == RoomType.Boss)
        {
            if (!player.HasBossKey)
                return;
        }

        Transform playerTransform = player != null ? player.transform : collision.transform;
        door.TeleportPlayer(playerTransform);
    }

    private void RevealDoorPair()
    {
        door.RevealSecretDoor();

        Room targetRoom = RoomManager.instance.GetRoomByCellIndex(door.TargetCellIndex);

        if (targetRoom == null)
            return;

        Door pairDoor = targetRoom.GetPairDoor(door);

        if (pairDoor != null)
            pairDoor.RevealSecretDoor();
    }
}