using UnityEngine;

public class PotionScript : MonoBehaviour
{
    public float healAmount = 30f;  
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance?.PlayHeal();
            
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.Heal(healAmount);
                
            Destroy(gameObject);
        }
    }
}
