using UnityEngine;

public class HurtBox : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        IDamageable playerDamageable = other.gameObject.GetComponent<IDamageable>(); 

        if(playerDamageable != null)
        {
            
            playerDamageable.TakeDamage(1f); 
            Debug.Log("Took 1 dmg!");
        }
    }
}
