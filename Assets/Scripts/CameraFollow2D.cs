using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;            // The object to follow
    public float followSmoothTime = 0.15f; // How smoothly the camera follows
    public float zoomSpeed = 5f;        // Mouse scroll zoom speed
    public float minZoom = 5f;          // Minimum camera size
    public float maxZoom = 12f;         // Maximum camera size

    private Camera cam;
    private float targetZoom;
    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // --- Follow the target smoothly ---
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, followSmoothTime);

        // --- Keep camera rotation fixed ---
        transform.rotation = Quaternion.identity;

        // --- Zoom with mouse wheel ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scroll * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * 5f);
    }
}
