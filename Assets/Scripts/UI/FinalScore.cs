using TMPro;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.SetText("Score: " + GameManager.Instance.Score + "\nHigh-Score: " + GameManager.Instance.HighScore);
    }
}
