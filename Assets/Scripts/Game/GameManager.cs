using System;
using System.Collections.Generic;
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

    [Header("Bounties")] [SerializeField] private BountyData[] availableBounties;
    [SerializeField] private int offeredBountyCount = 3;
    [SerializeField] private BountyData finalBossBounty;

    [Header("Player Stats")] [SerializeField]
    private float startingMaxHealth = 100f;

    [SerializeField] private float startingHealth = 100f;
    [SerializeField] private float startingMaxStamina = 10f;
    [SerializeField] private float startingStamina = 10f;
    [SerializeField] private int startingDamageMultiplier = 1;

    [Header("Run Progress")] [SerializeField]
    private int currentFloor = 1;

    [SerializeField] private int finalFloor = 5;
    [SerializeField] private int minibossFloor = 6;
    
    public BountyRunState BountyRunState { get; private set; }
    public PlayerState PlayerState { get; private set; }
    public int CurrentFloor => currentFloor;
    public int FinalFloor => finalFloor;
    public int MinibossFloor => minibossFloor;
    public int Score { get; private set; }
    
    public event Action<int> OnScoreChanged;
    public event Action OnBountyOfferChanged;
    public event Action<BountyData> OnOpeningBountySelected;
    public event Action OnFinalBossUnlocked;
    public event Action<BountyData> OnFinalBossSelected;


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
        InitializePlayerState();
        InitializeBountyRunState();
    }

    private void InitializePlayerState()
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

    private void InitializeBountyRunState()
    {
        if (BountyRunState != null)
        {
            return;
        }

        BountyRunState = new BountyRunState();
        ResetScore();
        GenerateBountyOffers();
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
        
        BountyRunState = new BountyRunState();
        GenerateBountyOffers();
    }

    public void GenerateBountyOffers()
    {
        if (availableBounties == null || availableBounties.Length == 0)
        {
            BountyRunState.SetOfferedBounties(Array.Empty<BountyData>());
            OnBountyOfferChanged?.Invoke();
            return;
        }
        
        List<BountyData> pool = new List<BountyData>(availableBounties);
        List<BountyData> selected = new List<BountyData>();
        
        int count = Mathf.Min(offeredBountyCount, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, pool.Count);
            selected.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }
        
        BountyRunState.SetOfferedBounties(selected.ToArray());
        OnBountyOfferChanged?.Invoke();
    }

    public void SelectOpeningBounty(BountyData bounty)
    {
        if (BountyRunState == null)
        {
            return;
        }
        
        BountyRunState.SelectOpeningBounty(bounty);
        OnOpeningBountySelected?.Invoke(bounty);
    }

    public void MarkMinibossDefeated()
    {
        if (BountyRunState == null)
        {
            return;
        }
        
        BountyRunState.MarkMinibossDefeated();
        OnFinalBossUnlocked?.Invoke();
    }

    public void SelectFinalBoss(BountyData bounty)
    {
        if (BountyRunState == null)
        {
            return;
        }
        
        BountyRunState.SelectFinalBoss(bounty, IsFinalFloor());
        OnFinalBossSelected?.Invoke(bounty);
    }

    public bool IsEarlyBountyFloor()
    {
        return currentFloor >= 1 && currentFloor < minibossFloor;
    }

    public bool IsMinibossFloor()
    {
        return currentFloor == minibossFloor;
    }

    public bool IsFinalFloor()
    {
        return currentFloor == finalFloor;
    }

    public bool IsFinalBossUnlocked()
    {
        return BountyRunState != null && BountyRunState.FinalBossUnlocked;
    }

    public BountyData GetSelectedBounty()
    {
        return BountyRunState != null ? BountyRunState.SelectedBounty : null;
    }

    public BountyData GetSelectedFinalBoss()
    {
        return BountyRunState != null ? BountyRunState.SelectedFinalBoss : null;
    }

    public BountyData GetActiveBounty()
    {
        if (BountyRunState == null)
        {
            return null;
        }

        if (IsFinalFloor() && BountyRunState.SelectedFinalBoss != null)
        {
            return BountyRunState.SelectedFinalBoss;
        }

        return BountyRunState.SelectedBounty;
    }

    public BountyData GetFinalBossBounty()
    {
        if (finalBossBounty != null)
        {
            return finalBossBounty;
        }

        if (availableBounties == null)
        {
            return null;
        }

        foreach (BountyData bounty in availableBounties)
        {
            if (bounty != null && bounty.BountyId == "final")
            {
                return bounty;
            }
        }

        return null;
    }

    public BountyData[] GetOfferedBounties()
    {
        return BountyRunState != null ? BountyRunState.OfferedBounties : Array.Empty<BountyData>();
    }

    public void AdvanceFloor()
    {
        currentFloor++;
    }

    public void DebugSetCurrentFloor(int floor)
    {
        currentFloor = Mathf.Max(1, floor);
        GenerateBountyOffers();
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
    
    public void AddScore(int points)
    {
        Score += points;
        OnScoreChanged?.Invoke(Score);
    }

    public void ResetScore()
    {
        Score = 0;
        OnScoreChanged?.Invoke(Score);
    }
}
