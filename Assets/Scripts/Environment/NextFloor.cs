using UnityEngine;

public class NextFloor : MonoBehaviour
{

    [SerializeField] private Room room;

    private bool active = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!active && room.isCleared)
        {
            active = true;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("player touched");
        if (!Application.isPlaying) return;
        if (other.CompareTag("Player") && active)
        {
            GameManager.Instance.LoadScene("Floor2");
        }
    }
}
