using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;

    // game over and victory UI
    public GameObject gameOverUI;
    public GameObject victoryUI;

    private bool isPaused = false;

    void Start()
    {
        pauseUI.SetActive(false);

        // hide game over / victory at start
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (victoryUI != null)
            victoryUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // don't allow pause if game is over or won
            if ((gameOverUI != null && gameOverUI.activeSelf) ||
                (victoryUI != null && victoryUI.activeSelf))
                return;

            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title Screen");
    }

    // =====================
    // GAME OVER
    // =====================
    public void GameOver()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // =====================
    // VICTORY
    // =====================
    public void Victory()
    {
        if (victoryUI != null)
            victoryUI.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // =====================
    // RESTART
    // =====================
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}