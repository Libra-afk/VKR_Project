using UnityEngine;

public class Door : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Header("Door Trigger Colliders")]
    [SerializeField] private BoxCollider2D doorTopCollider;
    [SerializeField] private BoxCollider2D doorBottomCollider;
    [SerializeField] private BoxCollider2D doorLeftCollider;
    [SerializeField] private BoxCollider2D doorRightCollider;

    private BoxCollider2D activeCollider;

    public Room OwnerRoom { get; private set; }
    public int FromCellIndex { get; private set; }
    public int TargetCellIndex { get; private set; }
    public EdgeDirection Direction { get; private set; }




    private void Awake()
    {
        DisableAllDoorColliders();
    }

    public void SetDoorSprite(Sprite door)
    {
        spriteRenderer.sprite = door;
        spriteRenderer.enabled = true;
    }

    public void SetupDoorTrigger(Room ownerRoom, int fromCellIndex, int targetCellIndex, EdgeDirection direction)
    {
        OwnerRoom = ownerRoom;
        FromCellIndex = fromCellIndex;
        TargetCellIndex = targetCellIndex;
        Direction = direction;

        activeCollider = GetColliderByDirection(direction);

        if (activeCollider == null)
            return;

        DoorTrigger doorTrigger = activeCollider.GetComponent<DoorTrigger>();

        if (doorTrigger == null)
            doorTrigger = activeCollider.gameObject.AddComponent<DoorTrigger>();

        doorTrigger.Setup(this);
    }

    public void SetDoorColliderActive(bool active)
    {
        DisableAllDoorColliders();

        if (active && activeCollider != null)
            activeCollider.enabled = true;
    }

    public Vector2 GetDoorWorldPosition()
    {
        if (activeCollider != null)
            return activeCollider.transform.position;

        return transform.position;
    }

    public void TeleportPlayer(Transform player)
    {
        RoomManager.instance.TeleportPlayerByDoor(player, this);
    }

    private BoxCollider2D GetColliderByDirection(EdgeDirection direction)
    {
        switch (direction)
        {
            case EdgeDirection.Up: return doorTopCollider;
            case EdgeDirection.Down: return doorBottomCollider;
            case EdgeDirection.Left: return doorLeftCollider;
            case EdgeDirection.Right: return doorRightCollider;
            default: return null;
        }
    }

    public bool IsSecretDoor { get; private set; }
    public bool IsRevealed { get; private set; } = true;

    public void MakeSecretDoor()
    {
        IsSecretDoor = true;
        IsRevealed = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    public void RevealSecretDoor()
    {
        if (!IsSecretDoor || IsRevealed)
            return;

        IsRevealed = true;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
    }

    private void DisableAllDoorColliders()
    {
        if (doorTopCollider != null) doorTopCollider.enabled = false;
        if (doorBottomCollider != null) doorBottomCollider.enabled = false;
        if (doorLeftCollider != null) doorLeftCollider.enabled = false;
        if (doorRightCollider != null) doorRightCollider.enabled = false;
    }
}