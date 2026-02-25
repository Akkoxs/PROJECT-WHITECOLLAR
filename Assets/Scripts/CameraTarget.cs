using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float influence = 0.1f;  
    [SerializeField] float maxOffset = 3f;    
    [SerializeField] float smoothSpeed = 5f; 

    Transform player;
    Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
        player = FindObjectOfType<PlayerControllerLower>().transform;
    }

    void Update()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 targetPos = Vector3.Lerp(player.position, hit.point, influence);
            
            Vector3 offset = targetPos - player.position;
            offset = Vector3.ClampMagnitude(offset, maxOffset);
            
            // Smooth the movement instead of snapping
            transform.position = Vector3.Lerp(transform.position, player.position + offset, Time.deltaTime * smoothSpeed);
        }
    }

    //vibe voded
    void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(player.position, transform.position);

        Gizmos.color = Color.white;
        DrawCircle(player.position, maxOffset, 32);
    }

    void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angle = 0f;
        Vector3 lastPoint = center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            angle += 360f / segments;
            Vector3 nextPoint = center + new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                0f,
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius
            );
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }
}
