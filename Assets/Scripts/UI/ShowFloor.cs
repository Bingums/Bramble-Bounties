using TMPro;
using UnityEngine;

public class ShowFloor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameManager GM;
    [SerializeField] private int maxFloor = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateFloorText();
    }

    private void UpdateFloorText()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        GameManager gameManager = GameManager.Instance != null ? GameManager.Instance : GM;
        if (text == null || gameManager == null)
        {
            return;
        }

        text.SetText("Floor " + gameManager.CurrentFloor + "/" + maxFloor);
    }
}
