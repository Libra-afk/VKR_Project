using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Image Bar;

    private Player player;

    private void Awake()
    {
        if (Bar != null)
            Bar.fillAmount = 1f;

        // Находим игрока автоматически, если он уже есть на сцене
        if (Player.Instance != null)
            player = Player.Instance;
    }

    private void Update()
    {
        // Если игрок ещё не присвоен, пробуем найти
        if (player == null)
        {
            if (Player.Instance != null)
            {
                player = Player.Instance;
            }
            else
            {
                return; // игрок ещё не появился
            }
        }

        if (Bar != null)
            Bar.fillAmount = player.CurrentHealth / player.MaxHealth;
    }
}