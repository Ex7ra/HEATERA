
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]//This is an attribute that tells Unity the GameObject needs a LineRenderer component for this script.
public class ModuleDamageVisual : MonoBehaviour
{
    public float flashDuration = 0.2f;
    public float lineWidth = 0.06f;
    public int circleSegments = 48;

    public Color damageColor = new Color(1f, 0.65f, 0f, 0.5f);
    public Color destroyedColor = new Color(1f, 0f, 0f, 0.5f);

    public string sortingLayerName = "Default";
    public int sortingOrder = 100;

    private Collider2D moduleCollider;
    private LineRenderer line;
    private Coroutine flashRoutine;

    private void Awake()
    {
        moduleCollider = GetComponent<Collider2D>();
        line = GetComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.loop = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 0;
        line.enabled = false;

        line.sortingLayerName = sortingLayerName;
        line.sortingOrder = sortingOrder;

        Shader shader = Shader.Find("Sprites/Default");

        if (shader != null)
            line.material = new Material(shader);

        if (moduleCollider == null)
            Debug.LogWarning(name + " has no Collider2D!");
    }

    private void Update()
    {
        if (line.enabled)
            DrawColliderOutline();
    }

    public void ShowDamage(bool destroyed)
    {
        if (moduleCollider == null)
            return;

        line.startColor = destroyed ? destroyedColor : damageColor;
        line.endColor = destroyed ? destroyedColor : damageColor;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        DrawColliderOutline();
        line.enabled = true;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(flashDuration);

        line.enabled = false;
        flashRoutine = null;
    }

    private void DrawColliderOutline()
    {
        if (moduleCollider is BoxCollider2D box)
        {
            Vector2 half = box.size * 0.5f;
            Vector2 offset = box.offset;

            Vector3[] corners =
            {
                offset + new Vector2(-half.x, -half.y),
                offset + new Vector2(-half.x,  half.y),
                offset + new Vector2( half.x,  half.y),
                offset + new Vector2( half.x, -half.y),
                offset + new Vector2(-half.x, -half.y)
            };

            line.positionCount = corners.Length;

            for (int i = 0; i < corners.Length; i++)
            {
                line.SetPosition(i,box.transform.TransformPoint(corners[i]));
            }
        }
        else if (moduleCollider is CircleCollider2D circle)
        {
            int segments = Mathf.Max(12, circleSegments);
            line.positionCount = segments + 1;

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                Vector2 point = circle.offset + new Vector2(Mathf.Cos(angle) * circle.radius,Mathf.Sin(angle) * circle.radius);
                line.SetPosition(i, circle.transform.TransformPoint(point));
            }
        }
    }
}

