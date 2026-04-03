using System.Collections;
using UnityEngine;
using TMPro; 

public class FloatingText : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float floatSpeed = 1f;
    
    private TextMeshPro textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    // New public method to be called by other scripts
    public void SetText(string message)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();
        
        textMesh.text = message;
        StartCoroutine(FadeAndDestroy());
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        Color startColor = textMesh.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}