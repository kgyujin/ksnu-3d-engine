using UnityEngine;

public class WandItem : MonoBehaviour
{
    
    public float raycast_distance = 10f;

    // Transform values for proper positioning when equipped
    public Vector3 equipPosition = Vector3.zero;
    public Vector3 equipRotation = new Vector3(90f, 0f, 0f); // X=90 degrees to point forward
    public Vector3 equipScale = new Vector3(1f, 1f, 1f);

    // Optional: Reference to any special effects or components
    public ParticleSystem wandEffect;

    // You can add methods here to activate wand special effects or behaviors
    public void ActivateWand()
    {
        if (wandEffect != null)
        {
            wandEffect.Play();
        }
    }

}
