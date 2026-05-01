using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float distance;
    Rigidbody2D rb;
    private Transform target;
    Vector2 moveDirection;
    
    void Start()
    {
        target = EnemySpawnManager.Instance.player;
    }
    
    void Update()
    {
        if(target){
            distance = Vector2.Distance(transform.position, target.position);
            Vector3 direction = (target.position - transform.position).normalized;
            moveDirection = direction; 
        }
        
    }

    private void FixedUpdate(){
        if(target)
        {
            rb.linearVelocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }

}
