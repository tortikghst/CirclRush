using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    public float baseRadius = 10f;
    public float speed = 13f;
    public int expValue = 1;
    
    private Rigidbody2D rb;
    private Transform player;
    private float currentRadius;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Получаем бонус радиуса подбора
        float radiusBonus = PlayerPrefs.GetFloat("Bonus_PickupRadius", 0f);
        currentRadius = baseRadius + radiusBonus;
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= currentRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 newPosition = rb.position + direction * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance?.PlayExpPickup();
            
            int finalExp = expValue;
            float multiplier = PlayerPrefs.GetFloat("Bonus_ExpMultiplier", 1f);
            finalExp = Mathf.RoundToInt(expValue * multiplier);
            
            if (ExpManager.Instance != null)
                ExpManager.Instance.AddExp(finalExp);
                
            Destroy(gameObject);
        }
    }
}