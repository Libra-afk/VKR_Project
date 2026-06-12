using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreenUI : MonoBehaviour
{
    public static VictoryScreenUI Instance { get; private set; }

    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button mainMenuButton;

    private EnemyEntity currentBoss;
    private bool victoryShown = false;

    private void Awake()
    {
        Instance = this;

        if (victoryScreen != null)
            victoryScreen.SetActive(false);
    }

    private void Start()
    {
        nextLevelButton.onClick.AddListener(NextLevel);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    public void RegisterBoss(EnemyEntity boss)
    {
        if (boss == null)
            return;

        if (currentBoss != null)
            currentBoss.OnDeath -= Boss_OnDeath;

        currentBoss = boss;
        currentBoss.OnDeath += Boss_OnDeath;

        victoryShown = false;
    }

    private void Boss_OnDeath(object sender, System.EventArgs e)
    {
        if (victoryShown)
            return;

        victoryShown = true;
        StartCoroutine(ShowVictoryScreenAfterDelay());
    }

    private IEnumerator ShowVictoryScreenAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        victoryScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    private void NextLevel()
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
        if (currentBoss != null)
            currentBoss.OnDeath -= Boss_OnDeath;
    }
}