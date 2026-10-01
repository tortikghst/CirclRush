using UnityEngine;
using System.Collections;

public class MagniteScript : MonoBehaviour
{
    public float speed = 13f;
    public float attractionDuration = 3f;
    public float tempRadius = 1000f;
    
    [SerializeField] private GameObject visualPart;
    [SerializeField] private GameObject particle;
    
    private float originalCoinRadius;
    private float originalExpOrbRadius;
    
    private void OnTriggerEnter2D(Collider2D other)
    {   
        if (other.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance?.PlayMagnetPickup();
            // Отключаем визуальную часть
            if (visualPart != null) visualPart.SetActive(false);
            if (particle != null) particle.SetActive(false);
            
            // Сохраняем оригинальные радиусы и временно увеличиваем
            IncreaseCollectionRadius();
            
            // Запускаем притяжение предметов
            StartCoroutine(AttractItemsCoroutine(other.transform));
            
            // Уничтожаем магнит после окончания
            Destroy(gameObject, attractionDuration + 0.5f);
        }
    }
    
    void IncreaseCollectionRadius()
    {
        // Для Coin
        Coin[] allCoins = FindObjectsOfType<Coin>();
        foreach (Coin coin in allCoins)
        {
            if (coin != null)
            {
                // Сохраняем оригинальный радиус
                originalCoinRadius = coin.baseRadius;
                coin.baseRadius = tempRadius;
            }
        }
        
        // Для ExpOrb
        ExpOrb[] allExpOrbs = FindObjectsOfType<ExpOrb>();
        foreach (ExpOrb expOrb in allExpOrbs)
        {
            if (expOrb != null)
            {
                originalExpOrbRadius = expOrb.baseRadius;
                expOrb.baseRadius = tempRadius;
            }
        }
    }
    
    void ResetCollectionRadius()
    {
        // Восстанавливаем оригинальные радиусы
        Coin[] allCoins = FindObjectsOfType<Coin>();
        foreach (Coin coin in allCoins)
        {
            if (coin != null && originalCoinRadius > 0)
            {
                coin.baseRadius = originalCoinRadius;
            }
        }
        
        ExpOrb[] allExpOrbs = FindObjectsOfType<ExpOrb>();
        foreach (ExpOrb expOrb in allExpOrbs)
        {
            if (expOrb != null && originalExpOrbRadius > 0)
            {
                expOrb.baseRadius = originalExpOrbRadius;
            }
        }
    }
    
    IEnumerator AttractItemsCoroutine(Transform playerTransform)
    {
        float timer = 0f;
        
        while (timer < attractionDuration)
        {
            timer += Time.deltaTime;
            
            // Притягиваем монеты
            AttractObjectsWithTag("Coin", playerTransform);
            // Притягиваем сферы опыта
            AttractObjectsWithTag("Exp Orb", playerTransform);
            
            yield return null;
        }
        
        // Восстанавливаем обычные радиусы
        ResetCollectionRadius();
    }
    
    void AttractObjectsWithTag(string tag, Transform target)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == null) continue;
            
            Rigidbody2D rb = objects[i].GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (target.position - objects[i].transform.position).normalized;
                rb.linearVelocity = direction * speed;
            }
        }
    }
}