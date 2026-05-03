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
        score = 0;
        StartCoroutine(WaitForHUD());
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
