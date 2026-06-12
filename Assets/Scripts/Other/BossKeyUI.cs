using UnityEngine;
using UnityEngine.UI;

public class BossKeyUI : MonoBehaviour
{
    [SerializeField] private Image bossKeyIcon;

    private void Start()
    {
        bossKeyIcon.enabled = false;
    }

    private void Update()
    {
        if (Player.Instance == null || bossKeyIcon == null)
            return;

        bossKeyIcon.enabled = Player.Instance.HasBossKey;
    }
}