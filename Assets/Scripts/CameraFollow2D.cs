using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;            
    public float followSmoothTime = 0.15f; 
    public float zoomSpeed = 5f;       
    public float minZoom = 5f;          
    public float maxZoom = 12f;        
    private Camera cam;
    private float targetZoom;
    private Vector3 velocity = Vector3.zero;
    [Header("Aiming")]
    public float aimMaxOffset = 3f;
    public float aimSmoothMultiplier = 1.5f;
    private Vector3 aimOffset;
    public float deadZoneRadius = 1.5f;


    void Awake()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = new Vector3(target.position.x + aimOffset.x, target.position.y + aimOffset.y, transform.position.z);
        float smooth = Input.GetMouseButton(1) ? followSmoothTime / aimSmoothMultiplier : followSmoothTime;

        transform.position = Vector3.SmoothDamp(transform.position,targetPos,ref velocity,smooth);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scroll * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * 5f);

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Vector3 direction = mouseWorld - target.position;
        float distance = direction.magnitude;

        Vector3 desiredOffset = Vector3.zero;

        if (Input.GetMouseButton(1))
        {
            if (distance > deadZoneRadius)
            {
                Vector3 dirNormalized = direction.normalized;

                float t = Mathf.InverseLerp(deadZoneRadius, deadZoneRadius + aimMaxOffset, distance);
                t = t * t;

                desiredOffset = dirNormalized * (t * aimMaxOffset);
            }
        }
        aimOffset = Vector3.Lerp(aimOffset, desiredOffset, Time.deltaTime * 8f);
    }
}
