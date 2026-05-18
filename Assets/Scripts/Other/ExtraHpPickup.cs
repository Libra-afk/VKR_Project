using UnityEngine;

public class ExtraHpPickup : MonoBehaviour
{
    public float healPercent = 0.2f; // 20% здоровья

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player._currentHealth += player._maxHealth * healPercent;
            player._currentHealth = Mathf.Clamp(player._currentHealth, 0, player._maxHealth);

            Destroy(gameObject); // убираем сердечко после подбора
        }
    }
}