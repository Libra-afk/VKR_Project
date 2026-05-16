using UnityEngine;

public class DestructibleStoneVisual : MonoBehaviour
{
    [SerializeField] private DestructibleStone destructibleStone;
    [SerializeField] private GameObject StoneDeathVFXPrefab;

    private void Start()
    {
        destructibleStone.OnDestructibleTakeDamage += DestructibleStone_OnDestructibleTakeDamage;
    }

    private void DestructibleStone_OnDestructibleTakeDamage(object sender, System.EventArgs e)
    {
        ShowDeathVFX();
    }

    private void ShowDeathVFX()
    {
        Instantiate(StoneDeathVFXPrefab, destructibleStone.transform.position, Quaternion.identity);
    }

    private void OnDestroy()
    {
        destructibleStone.OnDestructibleTakeDamage -= DestructibleStone_OnDestructibleTakeDamage;
    }
}
