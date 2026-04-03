using UnityEngine;
using UnityEngine.SceneManagement; // Crucial for loading scenes!

public class WinTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string sceneToLoad = "TempEnd";

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the zone has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Load the victory scene!
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}