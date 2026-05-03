using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BountySelectionUI : MonoBehaviour
{
    [System.Serializable]
    private class BountySlotView
    {
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
        SetHeader(openingHeader);
        SetPanelVisible(true);

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
                BountyData selectedBounty = offeredBounties[i];
                posterSlots[i].Bind(selectedBounty, () => SelectBounty(selectedBounty));
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
            sceneController.LoadScene(nextSceneName);
        }
        else
        {
            GameManager.Instance.LoadScene(nextSceneName);
        }

        SetPanelVisible(false);
    }
}
