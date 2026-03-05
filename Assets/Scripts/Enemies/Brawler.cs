using UnityEngine;

public class Brawler : MonoBehaviour
{
    SpriteRenderer brownPants;
    public int shitPantsChance;
    Color shitBrown;
    private float red = .4f, blue = .04f, green = .2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        brownPants = GetComponent<SpriteRenderer>();
        shitBrown = new Color(red, green, blue);
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "Player"){
            if(Random.Range(0, shitPantsChance) < 1){
                brownPants.color = shitBrown;
                Debug.Log("SHIT YOUR PaNTS");
            }
            
        }
    }
}
