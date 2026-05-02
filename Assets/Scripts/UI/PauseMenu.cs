using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;        // Drag your entire PauseMenu panel here
    private bool isPaused = false;

    void Start()
    {
        // Make sure the pause menu starts hidden
        if (pauseUI != null)
            pauseUI.SetActive(false);
        
        // Lock cursor at start (typical for first-person games)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Toggle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        Debug.Log("Pausing game");
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
        Time.timeScale = 1f; // Very important: reset time before loading
        SceneManager.LoadScene("Title Scene"); // Make sure the scene name matches exactly
    }
}