using UnityEngine;


public class GameManager : MonoBehaviour
{
    
    
}
public interface IDamagiable
{
    float hp {get; set;}
    float currentHp{get; set;}
    float damageResist{get;set;}
    public void Hit( float amount){}
}
