using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 8f;

    [Header("Room Bounds")]
    [SerializeField] private Vector2 roomCenter;
    [SerializeField] private Vector2 roomSize;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position;
        targetPosition.z = transform.position.z;

        // Ограничиваем внутри комнаты
        Vector3 clampedPosition = ClampToRoomBounds(targetPosition);

        // Камера сразу ставится в позицию игрока
        transform.position = clampedPosition;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetRoomBounds(Vector2 center, Vector2 size)
    {
        roomCenter = center;
        roomSize = size;
    }

    private Vector3 ClampToRoomBounds(Vector3 targetPosition)
    {
        float camHeight = _camera.orthographicSize;
        float camWidth = camHeight * _camera.aspect;

        float minX = roomCenter.x - roomSize.x / 2f + camWidth;
        float maxX = roomCenter.x + roomSize.x / 2f - camWidth;
        float minY = roomCenter.y - roomSize.y / 2f + camHeight;
        float maxY = roomCenter.y + roomSize.y / 2f - camHeight;

        float clampedX = minX > maxX
            ? roomCenter.x
            : Mathf.Clamp(targetPosition.x, minX, maxX);

        float clampedY = minY > maxY
            ? roomCenter.y
            : Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(clampedX, clampedY, targetPosition.z);
    }
}