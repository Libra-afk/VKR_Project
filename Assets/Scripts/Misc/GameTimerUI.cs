using TMPro;
using UnityEngine;

public class GameTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float elapsedTime;

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        int hours = Mathf.FloorToInt(elapsedTime / 3600);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        if (hours > 0)
        {
            timerText.text = $"{hours:00}:{minutes:00}:{seconds:00}";
        }
        else
        {
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}