using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BountySelectionUI : MonoBehaviour
{
    [System.Serializable]
    private class BountySlotView
    {
        [SerializeField] private GameObject slotRoot;
        [SerializeField] private Button posterButton;
        [SerializeField] private Image posterImage;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Text legacyTitleText;
        [SerializeField] private Text legacyDescriptionText;
        [SerializeField] private TMP_Text bountyAmount;

        public void Clear()
        {
            if (posterButton != null)
            {
                posterButton.onClick = new Button.ButtonClickedEvent();
                posterButton.interactable = false;
            }

            if (posterImage != null)
            {
                posterImage.sprite = null;
                posterImage.enabled = false;
            }

            if (bountyAmount != null)
            {
                bountyAmount.text = string.Empty;
                bountyAmount.gameObject.SetActive(false);
            }

            SetTitle(string.Empty);
            SetDescription(string.Empty);
        }

        public void SetVisible(bool visible)
        {
            SetSlotActive(visible);
            SetObjectVisible(posterButton != null ? posterButton.gameObject : null, visible);
            SetObjectVisible(posterImage != null ? posterImage.gameObject : null, visible);
            SetObjectVisible(titleText != null ? titleText.gameObject : null, visible);
            SetObjectVisible(descriptionText != null ? descriptionText.gameObject : null, visible);
            SetObjectVisible(legacyTitleText != null ? legacyTitleText.gameObject : null, visible);
            SetObjectVisible(legacyDescriptionText != null ? legacyDescriptionText.gameObject : null, visible);
            SetObjectVisible(bountyAmount != null ? bountyAmount.gameObject : null, visible);
        }

        public void SetSlotActive(bool active)
        {
            GameObject root = GetSlotRoot();
            if (root != null)
            {
                root.SetActive(active);
            }
        }

        public void Bind(BountyData bounty, UnityEngine.Events.UnityAction onSelected)
        {
            if (posterButton != null)
            {
                posterButton.onClick = new Button.ButtonClickedEvent();
                posterButton.interactable = bounty != null;

                if (bounty != null && onSelected != null)
                {
                    posterButton.onClick.AddListener(onSelected);
                }
            }

            if (posterImage != null)
            {
                posterImage.sprite = bounty != null ? bounty.PosterSprite : null;
                posterImage.enabled = bounty != null && bounty.PosterSprite != null;
                posterImage.preserveAspect = true;
            }

            if (bountyAmount != null)
            {
                bountyAmount.raycastTarget = false;
                bountyAmount.gameObject.SetActive(bounty != null);
                SetBountyText(bounty != null ? bounty.Bounty.ToString() : string.Empty);
            }

            SetTitle(bounty != null ? bounty.DisplayName : string.Empty);
            SetDescription(bounty != null ? bounty.Description : string.Empty);
        }

        private void SetTitle(string value)
        {
            if (titleText != null)
            {
                titleText.text = value;
            }

            if (legacyTitleText != null)
            {
                legacyTitleText.text = value;
            }
        }

        private void SetDescription(string value)
        {
            if (descriptionText != null)
            {
                descriptionText.text = value;
            }

            if (legacyDescriptionText != null)
            {
                legacyDescriptionText.text = value;
            }
        }

        private void SetBountyText(string bountyVal)
        {
            if (bountyAmount != null)
            {
                bountyAmount.text = bountyVal + " Credits";
            }
        }

        private void SetObjectVisible(GameObject target, bool visible)
        {
            if (target != null)
            {
                target.SetActive(visible);
            }
        }

        private GameObject GetSlotRoot()
        {
            if (slotRoot != null)
            {
                return slotRoot;
            }

            Transform current = GetAnySlotTransform();
            while (current != null)
            {
                if (current.name.StartsWith("PosterSlot"))
                {
                    slotRoot = current.gameObject;
                    return slotRoot;
                }

                current = current.parent;
            }

            return null;
        }

        private Transform GetAnySlotTransform()
        {
            if (posterButton != null) return posterButton.transform;
            if (posterImage != null) return posterImage.transform;
            if (titleText != null) return titleText.transform;
            if (descriptionText != null) return descriptionText.transform;
            if (legacyTitleText != null) return legacyTitleText.transform;
            if (legacyDescriptionText != null) return legacyDescriptionText.transform;
            if (bountyAmount != null) return bountyAmount.transform;
            return null;
        }
    }

    [Header("Panel")]
    [SerializeField] private GameObject panelRoot;

    [Header("Poster Slots")]
    [SerializeField] private BountySlotView[] posterSlots = new BountySlotView[3];

    [Header("Header Text")]
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private Text legacyHeaderText;
    [SerializeField] private string openingHeader = "Choose Your Bounty";
    [SerializeField] private SceneController sceneController;
    [SerializeField] private string nextSceneName = "MainScene";
    [SerializeField] private float selectionDelay = 0.2f;

    private bool isBound;
    private bool selectionInProgress;

    private void OnEnable()
    {
        TryBind();
    }

    private void Start()
    {
        TryBind();
    }

    private void Update()
    {
        if (!isBound)
        {
            TryBind();
        }
    }

    private void OnDisable()
    {
        Unbind();
    }

    private void TryBind()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (!isBound)
        {
            GameManager.Instance.OnBountyOfferChanged += RefreshOffers;
            GameManager.Instance.OnOpeningBountySelected += HandleOpeningBountySelected;
            isBound = true;
        }

        RefreshOffers();
    }

    private void Unbind()
    {
        if (!isBound || GameManager.Instance == null)
        {
            isBound = false;
            return;
        }

        GameManager.Instance.OnBountyOfferChanged -= RefreshOffers;
        GameManager.Instance.OnOpeningBountySelected -= HandleOpeningBountySelected;
        isBound = false;
    }

    private void RefreshOffers()
    {
        SetPanelVisible(true);

        if (GameManager.Instance != null && GameManager.Instance.IsFinalFloor())
        {
            RefreshFinalBossOffer();
            return;
        }

        SetHeader(openingHeader);

        BountyData[] offeredBounties = GameManager.Instance != null
            ? GameManager.Instance.GetOfferedBounties()
            : System.Array.Empty<BountyData>();

        for (int i = 0; i < posterSlots.Length; i++)
        {
            if (posterSlots[i] == null)
            {
                continue;
            }

            if (i < offeredBounties.Length && offeredBounties[i] != null)
            {
                posterSlots[i].SetSlotActive(true);
                posterSlots[i].SetVisible(true);
                BountyData selectedBounty = offeredBounties[i];
                posterSlots[i].Bind(selectedBounty, () => SelectBounty(selectedBounty));
            }
            else
            {
                posterSlots[i].SetSlotActive(true);
                posterSlots[i].SetVisible(true);
                posterSlots[i].Clear();
            }
        }
    }

    private void RefreshFinalBossOffer()
    {
        SetHeader("Choose Your Final Bounty");

        int centerSlotIndex = posterSlots.Length / 2;
        BountyData finalBossBounty = GameManager.Instance.GetFinalBossBounty();

        for (int i = 0; i < posterSlots.Length; i++)
        {
            if (posterSlots[i] == null)
            {
                continue;
            }

            bool isCenterSlot = i == centerSlotIndex;
            posterSlots[i].SetSlotActive(isCenterSlot);

            if (isCenterSlot)
            {
                posterSlots[i].SetVisible(true);
                posterSlots[i].Bind(finalBossBounty, () => SelectFinalBoss(finalBossBounty));
            }
            else
            {
                posterSlots[i].Clear();
            }
        }
    }

    private void SelectBounty(BountyData bounty)
    {
        if (selectionInProgress)
        {
            return;
        }

        if (GameManager.Instance == null || bounty == null)
        {
            Debug.Log("No game manager");
            return;
        }

        selectionInProgress = true;
        GameManager.Instance.SelectOpeningBounty(bounty);
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private void SelectFinalBoss(BountyData bounty)
    {
        if (selectionInProgress)
        {
            return;
        }

        if (GameManager.Instance == null || bounty == null)
        {
            Debug.Log("No final boss bounty");
            return;
        }

        selectionInProgress = true;
        GameManager.Instance.SelectFinalBoss(bounty);
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private void HandleOpeningBountySelected(BountyData bounty)
    {
        if (selectionInProgress)
        {
            return;
        }

        selectionInProgress = true;
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private void SetPanelVisible(bool isVisible)
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(isVisible);
        }
    }

    private void SetHeader(string value)
    {
        if (headerText != null)
        {
            headerText.text = value;
        }

        if (legacyHeaderText != null)
        {
            legacyHeaderText.text = value;
        }
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        if (selectionDelay > 0f)
        {
            yield return new WaitForSeconds(selectionDelay);
        }

        if (sceneController != null)
        {
            if (GameManager.Instance.CurrentFloor < 5) GameManager.Instance.LoadScene(nextSceneName);
            else GameManager.Instance.LoadScene("Boss1");
        }
        else
        {
            if (GameManager.Instance.CurrentFloor < 5) GameManager.Instance.LoadScene(nextSceneName);
            else GameManager.Instance.LoadScene("Boss1");
        }

        SetPanelVisible(false);
    }
}
