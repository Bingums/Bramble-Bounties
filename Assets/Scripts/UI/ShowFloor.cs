using TMPro;
using UnityEngine;

public class ShowFloor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameManager GM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.SetText("FLOOR " + GM.CurrentFloor);
    }
}
