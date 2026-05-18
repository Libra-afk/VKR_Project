using UnityEngine;

public class DestructibleStoneVisual : MonoBehaviour
{
    [SerializeField] private DestructibleStone destructibleStone;
    [SerializeField] private GameObject StoneDeathVFXPrefab;

    private GameObject itemPrefab; // сюда будем сохранять ExtraHp или ключ

    private void Start()
    {
        if (destructibleStone != null)
            destructibleStone.OnDestructibleTakeDamage += DestructibleStone_OnDestructibleTakeDamage;
    }

    private void DestructibleStone_OnDestructibleTakeDamage(object sender, System.EventArgs e)
    {
        ShowDeathVFX();

        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, destructibleStone.transform.position, Quaternion.identity);
            itemPrefab = null; // чтобы не спавнилось повторно
        }
    }

    private void ShowDeathVFX()
    {
        if (StoneDeathVFXPrefab != null)
            Instantiate(StoneDeathVFXPrefab, destructibleStone.transform.position, Quaternion.identity);
    }

    public void SetItemPrefab(GameObject prefab)
    {
        itemPrefab = prefab;
    }

    private void OnDestroy()
    {
        if (destructibleStone != null)
            destructibleStone.OnDestructibleTakeDamage -= DestructibleStone_OnDestructibleTakeDamage;
    }
}