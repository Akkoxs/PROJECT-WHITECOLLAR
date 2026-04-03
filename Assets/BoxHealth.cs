using UnityEngine;

public class BoxHealth : MonoBehaviour, IDamageable
{
    [Header("Hit Effects")]
    [SerializeField] private GameObject hitTextPrefab;
    [SerializeField] private Vector3 textOffset = new Vector3(0, 1.5f, 0); 

    public void TakeDamage(float dmgAmount)
    {
        if (hitTextPrefab != null)
        {
            // 1. Spawn the object and store it in a variable
            GameObject textInstance = Instantiate(hitTextPrefab, transform.position + textOffset, Quaternion.identity);
            
            // 2. Get the FloatingText component from that instance
            FloatingText floatingTextScript = textInstance.GetComponent<FloatingText>();

            // 3. Pass the custom string (or dmgAmount.ToString()) to the script
            if (floatingTextScript != null)
            {
                floatingTextScript.SetText("HIT!"); 
                // Or: floatingTextScript.SetText(dmgAmount.ToString());
            }
        }

        Destroy(gameObject);
    }
}