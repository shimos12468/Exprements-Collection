using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivelyCamera : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
        springStrength = 100f,
        dampingStrength = 10f,
        jostleStrength = 40f,
        pushtrength = 1f;

    Vector3 velocity,anchorPosition;

    public void JostileY() => velocity.y += jostleStrength;

    public void PushXZ(Vector2 impulse)
    {
        velocity.x += pushtrength*impulse.x;
        velocity.z += jostleStrength*impulse.y;
    }
    void Start()
    {
        anchorPosition = transform.localPosition;    
    }
    private void LateUpdate()
    {
        Vector3 displacement = anchorPosition - transform.localPosition;
        Vector3 acceleration = springStrength * displacement - dampingStrength * velocity;
        velocity += acceleration * Time.deltaTime;
        transform.localPosition += velocity * Time.deltaTime;
    }
}
