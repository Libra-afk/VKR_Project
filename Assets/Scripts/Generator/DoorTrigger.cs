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
        Player player = collision.GetComponentInParent<Player>();
        if (player == null && !collision.CompareTag("Player"))
            return;

        // Если в комнате есть живые враги – не выпускаем
        if (door.OwnerRoom.HasEnemies())
            return;

        Transform playerTransform = player != null ? player.transform : collision.transform;
        door.TeleportPlayer(playerTransform);
    }
}