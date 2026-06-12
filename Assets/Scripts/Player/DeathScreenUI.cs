using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private Player subscribedPlayer;

    private void Awake()
    {
        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    private void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);

        StartCoroutine(WaitForPlayerAndSubscribe());
    }

    private IEnumerator WaitForPlayerAndSubscribe()
    {
        while (Player.Instance == null)
        {
            yield return null;
        }

        subscribedPlayer = Player.Instance;
        subscribedPlayer.OnPlayerDeath += Player_OnPlayerDeath;
    }

    private void Player_OnPlayerDeath(object sender, System.EventArgs e)
    {
        StartCoroutine(ShowDeathScreenAfterDelay());
    }

    private IEnumerator ShowDeathScreenAfterDelay()
    {
        yield return new WaitForSeconds(0.9f);

        deathScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        if (subscribedPlayer != null)
            subscribedPlayer.OnPlayerDeath -= Player_OnPlayerDeath;
    }
}