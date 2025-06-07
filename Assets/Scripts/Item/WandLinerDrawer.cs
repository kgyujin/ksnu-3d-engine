using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WandLineDrawer : MonoBehaviour
{
    public RaycastObjectMover objectMover;
    public Transform wandTip; // Ȱ��ȭ�� �������� ��

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
        if (objectMover == null) return;

        // �� �����̰� ���� ���õ� �����̰� �ƴ϶�� ���� ����
        if (objectMover.WandTip == null || objectMover.WandTip.gameObject != this.gameObject)
        {
            lineRenderer.enabled = false;
            return;
        }

        // ������ ���� �ʱ�ȭ
        if (wandTip == null)
        {
            Transform tipTransform = objectMover.WandTip.Find("Tip");
            wandTip = tipTransform != null ? tipTransform : objectMover.WandTip;
        }

        Transform target = objectMover.GetSelectedObject();

        // Ÿ�� ���� ���� ���� ǥ��
        if (wandTip != null && target != null)
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