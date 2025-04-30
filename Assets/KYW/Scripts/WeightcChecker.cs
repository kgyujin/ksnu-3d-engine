using UnityEngine;

public class WeightChecker : MonoBehaviour
{
    public float pressMinMass = 20f;

    private Foothold foothold;
    private Door door;

    private void Awake()
    {
        foothold = GetComponent<Foothold>();
        door = GetComponentInChildren<Door>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        Rigidbody rb = collider.attachedRigidbody;

        if (rb != null)
        {
            if (rb.mass >= pressMinMass)
            {
                foothold.SetTrigger(true);
                foothold.Press();
                door.Open();
            }
            else
            {
                foothold.SetTrigger(false);
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        Rigidbody rb = collider.attachedRigidbody;

        if (rb != null && rb.mass >= pressMinMass)
        {
            foothold.Release();
            door.Close();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null && rb.mass < pressMinMass)
        {
            foothold.SetTrigger(true);
        }
    }
}
