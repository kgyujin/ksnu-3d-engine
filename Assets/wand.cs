using JetBrains.Annotations;
using UnityEngine;

public class Wand : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float raycastdistance = 0f;


    public Vector3 Get_item_position() { 
        return transform.position;
    }
   
}
