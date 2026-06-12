using UnityEngine;

public class BossKeyPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            player.HasBossKey = true;
            Destroy(gameObject);
        }
    }
}