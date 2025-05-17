using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WandLineDrawer : MonoBehaviour
{
    public RaycastObjectMover objectMover;
    public Transform wandTip; // 활성화된 지팡이의 끝

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.03f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.blue;
        lineRenderer.enabled = false;

        if (objectMover == null)
            objectMover = FindObjectOfType<RaycastObjectMover>();
    }

    void Update()
    {
        if (objectMover == null || wandTip == null) return;

        Transform target = objectMover.GetSelectedObject();

        if (target != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, wandTip.position);
            lineRenderer.SetPosition(1, target.position);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}
