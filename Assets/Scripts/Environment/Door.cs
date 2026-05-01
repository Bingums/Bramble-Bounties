using UnityEngine;
using System;

public class Door : MonoBehaviour
{

    [SerializeField] Sprite unlocked;
    [SerializeField] Sprite locked;

    private int timeToCheck = 60;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeToCheck--;

        if (timeToCheck <= 0)
        {
            if (GetComponent<CircleCollider2D>().enabled) GetComponent<SpriteRenderer>().sprite = unlocked;
            else GetComponent<SpriteRenderer>().sprite = locked;

            timeToCheck = UnityEngine.Random.Range(1, 121);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!Application.isPlaying) return;
        if (other.CompareTag("Player"))
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (!Application.isPlaying) return;
        if (other.CompareTag("Player"))
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
