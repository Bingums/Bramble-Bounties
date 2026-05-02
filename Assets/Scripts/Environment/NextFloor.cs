using UnityEngine;

public class NextFloor : MonoBehaviour
{

    [SerializeField] private Room room;

    public string nextScene;

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
        Debug.Log("player touched");
        if (!Application.isPlaying) return;
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.LoadScene("Bounty Testing");
        }
    }
}
