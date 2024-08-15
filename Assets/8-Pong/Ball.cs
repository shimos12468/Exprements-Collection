using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    [SerializeField, Min(0f)]
    float maxStartXSpeed=2,maxSpeed = 20, startSpeed = 10 , constantYSpeed = 8,extents =0.5f;

    [SerializeField]
    ParticleSystem bounceParticleSystem ,startParticleSystem ,trailParticleSystem;

    [SerializeField]
    int bounceParticleEmission = 20,
     startParticleEmission = 200;

    public float Extents => extents;
    public Vector2 Position => position;
    public Vector2 Velocity => velocity;
    Vector2 position, velocity;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    void EmitBounceParticles(float x, float z, float rotation)
    {
        print("jhi");
        ParticleSystem.ShapeModule shape = bounceParticleSystem.shape;
        shape.position = new Vector3(x, 0f, z);
        shape.rotation = new Vector3(0f, rotation, 0f);
        bounceParticleSystem.Emit(bounceParticleEmission);
    }
    public void UpdateVisualization()
    {
         trailParticleSystem.transform.position= transform.localPosition = new Vector3(position.x, 0, position.y);
    }

    public void SetXPositionAndSpeed(float start ,float speedFactor,float deltaTime)
    {
        velocity.x = maxSpeed*speedFactor;
        position.x = start+velocity.x*deltaTime;
    }


    public void Move()
    {
        position += velocity * Time.deltaTime;
    }

    public void StartNewGame()
    {
        position = Vector2.zero;
        UpdateVisualization();
        velocity = new Vector2(Random.Range(-maxStartXSpeed, maxStartXSpeed), -constantYSpeed);
        gameObject.SetActive(true);
        startParticleSystem.Emit(startParticleEmission);
        SetTrailEmission(true);
        trailParticleSystem.Play();
    }

    private void SetTrailEmission(bool v)
    {
        ParticleSystem.EmissionModule emission = trailParticleSystem.emission;
        emission.enabled = v;
    }

    public void EndGame()
    {
        position.x = 0f;
        gameObject.SetActive(false);
        SetTrailEmission(false);
    }
    public void BounceX(float boundary)
    {
        float durationAfterBounce = (position.x - boundary) / velocity.x;
        position.x = 2* boundary-position.x;
        velocity.x = -velocity.x;
        EmitBounceParticles(
            boundary,
            position.y - velocity.y * durationAfterBounce,
            boundary < 0f ? 90f : 270f
        );
    }
    public void BounceY(float boundary)
    {
        float durationAfterBounce = (position.y - boundary) / velocity.y;
        position.y = 2 * boundary - position.y;
        velocity.y = -velocity.y;
        EmitBounceParticles(
           position.x - velocity.x * durationAfterBounce    ,
            boundary,
           boundary < 0f ? 0f : 180f
       );
    }
}
