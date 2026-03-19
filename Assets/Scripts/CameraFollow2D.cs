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

    void Awake()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, followSmoothTime);

        transform.rotation = Quaternion.identity;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scroll * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * 5f);
    }
}
