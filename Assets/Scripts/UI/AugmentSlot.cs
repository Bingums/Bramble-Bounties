using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AugmentSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image augmentImage;
    [SerializeField] public GameObject selectedIndicator;
    public bool selected;
    
    [SerializeField] public Image infoImage;
    [SerializeField] public TMP_Text infoName;
    [SerializeField] public TMP_Text infoText;

    public HUDController hc;
    
    public AugmentData augment;
    public int index = 0;

    public void AddItem(AugmentData data)
    {   
        this.augment = data.GetCopy();
        
        augmentImage.sprite = augment.augmentSprite;
        augmentImage.color = augment.augmentColor;
        augmentImage.enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!selected)
            {
                hc.DeselectAllSlots();
                selectedIndicator.SetActive(true);
                selected = true;
                if (augment != null)
                {
                    infoImage.sprite = augment.augmentSprite;
                    infoImage.color = augment.augmentColor;
                    infoImage.enabled = true;
                    infoName.text = augment.augmentName;
                    infoText.text = BuildInfo();
                }
            }
            else
            {
                selectedIndicator.SetActive(false);
                selected = false;
                infoImage.enabled = false;
                infoName.text = "";
                infoText.text = "";
            }
        } else if (eventData.button == PointerEventData.InputButton.Right)
        {
            selectedIndicator.SetActive(false);
            selected = false;
            if (augment == null) return;
            selectedIndicator.SetActive(false);
            selected = false;
            playerController.Instance.RemoveAugment(index);
            hc.DeselectAllSlots();
        }
    }
    
    private string BuildInfo()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
    
        string[] rarityNames = { "Common", "Rare", "Epic", "Legendary" };
        string rarityStr = augment.rarity >= 0 && augment.rarity < rarityNames.Length ? rarityNames[augment.rarity] : "Unknown";
        sb.AppendLine(rarityStr);
        sb.AppendLine(augment.description);
        sb.AppendLine();
    
        foreach (StatModifier modifier in augment.statModifiers)
        {
            string changeStr;
            switch (modifier.changeType)
            {
                case ChangeType.Flat:
                    changeStr = (modifier.changeValue >= 0 ? "+" : "") + modifier.changeValue;
                    break;
                case ChangeType.Percentage:
                    changeStr = (modifier.changeValue >= 0 ? "*" : "") + (modifier.changeValue * 100f) + "%";
                    break;
                case ChangeType.Multiplier:
                    changeStr = "x" + modifier.changeValue;
                    break;
                default:
                    changeStr = modifier.changeValue.ToString();
                    break;
            }
        
            sb.AppendLine($"{modifier.statType}: {changeStr}");
        }
    
        return sb.ToString();
    }
    
    public void ClearItem()
    {
        augment = null;
        augmentImage.sprite = null;
        augmentImage.enabled = false;
        selectedIndicator.SetActive(false);
        selected = false;
    }
}
