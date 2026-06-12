using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;

    private bool isPaused = false;

    private void Awake()
    {
        if (pauseScreen != null)
            pauseScreen.SetActive(false);
    }

    private void Start()
    {
        continueButton.onClick.AddListener(ContinueGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (DeathScreenIsActive() || VictoryScreenIsActive())
                return;

            if (isPaused)
                ContinueGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    private void ContinueGame()
    {
        isPaused = false;
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private bool DeathScreenIsActive()
    {
        DeathScreenUI deathScreenUI = GetComponent<DeathScreenUI>();

        if (deathScreenUI == null)
            return false;

        return Time.timeScale == 0f && !isPaused;
    }

    private bool VictoryScreenIsActive()
    {
        VictoryScreenUI victoryScreenUI = GetComponent<VictoryScreenUI>();

        if (victoryScreenUI == null)
            return false;

        return Time.timeScale == 0f && !isPaused;
    }
}