using UnityEngine;

public class NextFloor : MonoBehaviour
{
    [SerializeField]
    private Room room;

    private bool hasTriggered;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("player touched goal");
        if (!Application.isPlaying) return;
        if(hasTriggered) return;
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            
            if (GameManager.Instance.CurrentFloor < 5)
            {
                GameManager.Instance.AdvanceFloor();
                GameManager.Instance.LoadScene("Bounty Testing");
            }
            else
            {
                GameManager.Instance.LoadScene("VICTORY");
            }
        }
    }
}
