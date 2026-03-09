using UnityEngine;

public class PimpManager : MonoBehaviour
{
    public int enemyCount;
    public delegate void SlapOClock();
    public static event SlapOClock slap;

    private bool slapping = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyCount == 0 && !slapping){
            slap.Invoke();

            slapping = true;
        }
    }
}
