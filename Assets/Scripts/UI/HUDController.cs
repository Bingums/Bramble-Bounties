using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Bar Fills")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;

    [Header("Weapon UI")]
    [SerializeField] private Image weaponIcon;
    [SerializeField] private playerController player;
    [SerializeField] private Slider reloadBar;
    [SerializeField] private TMP_Text reloadText;
    [SerializeField] private TMP_Text curAmmoText;
    [SerializeField] private TMP_Text ammoReservesText;
    
    [Header("Augment UI")]
    [SerializeField] private GameObject augmentMenu;
    [SerializeField] public Image infoImage;
    [SerializeField] public TMP_Text infoName;
    [SerializeField] public TMP_Text infoText;
    
    [Header("Wave UI")]
    [SerializeField] public TMP_Text numWaveText;
    [SerializeField] public TMP_Text numEnemiesText;
    
    [Header("Score UI")]
    [SerializeField] public TMP_Text scoreText;
    
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

    private bool menuOpen = false;
    
    private void OnEnable()
    {
        TryInitialize();
    }

    private void OnDisable()
    {
        UnbindState();
        UnbindPlayer();
        displayedWeapon = null;
    }
    
    private void Start()
    {
        reloadBar.gameObject.SetActive(false);
        reloadText.gameObject.SetActive(false);
        curAmmoText.gameObject.SetActive(false);
        ammoReservesText.gameObject.SetActive(false);
        noAmmoText.gameObject.SetActive(false);
        fullInventoryText.gameObject.SetActive(false);
        augmentMenu.gameObject.SetActive(false);
        numWaveText.gameObject.SetActive(false);
        numEnemiesText.gameObject.SetActive(false);
        incomingWaveText.gameObject.SetActive(false);
        waveCountdownText.gameObject.SetActive(false);
        roomClearedText.gameObject.SetActive(false);

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

        RefreshWeaponIcon();
        
        if (Input.GetKeyDown(KeyCode.Tab) && menuOpen)
        {
            menuOpen = !menuOpen;
            augmentMenu.SetActive(menuOpen);
            DeselectAllSlots();
            Time.timeScale = 1;
        } else if (Input.GetKeyDown(KeyCode.Tab) && !menuOpen)
        {
            Time.timeScale = 0; // pauses game while in menu
            menuOpen = !menuOpen;
            augmentMenu.SetActive(menuOpen);
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

    public void ChangeScore(int score)
    {
        scoreText.text = "Score: " + score;
    }
}

