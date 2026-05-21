using System.Collections;
using UnityEngine;

public class Pimp : EnemyController
{
    private enum PimpState
    {
        Approaching,
        Holding,
        Retreating,
        Summoning
    }

    [Header("Shooting")]
    [SerializeField] private float firingDistance = 7f;
    [SerializeField] private float minimumDistance = 3.5f;
    [SerializeField] private float shotCooldown = 0.5f;
    [SerializeField] private bool shootWhileRetreating = true;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private AudioClip plasmaGunSFX;

    [Header("Movement")]
    [SerializeField] private float approachSpeed = 2f;
    [SerializeField] private float retreatSpeed = 2f;
    [SerializeField] private float wallCheckDistance = 0.75f;
    [SerializeField] private float wallCheckRadius = 0.25f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Summoning")]
    [SerializeField] private GameObject[] minionPrefabs;
    [SerializeField] private Transform[] minionSpawnPoints;
    [SerializeField] private float summonCooldown = 8f;
    [SerializeField] private int minionsPerSummon = 2;
    [SerializeField] private float summonDuration = 0.75f;
    [SerializeField] private bool canShootWhileSummoning = false;

    private PimpState state;
    private float shotTimer;
    private float summonTimer;
    private bool isSummoning;

    private SpriteRenderer shooterRenderer;
    private Color shooterColor;
    private readonly Color damageColor = new Color(0.85f, 0.24f, 0.24f);

    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();

        shooterRenderer = GetComponent<SpriteRenderer>();
        if (shooterRenderer != null)
            shooterColor = shooterRenderer.color;

        audioSource = GetComponent<AudioSource>();
        scoreValue = 2000;
    }

    protected override void Update()
    {
        if (isDefeated || target == null)
            return;

        distance = Vector2.Distance(transform.position, target.position);

        shotTimer -= Time.deltaTime;
        summonTimer -= Time.deltaTime;

        ChooseState();
        HandleState();

        animator.SetFloat("TargetX", moveDirection.x);
        animator.SetBool("isMoving", rb.linearVelocity.magnitude > 0.1f);
    }

    private void ChooseState()
    {
        if (isSummoning)
        {
            state = PimpState.Summoning;
            return;
        }

        if (ShouldSummon())
        {
            state = PimpState.Summoning;
            StartCoroutine(SummonRoutine());
            return;
        }

        if (distance <= minimumDistance)
        {
            state = PimpState.Retreating;
            return;
        }

        if (distance <= firingDistance)
        {
            state = PimpState.Holding;
            return;
        }

        state = PimpState.Approaching;
    }

    private void HandleState()
    {
        switch (state)
        {
            case PimpState.Approaching:
                MoveTowardPlayer();
                TryShoot();
                break;

            case PimpState.Holding:
                StopMoving();
                TryShoot();
                break;

            case PimpState.Retreating:
                RetreatFromPlayer();

                if (shootWhileRetreating)
                    TryShoot();

                break;

            case PimpState.Summoning:
                StopMoving();

                if (canShootWhileSummoning)
                    TryShoot();

                break;
        }
    }

    private void MoveTowardPlayer()
    {
        moveDirection = (target.position - transform.position).normalized;
        moveSpeed = approachSpeed;
    }

    private void StopMoving()
    {
        moveDirection = Vector2.zero;
        moveSpeed = 0f;
    }

    private void RetreatFromPlayer()
    {
        Vector2 awayFromPlayer = ((Vector2)transform.position - (Vector2)target.position).normalized;
        Vector2 safeDirection = GetSafeRetreatDirection(awayFromPlayer);

        moveDirection = safeDirection;
        moveSpeed = safeDirection == Vector2.zero ? 0f : retreatSpeed;
    }

    private Vector2 GetSafeRetreatDirection(Vector2 preferredDirection)
    {
        if (!DirectionBlocked(preferredDirection))
            return preferredDirection;

        Vector2 leftStrafe = new Vector2(-preferredDirection.y, preferredDirection.x);
        if (!DirectionBlocked(leftStrafe))
            return leftStrafe;

        Vector2 rightStrafe = new Vector2(preferredDirection.y, -preferredDirection.x);
        if (!DirectionBlocked(rightStrafe))
            return rightStrafe;

        return Vector2.zero;
    }

    private bool DirectionBlocked(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return true;

        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position,
            wallCheckRadius,
            direction,
            wallCheckDistance,
            obstacleMask
        );

        return hit.collider != null;
    }

    private void TryShoot()
    {
        if (bullet == null || bulletSpawnPoint == null)
            return;

        if (distance > firingDistance)
            return;

        if (shotTimer > 0f)
        {
            animator.SetTrigger("waiting");
            return;
        }

        shotTimer = shotCooldown;

        animator.SetTrigger("attacking");
        Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        if (audioSource != null && plasmaGunSFX != null)
            audioSource.PlayOneShot(plasmaGunSFX);
    }

    private bool ShouldSummon()
    {
        if (summonTimer > 0f)
            return false;

        if (minionPrefabs == null || minionPrefabs.Length == 0)
            return false;

        if (minionSpawnPoints == null || minionSpawnPoints.Length == 0)
            return false;

        return distance <= firingDistance;
    }

    private IEnumerator SummonRoutine()
    {
        isSummoning = true;
        summonTimer = summonCooldown;

        animator.SetTrigger("summoning");

        yield return new WaitForSeconds(summonDuration);

        SpawnMinions();

        isSummoning = false;
    }

    private void SpawnMinions()
    {
        Room currentRoom = EnemySpawnManager.Instance != null
            ? EnemySpawnManager.Instance.GetCurrentRoom()
            : null;

        int spawned = 0;

        for (int i = 0; i < minionSpawnPoints.Length && spawned < minionsPerSummon; i++)
        {
            Transform spawnPoint = minionSpawnPoints[i];

            if (spawnPoint == null)
                continue;

            GameObject minionPrefab = minionPrefabs[Random.Range(0, minionPrefabs.Length)];

            if (minionPrefab == null)
                continue;

            GameObject spawnedMinion = Instantiate(
                minionPrefab,
                spawnPoint.position,
                Quaternion.identity
            );

            EnemyController enemy = spawnedMinion.GetComponent<EnemyController>();

            if (enemy != null && GameManager.Instance != null)
            {
                BountyData bounty = GameManager.Instance.GetActiveBounty();

                if (bounty != null)
                {
                    enemy.ScaleStats(
                        bounty.HealthMultiplier,
                        bounty.AttackMultiplier,
                        bounty.MoveSpeedMultiplier
                    );
                }
            }

            if (currentRoom != null)
                currentRoom.IncreaseEnemyCounts();

            spawned++;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") ||
            collision.CompareTag("PlayerProjectile") ||
            collision.CompareTag("Weapon"))
        {
            if (collision.CompareTag("Player"))
                collision.GetComponentInParent<playerController>().TakeDamage(attack);

            if (shooterRenderer != null)
                shooterRenderer.color = damageColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        StartCoroutine(Recolor(0.2f));
    }

    private IEnumerator Recolor(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (shooterRenderer != null)
            shooterRenderer.color = shooterColor;
    }
}
