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
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    [Header("Player Stats")]
    [SerializeField] private float startingMaxHealth = 100f;
    [SerializeField] private float startingHealth = 100f;
    [SerializeField] private float startingMaxStamina = 10f;
    [SerializeField] private float startingStamina = 10f;
    [SerializeField] private int startingDamageMultiplier = 1;

    [SerializeField] private int currentFloor = 1;

    public PlayerState PlayerState { get; private set; }
    public int CurrentFloor => currentFloor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeRunState();
    }

    private void Start()
    {
        CurrentState = GameState.Playing;
    }

    private void InitializeRunState()
    {
        if (PlayerState != null)
        {
            return;
        }

        PlayerState = new PlayerState(
            startingHealth,
            startingMaxHealth,
            startingStamina,
            startingMaxStamina,
            startingDamageMultiplier
        );
    }

    public void StartNewRun()
    {
        currentFloor = 1;
        CurrentState = GameState.Playing;

        PlayerState = new PlayerState(
            startingHealth,
            startingMaxHealth,
            startingStamina,
            startingMaxStamina,
            startingDamageMultiplier
        );
    }

    public void AdvanceFloor()
    {
        currentFloor++;
    }

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
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
