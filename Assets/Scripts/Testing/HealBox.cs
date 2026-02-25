using UnityEngine;

public class HealBox : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        IHealable playerHealable = other.gameObject.GetComponent<IHealable>(); 

        if(playerHealable != null)
        {
            playerHealable.Heal(1f); 
            Debug.Log("Healed 1 point!");
        }
    }
}
