using UnityEngine;

public class Coin : MonoBehaviour
{
    public float baseRadius = 10f;
    public float speed = 13f;
    public int coinValue = 1;
    
    private Rigidbody2D rb;
    private Transform player;
    private float currentRadius;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Получаем бонус радиуса подбора из магазина
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
            AudioManager.Instance?.PlayCoinPickup();
            
            if (CoinManager.Instance != null)
                CoinManager.Instance.AddCoins(coinValue);
                
            Destroy(gameObject);
        }
    }
}