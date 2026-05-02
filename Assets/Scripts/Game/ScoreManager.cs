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
        Debug.Log("Method started");
        score += points;

        if (hc != null)
        {
            hc.ChangeScore(score);
        }
        Debug.Log("Score Changed");
    }
    
    IEnumerator WaitForHUD()
    {
        while (hc == null)
        {
            GameObject hudContainer = GameObject.Find("HUD Container");
            if (hudContainer != null)
            {
                hc = hudContainer.GetComponentInChildren<HUDController>();
            }

            yield return null;
        }
    }
}
