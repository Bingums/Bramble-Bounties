using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float distance;
    Rigidbody2D rb;
    Transform target;
    Vector2 moveDirection;

    private void Awake(){
         rb = GetComponent<Rigidbody2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.Find("player Variant").transform;
    
    }

    // Update is called once per frame
    void Update()
    {
        if(target){
            float distance = Vector2.Distance(transform.position, target.position);
            if(distance > 8f)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                moveDirection = direction; 
            }
        }
        
    }

    private void FixedUpdate(){
        if(target)
        {
            rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }

}
