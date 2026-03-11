using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public GameState CurrentState;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentState = GameState.Playing;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Quit()
    {
        Application.Quit();
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
