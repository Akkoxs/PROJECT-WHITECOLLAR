using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent

[RequireComponent(typeof(NavMeshAgent))]
public class BasicEnemy : MonoBehaviour, IDamageable
{
    [Header("Hit Effects")]
    [SerializeField] private GameObject hitTextPrefab;
    [SerializeField] private Vector3 textOffset = new Vector3(0, 1.5f, 0); 

    [Header("AI Settings")]
    [SerializeField] private float detectionRange = 10f;
    
    private Transform playerTransform;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Find the player by tag. Make sure your player object has the "Player" tag!
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Pathfind to the player if they are within detection range
        if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(playerTransform.position);
        }
        else
        {
            // Optional: Stop moving if the player gets too far away
            agent.ResetPath();
        }
    }

    public void TakeDamage(float dmgAmount)
    {
        // 1. Spawn floating text (Keep your awesome setup)
        if (hitTextPrefab != null)
        {
            GameObject textInstance = Instantiate(hitTextPrefab, transform.position + textOffset, Quaternion.identity);
            FloatingText floatingTextScript = textInstance.GetComponent<FloatingText>();

            if (floatingTextScript != null)
            {
                floatingTextScript.SetText("HIT!"); 
            }
        }

        // 2. Destroy the enemy
        Destroy(gameObject);
    }

    // Visualizes the detection range in the Unity Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}