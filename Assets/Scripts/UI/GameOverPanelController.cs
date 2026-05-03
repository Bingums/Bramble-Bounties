using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanelController : MonoBehaviour
{
    public void QuitToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title Screen");
    }
}
