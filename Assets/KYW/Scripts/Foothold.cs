using UnityEngine;

public class Foothold : MonoBehaviour
{
    public float pressedY = -0.4f;
    public float originalY;
    private Transform platformTransform;
    private Collider platformCollider;

    void Awake()
    {
        platformTransform = transform;
        originalY = platformTransform.position.y;

        platformCollider = GetComponent<Collider>();
        platformCollider.isTrigger = true;
    }

    public void Press()
    {
        platformTransform.position = new Vector3(platformTransform.position.x, pressedY, platformTransform.position.z);
    }

    public void Release()
    {
        platformTransform.position = new Vector3(platformTransform.position.x, originalY, platformTransform.position.z);
    }

    public void SetTrigger(bool isTrigger)
    {
        if (platformCollider != null)
            platformCollider.isTrigger = isTrigger;
    }
}
