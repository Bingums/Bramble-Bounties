using UnityEngine;

[CreateAssetMenu(fileName = "BountyData", menuName = "Bounties/Bounty Data")]
public class BountyData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string bountyId;
    [SerializeField] private string displayName;
    [SerializeField] [TextArea(2, 5)] private string description;
    [SerializeField] private Sprite posterSprite;

    [Header("Boss Flow")]
    [SerializeField] private string minibossId;
    [SerializeField] private string minibossSceneName;

    [Header("Enemy Difficulty (Floors 1-5)")]
    [SerializeField] private float healthMultiplier = 1f;
    [SerializeField] private float attackMultiplier = 1f;
    [SerializeField] private float moveSpeedMultiplier = 1f;
    [SerializeField] private int extraEnemiesPerWave = 0;
    [SerializeField] private int extraWaves = 0;
    [SerializeField] private int bountyValue = 0;

    public string BountyId => bountyId;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite PosterSprite => posterSprite;

    public string MinibossId => minibossId;
    public string MinibossSceneName => minibossSceneName;

    public float HealthMultiplier => healthMultiplier;
    public float AttackMultiplier => attackMultiplier;
    public float MoveSpeedMultiplier => moveSpeedMultiplier;
    public int ExtraEnemiesPerWave => extraEnemiesPerWave;
    public int ExtraWaves => extraWaves;
    public int Bounty => bountyValue;
    

    private void OnValidate()
    {
        healthMultiplier = Mathf.Max(0.1f, healthMultiplier);
        attackMultiplier = Mathf.Max(0.1f, attackMultiplier);
        moveSpeedMultiplier = Mathf.Max(0.1f, moveSpeedMultiplier);
        extraEnemiesPerWave = Mathf.Max(0, extraEnemiesPerWave);
        extraWaves = Mathf.Max(0, extraWaves);
        bountyValue = Mathf.Max(0, Bounty);
    }
}
