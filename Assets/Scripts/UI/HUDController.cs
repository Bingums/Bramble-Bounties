using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
    [Header("Bar Fills")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;
    [SerializeField] private GameObject bossHealthRoot;
    [SerializeField] private Image bossHealthFill;

    [Header("Weapon UI")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private playerController player;
    [SerializeField] private Slider reloadBar;
    [SerializeField] private TMP_Text reloadText;
    [SerializeField] private TMP_Text curAmmoText;
    [SerializeField] private TMP_Text ammoReservesText;
    
    [Header("Augment UI")]
    [SerializeField] private GameObject augmentMenu;
    [SerializeField] public Image blockerImage;
    [SerializeField] public Image infoImage;
    [SerializeField] public TMP_Text infoName;
    [SerializeField] public TMP_Text infoText;
    [SerializeField] public TMP_Text pickupPrompt;
    
    [Header("Wave UI")]
    [SerializeField] public TMP_Text numWaveText;
    [SerializeField] public TMP_Text numEnemiesText;
    
    [Header("Score UI")]
    [SerializeField] public TMP_Text scoreText;

    [Header("Run UI")]
    [SerializeField] private TMP_Text floorLabelText;
    [SerializeField] private int maxFloor = 5;
    
    [Header("Game Menu UI")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject victoryMenu; // doesnt exist rn
    
    [Header("Alerts")]
    [SerializeField] public TMP_Text incomingWaveText;
    [SerializeField] public TMP_Text waveCountdownText;
    [SerializeField] public TMP_Text roomClearedText;
    [SerializeField] private TMP_Text noAmmoText;
    [SerializeField] public TMP_Text fullInventoryText;
    
    private PlayerState boundState;
    private WeaponData displayedWeapon;
    private playerCombat combatScript;
    public EquipSlot[] equippedSlots;
    public AugmentSlot[] inventorySlots;
    private EnemyController trackedBoss;
    private Coroutine bossHealthIntroRoutine;

    private bool menuOpen = false;
    private bool isPaused = false;
    
    private void OnEnable()
    {
        TryInitialize();
    }

    private void OnDisable()
    {
        UnbindState();
        UnbindPlayer();
        HideBossHealth();
        displayedWeapon = null;
    }
    
    private void Start()
    {
        reloadBar.gameObject.SetActive(false);
        reloadText.gameObject.SetActive(false);
        curAmmoText.gameObject.SetActive(false);
        ammoReservesText.gameObject.SetActive(false);
        noAmmoText.gameObject.SetActive(false);
        bossHealthRoot.SetActive(false);
        
        fullInventoryText.gameObject.SetActive(false);
        augmentMenu.gameObject.SetActive(false);
        blockerImage.gameObject.SetActive(false);
        
        numWaveText.gameObject.SetActive(false);
        numEnemiesText.gameObject.SetActive(false);
        incomingWaveText.gameObject.SetActive(false);
        waveCountdownText.gameObject.SetActive(false);
        roomClearedText.gameObject.SetActive(false);
        
        pickupPrompt.gameObject.SetActive(false);
        
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        if(victoryMenu != null) victoryMenu.SetActive(false); // doesnt exist rn
        
        BindFloorLabel();
        RefreshFloorLabel();

        for (int i = 0; i < equippedSlots.Length; i++)
        {
            equippedSlots[i].hc = this;
            equippedSlots[i].index = i;
        }
    
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].hc = this;
            inventorySlots[i].index = i;
        }
    }

    private void Update()
    {
        // Retry setup until GameManager and player initialization are ready.
        if (boundState == null || player == null)
        {
            TryInitialize();
            return;
        }
        
        scoreText.text = "Score: " + GameManager.Instance.Score;
        RefreshFloorLabel();

        RefreshWeaponIcon();
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if ((gameOverMenu != null && gameOverMenu.activeSelf))
                //|| (victoryMenu != null && victoryMenu.activeSelf))
                return;

            if (isPaused)
                Resume();
            else
                Pause();
        }
        
        if (Input.GetKeyDown(KeyCode.Tab) && menuOpen)
        {
            menuOpen = !menuOpen;
            augmentMenu.SetActive(menuOpen);
            blockerImage.enabled = !blockerImage.enabled;
            DeselectAllSlots();
            Time.timeScale = 1;
        } else if (Input.GetKeyDown(KeyCode.Tab) && !menuOpen && !isPaused)
        {
            Time.timeScale = 0; // pauses game while in menu
            menuOpen = !menuOpen;
            augmentMenu.SetActive(menuOpen);
            blockerImage.enabled = !blockerImage.enabled;
        }
        
        reloadBar.gameObject.SetActive(combatScript.isReloading);
        reloadText.gameObject.SetActive(combatScript.isReloading);
        if (combatScript.isReloading)
            reloadBar.value = combatScript.reloadProgress;

        if (combatScript.weapon != null)
        {
            WeaponData curWeapon = combatScript.weapon;
            curAmmoText.gameObject.SetActive(!curWeapon.isMelee);
            ammoReservesText.gameObject.SetActive(!curWeapon.isMelee);
            if (!curWeapon.isMelee)
            {
                noAmmoText.gameObject.SetActive(curWeapon.ammoReserves == 0 && curWeapon.currentAmmo == 0);
                curAmmoText.text = curWeapon.currentAmmo + " / " + curWeapon.ammoCapacity;
                ammoReservesText.text = curWeapon.ammoReserves.ToString();
            }
        }
        
        Room currentRoom = EnemySpawnManager.Instance.GetCurrentRoom();
        if (currentRoom != null)
        {
            numWaveText.gameObject.SetActive(true);
            if (currentRoom.isBossRoom)
            {
                numWaveText.text = "Boss Room";
            }
            else
            {
                numEnemiesText.gameObject.SetActive(true);
                numWaveText.text = currentRoom.currentWave + " / " + currentRoom.numWaves + " Waves";
                numEnemiesText.text = currentRoom.enemyCount + " Enemies";
            }
        }
        else
        {
            numWaveText.text = "";
            numEnemiesText.text = "";
        }
    }

    private void TryInitialize()
    {
        BindState();
        BindPlayer();
        RefreshAll();
    }

    private void BindState()
    {
        if (GameManager.Instance == null || GameManager.Instance.PlayerState == null)
        {
            return;
        }

        if (boundState == GameManager.Instance.PlayerState)
        {
            return;
        }

        UnbindState();

        boundState = GameManager.Instance.PlayerState;
        boundState.OnHealthChanged += HandleHealthChanged;
        boundState.OnStaminaChanged += HandleStaminaChanged;
    }

    private void UnbindState()
    {
        if (boundState == null)
        {
            return;
        }

        boundState.OnHealthChanged -= HandleHealthChanged;
        boundState.OnStaminaChanged -= HandleStaminaChanged;
        boundState = null;
    }

    private void BindPlayer()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<playerController>();
        }

        if (player == null)
        {
            return;
        }

        combatScript = player.GetComponent<playerCombat>();
        player.OnEquippedWeaponChanged -= HandleWeaponChanged;
        player.OnEquippedWeaponChanged += HandleWeaponChanged;
    }

    private void UnbindPlayer()
    {
        if (player == null)
        {
            return;
        }

        player.OnEquippedWeaponChanged -= HandleWeaponChanged;
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        UpdateFill(healthFill, currentHealth, maxHealth);
    }

    private void HandleStaminaChanged(float currentStamina, float maxStamina)
    {
        UpdateFill(staminaFill, currentStamina, maxStamina);
    }

    private void HandleWeaponChanged(WeaponData weapon)
    {
        UpdateWeaponIcon(weapon);
    }

    private void RefreshAll()
    {
        RefreshBars();
        RefreshWeaponIcon();
        RefreshFloorLabel();
    }

    private void BindFloorLabel()
    {
        if (floorLabelText != null)
        {
            return;
        }

        TMP_Text[] labels = GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text label in labels)
        {
            if (label.gameObject.name == "Floor Label")
            {
                floorLabelText = label;
                return;
            }
        }
    }

    private void RefreshFloorLabel()
    {
        if (floorLabelText == null || GameManager.Instance == null)
        {
            return;
        }

        floorLabelText.text = "Floor " + GameManager.Instance.CurrentFloor + "/" + maxFloor;
    }

    private void RefreshBars()
    {
        if (boundState == null)
        {
            UpdateFill(healthFill, 0f, 1f);
            UpdateFill(staminaFill, 0f, 1f);
            return;
        }

        UpdateFill(healthFill, boundState.CurrentHealth, boundState.MaxHealth);
        UpdateFill(staminaFill, boundState.CurrentStamina, boundState.MaxStamina);
    }

    private void RefreshWeaponIcon()
    {
        if (player == null || player.weapons == null || player.curSlot < 0 || player.curSlot >= player.weapons.Length)
        {
            UpdateWeaponIcon(null);
            return;
        }

        Weapon currentWeapon = player.weapons[player.curSlot];
        if (currentWeapon == null)
        {
            UpdateWeaponIcon(null);
            return;
        }

        WeaponData iconSource = currentWeapon.augmentedData;
        if (iconSource == null || iconSource.weaponSprite == null)
        {
            iconSource = currentWeapon.baseData;
        }

        UpdateWeaponIcon(iconSource);
    }

    public void ShowBossHealth(EnemyController boss)
    {
        if (trackedBoss != null)
        {
            trackedBoss.OnHealthChanged -= HandleBossHealthChanged;
            trackedBoss.OnDefeated -= HandleBossDefeated;
        }
        
        trackedBoss = boss;

        bossHealthRoot.SetActive(true);

        trackedBoss.OnHealthChanged += HandleBossHealthChanged;
        trackedBoss.OnDefeated += HandleBossDefeated;

        if (bossHealthIntroRoutine != null)
        {
            StopCoroutine(bossHealthIntroRoutine);
        }

        bossHealthIntroRoutine = StartCoroutine(FillBossHealthIntro(
            trackedBoss.CurrentHealth,
            trackedBoss.MaxHealth
        ));
    }

    private void HandleBossHealthChanged(EnemyController boss, int currentHealth, int maxHealth)
    {
        if (bossHealthIntroRoutine != null)
        {
            StopCoroutine(bossHealthIntroRoutine);
            bossHealthIntroRoutine = null;
        }

        bossHealthFill.fillAmount = maxHealth > 0
            ? (float)currentHealth / maxHealth
            : 0f;
    }

    private void HandleBossDefeated(EnemyController boss)
    {
        HideBossHealth();
    }

    private void HideBossHealth()
    {
        if (bossHealthIntroRoutine != null)
        {
            StopCoroutine(bossHealthIntroRoutine);
            bossHealthIntroRoutine = null;
        }

        if (trackedBoss != null)
        {
            trackedBoss.OnHealthChanged -= HandleBossHealthChanged;
            trackedBoss.OnDefeated -= HandleBossDefeated;
            trackedBoss = null;
        }
        
        bossHealthRoot.SetActive(false);
    }

    private IEnumerator FillBossHealthIntro(int currentHealth, int maxHealth)
    {
        float targetFill = maxHealth > 0
            ? (float)currentHealth / maxHealth
            : 0f;

        float duration = 0.75f;
        float elapsed = 0f;

        bossHealthFill.fillAmount = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bossHealthFill.fillAmount = Mathf.Lerp(0f, targetFill, elapsed / duration);
            yield return null;
        }

        bossHealthFill.fillAmount = targetFill;
        bossHealthIntroRoutine = null;
    }
    
    

    private void UpdateWeaponIcon(WeaponData weapon)
    {
        if (weaponIcon == null)
        {
            return;
        }

        if (weapon == displayedWeapon)
        {
            return;
        }

        displayedWeapon = weapon;

        if (weapon == null || weapon.weaponSprite == null)
        {
            weaponIcon.sprite = null;
            weaponIcon.enabled = false;
            return;
        }

        weaponIcon.sprite = weapon.weaponSprite;
        weaponIcon.enabled = true;
        weaponIcon.preserveAspect = true;
    }

    private void UpdateFill(Image fillImage, float currentValue, float maxValue)
    {
        if (fillImage == null)
        {
            return;
        }

        float fillAmount = maxValue <= 0f ? 0f : Mathf.Clamp01(currentValue / maxValue);
        fillImage.fillAmount = fillAmount;
    }

    public void AddInventoryAugment(AugmentData augment)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].augment == null)
            {
                inventorySlots[i].AddItem(augment);
                return;
            }
        }
    }
    
    public void SetAugmentPromptVisible(bool visible)
    {
        if (pickupPrompt != null)
        {
            pickupPrompt.gameObject.SetActive(visible);
        }
    }
    
    public void EquipAugment(AugmentData augment)
    {
        for (int i = 0; i < equippedSlots.Length; i++)
        {
            if (equippedSlots[i].augment == null)
            {
                equippedSlots[i].AddItem(augment);
                return;
            }
        }
    }
    
    public void DeselectAllSlots()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].selectedIndicator.SetActive(false);
            inventorySlots[i].selected = false;
        }
        
        for (int i = 0; i < equippedSlots.Length; i++)
        {
            equippedSlots[i].selectedIndicator.SetActive(false);
            equippedSlots[i].selected = false;
        }

        infoImage.enabled = false;
        infoName.text = "";
        infoText.text = "";
    }
    
    public void RefreshEquippedSlots(AugmentData[] augments, int count)
    {
        for (int i = 0; i < equippedSlots.Length; i++)
        {
            if (i < count && augments[i] != null)
                equippedSlots[i].AddItem(augments[i]);
            else
                equippedSlots[i].ClearItem();
        }
    }

    public void RefreshInventorySlots(AugmentData[] augments, int count)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < count && augments[i] != null)
                inventorySlots[i].AddItem(augments[i]);
            else
                inventorySlots[i].ClearItem();
        }
    }
    
    public void ShowRoomCleared()
    {
        StartCoroutine(RoomClearedAlert());
    }
    
    IEnumerator RoomClearedAlert()
    {
        roomClearedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        roomClearedText.gameObject.SetActive(false);
    }
    
    public void ShowIncomingWave(int countdown)
    {
        StartCoroutine(IncomingWaveAlert(countdown));
    }
    
    private IEnumerator IncomingWaveAlert(int countdown)
    {
        incomingWaveText.gameObject.SetActive(true);
        waveCountdownText.gameObject.SetActive(true);
    
        while (countdown > 0)
        {
            waveCountdownText.text = countdown.ToString();
        
            float elapsed = 0f;
            while (elapsed < 1f)
            {
                incomingWaveText.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.8f);
                incomingWaveText.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.3f);
                elapsed += 1.1f;
            }
        
            countdown--;
        }
    
        incomingWaveText.gameObject.SetActive(false);
        waveCountdownText.gameObject.SetActive(false);
    }
    
    private void Pause()
    {
        if (menuOpen)
        {
            menuOpen = false;
            augmentMenu.SetActive(false);
            blockerImage.enabled = false;
        }
        
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GameOver()
    {
        if (gameOverMenu != null)
            gameOverMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Victory()
    {
        if (victoryMenu != null)
            victoryMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void QuitToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title Screen");
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

