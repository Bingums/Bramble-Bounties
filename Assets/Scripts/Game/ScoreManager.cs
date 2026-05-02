using System.Collections;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int score;
    
    public HUDController hc;

    void Awake()
    {
        instance = this;
        StartCoroutine(WaitForHUD());
    }

    public void AddScore(int points)
    {
        score += points;
        hc.ChangeScore(score);
    }
    
    IEnumerator WaitForHUD()
    {
        while (hc == null)
        {
            hc = GameObject.Find("HUD Container").GetComponentInChildren<HUDController>();
            yield return null;
        }
    }
}