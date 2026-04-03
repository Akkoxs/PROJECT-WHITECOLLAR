using TMPro;
using UnityEngine;

public class StateDisplay : MonoBehaviour
{
    [SerializeField] PlayerControllerUpper upperBody;
    [SerializeField] PlayerControllerLower lowerBody;
    [SerializeField] Transform playerTransform; // drag your player here in Inspector
    
    TextMeshPro tmp;
    Camera mainCam;

    void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        mainCam = Camera.main;
    }

    void Update()
    {
        tmp.text = $"Upper: {upperBody.CurrentStateName}\nLower: {lowerBody.CurrentStateName}";
        transform.position = playerTransform.position;
        transform.rotation = mainCam.transform.rotation;
    }
}